namespace Server.Actors.Handler
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.ExceptionServices;
    using System.Text;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Data;
    using Newtonsoft.Json;

    public sealed class ClientHandlerActor : TypedActor, IHandle<ClientHandlerActor.Success>, IHandle<ClientHandlerActor.Failure>
    {
        private interface IReadResult { }

        public sealed class Failure : IReadResult
        {
            private readonly ExceptionDispatchInfo exceptionDispatchInfo;

            public Failure(Exception exception = null)
            {
                if (exception != null)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
            }

            public ExceptionDispatchInfo DispatchInfo
            {
                get { return exceptionDispatchInfo; }
            }
        }

        public sealed class Success : IReadResult
        {
            private readonly IncomingPacket packet;

            public Success(IncomingPacket packet)
            {
                this.packet = packet;
            }

            public IncomingPacket Packet
            {
                get { return packet; }
            }
        }

        private readonly string name;
        private readonly TcpClient client;

        public ClientHandlerActor(string name, TcpClient client)
        {
            this.name = name;
            this.client = client;
        }

        protected override void PreStart()
        {
            ReceiveClientMessage();
        }

        private void ReceiveClientMessage()
        {
            if (client.Connected)
            {
                var stream = client.GetStream();
                var reader = new StreamReader(stream, Encoding.UTF8);
                reader.ReadLineAsync().ContinueWith<IReadResult>(result =>
                                                                 {
                                                                     try
                                                                     {
                                                                         var line = result.Result;
                                                                         return
                                                                             new Success(JsonConvert.DeserializeObject<IncomingPacket>(line));
                                                                     }
                                                                     catch (Exception e)
                                                                     {
                                                                         return new Failure(e);
                                                                     }
                                                                 },
                    TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                      .PipeTo(Self);
            }
            else
            {
                Context.Parent.Tell(new HandlerAggregateActor.RemoveHandler(name));
            }
        }

        protected override void PostStop()
        {
            client.Close();
        }

        public void Handle(Success message)
        {
            if (message.Packet.CommandType == CommandType.Disconnect)
            {
                Context.Parent.Tell(new HandlerAggregateActor.RemoveHandler(name));
            }
            else
            {
                ReceiveClientMessage();
            }
        }

        public void Handle(Failure message)
        {
            Context.Parent.Tell(new HandlerAggregateActor.RemoveHandler(name));
            if (message.DispatchInfo != null)
            {
                message.DispatchInfo.Throw();
            }
        }
    }
}