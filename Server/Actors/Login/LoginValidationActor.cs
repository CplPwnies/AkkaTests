namespace Server.Actors.Login
{
    using System.Net.Sockets;
    using Akka.Actor;
    using Data;

    public sealed class LoginValidationActor : TypedActor, IHandle<LoginValidationActor.LogonRequest>
    {
        public sealed class LogonRequest
        {
            private readonly AccountInfo accountInfo;
            private readonly TcpClient client;
            public LogonRequest(AccountInfo accountInfo, TcpClient client)
            {
                this.accountInfo = accountInfo;
                this.client = client;
            }

            public AccountInfo AccountInfo
            {
                get { return accountInfo; }
            }

            public TcpClient Client
            {
                get { return client; }
            }
        }

        public void Handle(LogonRequest message)
        {
            var accountName = message.AccountInfo.AccountName;
            if (string.IsNullOrEmpty(accountName))
            {
                Context.Parent.Tell(new Status.Failure(message.Client, "Account name is missing"));
            }
            // Now this should be a database check etc etc.
            else if (accountName.Length > 10)
            {
                Context.Parent.Tell(new Status.Failure(message.Client, "Invalid credentials"));
            }
            else
            {
                Context.Parent.Tell(new Status.Success(accountName, message.Client));
            }
        }
    }
}
