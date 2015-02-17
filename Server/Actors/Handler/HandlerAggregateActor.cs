namespace Server.Actors.Handler
{
    using System.Collections.Generic;
    using System.Net.Sockets;
    using Akka.Actor;

    public sealed class HandlerAggregateActor : TypedActor, IHandle<HandlerAggregateActor.AddHandler>, IHandle<HandlerAggregateActor.RemoveHandler>
    {
        public sealed class AddHandler
        {
            private readonly string accountName;
            private readonly TcpClient client;

            public AddHandler(string accountName, TcpClient client)
            {
                this.accountName = accountName;
                this.client = client;
            }

            public TcpClient Client
            {
                get { return client; }
            }

            public string AccountName
            {
                get { return accountName; }
            }
        }

        public sealed class RemoveHandler
        {
            private readonly string accountName;

            public RemoveHandler(string accountName)
            {
                this.accountName = accountName;
            }

            public string AccountName
            {
                get { return accountName; }
            }
        }

        private readonly IDictionary<string, ActorRef> clientHandlers = new Dictionary<string, ActorRef>();

        public void Handle(AddHandler message)
        {
            clientHandlers[message.AccountName] = Context.ActorOf(Props.Create(() => new ClientHandlerActor(message.AccountName, message.Client)), message.AccountName);
        }

        public void Handle(RemoveHandler message)
        {
            var handler = clientHandlers[message.AccountName];
            Context.Stop(handler);
            clientHandlers.Remove(message.AccountName);
        }
    }
}
