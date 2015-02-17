namespace Data
{
    public sealed class OutgoingPacket
    {
        public ResponseType ResponseType { get; set; }
        public object Payload { get; set; }
    }
}