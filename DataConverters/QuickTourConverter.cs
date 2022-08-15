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
                Latitude = GetLatitude(rawData[12], rawData[13], rawData[14], rawData[15], rawData[16]),
                Longitude = GetLongitude(rawData[17], rawData[18], rawData[19], rawData[20], rawData[21])
            };
        }

        private double GetLatitude(byte b0, byte b1, byte b2, byte b3, byte b4)
        {
            string min = $"0.{b1:X2}{b2:X2}{b3:X2}";
            double latitudeGrad = Double.Parse($"{b0:X2}");
            double latitudeMin = Double.Parse(min) / 0.6;
            if (b4 == 06) return latitudeGrad + latitudeMin;
            else return -latitudeGrad - latitudeMin;
        }

        private double GetLongitude(byte b0, byte b1, byte b2, byte b3, byte b4)
        {
            string grad = $"{b0:X2}{b1:X2}".Substring(0, 3);
            string minutes = "0."+$"{b1:X2}{b2:X2}{b3:X2}{b4:X2}".Substring(1, 6);
            double longitudeGrad = Double.Parse(grad);
            double longitudeMin = Double.Parse(minutes) / 0.6;
            return (longitudeGrad + longitudeMin) * ((b4 & 0xF) == 0xE ? 1 : -1);
        }
        private double GetLatitude(string str, string NS)
        {
            string[] data = str.Split('.');
            double latitudeGrad = Double.Parse(data[0].Substring(0, 2));
            double latitudeMin = Double.Parse($"0.{data[0].Substring(2, 2)}{data[1]}") / 0.6;
            return (latitudeGrad + latitudeMin) * (NS == "N" ? 1 : -1);
        }
        private double GetLongitude(string str, string WE)
        {
            string[] data = str.Split('.');
            double longitudeGrad = Double.Parse(data[0].Substring(0, 3));
            double longitudeMin = Double.Parse("0." + data[0].Substring(3, 2) + data[1]) / 0.60;
            return (longitudeGrad + longitudeMin) * (WE == "E" ? 1 : -1);
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
                    Latitude = GetLatitude(data[5], data[6]),
                    Longitude = GetLongitude(data[7], data[8])
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