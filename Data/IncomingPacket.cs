namespace Data
{
    public sealed class IncomingPacket
    {
        public CommandType CommandType { get; set; }
        public object Payload { get; set; }
    }
}