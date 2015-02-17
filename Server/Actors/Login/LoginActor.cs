namespace Server.Actors.Login
{
    using System.IO;
    using System.Net.Sockets;
    using Akka.Actor;
    using Data;
    using Handler;
    using Newtonsoft.Json;
    using Status;
    using Failure = Status.Failure;
    using Success = Status.Success;

    public sealed class LoginActor : TypedActor, IHandle<LoginActor.NewClientConnected>, IHandle<Failure>, IHandle<Success>
    {
        public sealed class NewClientConnected
        {
            private readonly TcpClient client;

            public NewClientConnected(TcpClient client)
            {
                this.client = client;
            }

            public TcpClient Client
            {
                get { return client; }
            }
        }

        private readonly ActorRef loginValidation = Context.ActorOf<LoginValidationActor>(ActorNames.LoginValidation);

        public void Handle(NewClientConnected message)
        {
            var stream = message.Client.GetStream();
            var serializer = new JsonSerializer();
            var reader = new StreamReader(stream);
            var jtr = new JsonTextReader(reader);
            var packet = serializer.Deserialize<IncomingPacket>(jtr);
            if (packet.CommandType != CommandType.Login)
            {
                Self.Tell(new Failure(message.Client, "Invalid command: 'Login' has to be used first."));
            }
            else
            {
                var data = packet.ToTyped<AccountInfo>();
                loginValidation.Ask<ILoginStatus>(new LoginValidationActor.LogonRequest(data, message.Client))
                               .PipeTo(Self);
            }
        }

        public void Handle(Failure message)
        {
            if (message.Client.Connected)
            {
                var stream = message.Client.GetStream();
                var writer = new StreamWriter(stream);
                writer.WriteLineAsync(
                    JsonConvert.SerializeObject(new OutgoingPacket {ResponseType = ResponseType.LoginFailure, Payload = message.Message}))
                      .ContinueWith(_ => writer.FlushAsync().Wait());
            }
        }

        public void Handle(Success message)
        {
            if (message.Client.Connected)
            {
                var stream = message.Client.GetStream();
                var writer = new StreamWriter(stream);
                writer.WriteLineAsync(
                    JsonConvert.SerializeObject(new OutgoingPacket { ResponseType = ResponseType.ConnectionAccepted }))
                      .ContinueWith(_ => writer.FlushAsync().Wait());

                var aggregator = Context.System.ActorSelection(ActorPaths.ClientHandlerAggregator);
                aggregator.Tell(new HandlerAggregateActor.AddHandler(message.AccountName, message.Client));
            }
        }
    }
}
