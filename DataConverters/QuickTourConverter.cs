using Entities;
using System.Text;

namespace DataConverters
{
    public class QuickTourConverter : IDataConverter
    {
        public GeoPosition Convert(byte[] data, int length)
        {
            if (length > data.Length || length != 92 && length != 47) throw new ArgumentException();

            if (length == 92)
            {
                return ConvertFromString(Encoding.ASCII.GetString(data, 0, length));
            }
            else
            {
                return ConvertFromBinary(data, length);
            }
        }

        private GeoPosition ConvertFromBinary(byte[] rawData, int length)
        {
            string longitudeStr = $"{rawData[17]:X2}{rawData[18]:X2}{rawData[19]:X2}{rawData[20]:X2}{rawData[21]:X2}";
            double longitude;
            if (Double.TryParse(longitudeStr.TrimEnd('E', 'W'), out longitude))
            {
                longitude /= 10000.0;
            }
            return new GeoPosition()
            {
                DeviceId = Int64.Parse($"{rawData[1]:X2}{rawData[2]:X2}{rawData[3]:X2}{rawData[4]:X2}{rawData[5]:X2}"),
                DateTime = new DateTime(
                Int32.Parse(rawData[11].ToString("X2")) + 2000,
                Int32.Parse(rawData[10].ToString("X2")),
                Int32.Parse(rawData[9].ToString("X2")),
                Int32.Parse(rawData[6].ToString("X2")),
                Int32.Parse(rawData[7].ToString("X2")),
                Int32.Parse(rawData[8].ToString("X2"))
                ).ToLocalTime(),
                Latitude = Double.Parse($"{rawData[12]:X2}{rawData[13]:X2}.{rawData[14]:X2}{rawData[15]:X2}"),
                Longitude = longitude
            };
        }

        private GeoPosition ConvertFromString(string str)
        {
            try
            {
                string[] data = str.Split(',');

                return new GeoPosition()
                {
                    Quality = data[0],
                    DeviceId = Int64.Parse(data[1]),
                    DateTime = new DateTime(
                    Int32.Parse(data[11].Substring(4, 2)) + 2000,
                    Int32.Parse(data[11].Substring(2, 2)),
                    Int32.Parse(data[11].Substring(0, 2)),
                    Int32.Parse(data[3].Substring(0, 2)),
                    Int32.Parse(data[3].Substring(2, 2)),
                    Int32.Parse(data[3].Substring(4, 2))).ToLocalTime(),
                    Latitude = Double.Parse(data[5]),
                    Longitude = Double.Parse(data[7])
                };
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                return new GeoPosition();
            }
        }
    }
}