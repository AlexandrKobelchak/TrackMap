using System.Net;

namespace TrackService
{
    public interface ITrackService
    {
        IPAddress Addr { set; }
        int Port { set; }
        void SaveData(byte[] buffer, int received);
    }
}