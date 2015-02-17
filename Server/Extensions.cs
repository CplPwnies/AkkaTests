namespace Server
{
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Data;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class JsonExtensions
    {
        public static T ToTyped<T>(this IncomingPacket packet)
        {
            return JObject.FromObject(packet.Payload).ToObject<T>();
        }
    }

    public static class TcpClientExtensions
    {
        public static Task<IncomingPacket> ReceiveAsync(this TcpClient client)
        {
            var stream = client.GetStream();
            var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadLineAsync().ContinueWith(line => JsonConvert.DeserializeObject<IncomingPacket>(
                line.Result),
                TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
