namespace Server.Actors.Login.Status
{
    using System.Net.Sockets;

    public sealed class Success : ILoginStatus
    {
        private readonly string accountName;
        private readonly TcpClient client;

        public Success(string accountName, TcpClient client)
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
}