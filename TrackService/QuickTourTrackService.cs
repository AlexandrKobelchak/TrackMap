using DataConverters;
using Domain;
using Entities;
using System.Net;

namespace TrackService
{
    public class QuickTourTrackService: TrackService
    {
        private readonly IPAddress _ipAddress = IPAddress.Parse("192.168.31.56");
        private readonly int _port = 1024;
        IDataConverter _converter = new QuickTourConverter();

        public QuickTourTrackService(AppDataContext context) : base(context)
        {
            Addr = _ipAddress;
            Port = _port;
        }

        public override void SaveData(byte[] buffer, int received)
        {
            _context.Set<GeoPosition>().Add(_converter.Convert(buffer, received));
            _context.SaveChanges();
        }
    }
}