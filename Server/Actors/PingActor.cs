namespace Server.Actors
{
    using System;
    using Akka.Actor;

    public sealed class PingActor : TypedActor, IHandle<PingActor.Ping>
    {
        public sealed class Ping { }

        public void Handle(Ping message)
        {
            throw new NotImplementedException();
        }
    }
}