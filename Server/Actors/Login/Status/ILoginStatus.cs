namespace Server.Actors.Login.Status
{
    using System.Net.Sockets;

    public interface ILoginStatus
    {
        TcpClient Client { get; }
    }
}