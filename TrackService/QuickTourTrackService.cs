using DataConverters;
using Domain;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace TrackService
{
    public class QuickTourTrackService: TrackService
    {
        private readonly IPAddress _ipAddress = IPAddress.Parse("192.168.31.56");
        private readonly int _port = 1024;
        IDataConverter _converter = new QuickTourConverter();

        public QuickTourTrackService(IServiceScopeFactory factory) : base(factory)
        {
            Addr = _ipAddress;
            Port = _port;
        }

        public override void SaveData(byte[] buffer, int received)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<AppDataContext>();
                context?.Set<GeoPosition>().Add(_converter.Convert(buffer, received));
                context?.SaveChanges();
            }
        }
    }
}