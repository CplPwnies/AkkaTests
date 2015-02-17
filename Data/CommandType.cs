namespace Data
{
    public enum CommandType
    {
        Ping,
        Login,
        Disconnect
    }

    public enum ResponseType
    {
        LoginFailure,
        ConnectionAccepted
    }
}