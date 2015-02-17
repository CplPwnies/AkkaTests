namespace Server
{
    using System;
    using System.Net;
    using Actors;
    using Actors.Handler;
    using Actors.Network;
    using Akka.Actor;

    class Program
    {
       
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("TestNetworkingSystem");
            system.ActorOf(Props.Create(() => new ConnectionHandlingActor(new IPEndPoint(IPAddress.Loopback, 7001))), ActorNames.NetworkConnection);
            system.ActorOf<HandlerAggregateActor>(ActorNames.ClientHandlerAggregator);
            Console.WriteLine("System is running...");
            Console.ReadLine();
        }
    }
}
