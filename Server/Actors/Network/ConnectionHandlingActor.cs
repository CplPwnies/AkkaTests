namespace Server.Actors.Network
{
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Login;

    public sealed class ConnectionHandlingActor : ReceiveActor
    {
        public sealed class IncomingConnection
        {
            private readonly TcpClient client;

            public IncomingConnection(TcpClient client)
            {
                this.client = client;
            }

            public TcpClient Client
            {
                get { return client; }
            }
        }

        private readonly TcpListener listener;
        private readonly ActorRef clientHandler = Context.ActorOf<LoginActor>(ActorNames.Login);

        public ConnectionHandlingActor(IPEndPoint endPoint)
        {
            listener = new TcpListener(endPoint);

            Receive<IncomingConnection>(connection =>
                                        {
                                            var client = connection.Client;
                                            if (client.Connected)
                                            {
                                                clientHandler.Tell(new LoginActor.NewClientConnected(client));
                                            }
                                            listener.AcceptTcpClientAsync()
                                                    .ContinueWith(tcp => new IncomingConnection(tcp.Result),
                                                        TaskContinuationOptions.AttachedToParent &
                                                        TaskContinuationOptions.ExecuteSynchronously).PipeTo(Self);
                                        });
            listener.Start();
            listener.AcceptTcpClientAsync()
                    .ContinueWith(tcp => new IncomingConnection(tcp.Result),
                        TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                    .PipeTo(Self);
        }
    }
}