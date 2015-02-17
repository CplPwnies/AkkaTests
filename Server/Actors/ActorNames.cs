namespace Server.Actors
{
    public static class ActorPaths
    {
        public const string ClientHandlerAggregator = "/user/" + ActorNames.ClientHandlerAggregator;
        public const string NetworkConnection = "/user/" + ActorNames.NetworkConnection;
    }

    public static class ActorNames
    {
        public const string ClientHandlerAggregator = "clients";
        public const string NetworkConnection = "network";
        public const string Login = "login";
        public const string LoginValidation = "validation";
    }
}
