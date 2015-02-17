namespace Server.Actors.Login.Status
{
    using System.Net.Sockets;

    public sealed class Failure : ILoginStatus
    {
        private readonly TcpClient client;
        private readonly string message;

        public Failure(TcpClient client, string message)
        {
            this.client = client;
            this.message = message;
        }

        public TcpClient Client
        {
            get { return client; }
        }

        public string Message
        {
            get { return message; }
        }
    }
}