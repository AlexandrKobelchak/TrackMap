using System.Net.Sockets;

namespace TrackService
{
    public class StateObject
    {
        public const int bufferSize = 256;
        public byte[] Buffer { get; } = new byte[bufferSize];
        public Socket? WorkSocket { get; }
        public StateObject(Socket? workSocket) { WorkSocket = workSocket; }
    }
}