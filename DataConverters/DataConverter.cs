using Entities;

namespace DataConverters
{
    public interface IDataConverter
    {
        GeoPosition Convert(byte[] data, int length);
    }
}