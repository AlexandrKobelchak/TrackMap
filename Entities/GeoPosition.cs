using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Table("positions")]
    public class GeoPosition : DbEntity
    { 
        [Column("device")]
        public long DeviceId { get; set; }

        [Column("quality")]
        [StringLength(2)]
        public string? Quality { get; set; }

        [Column("datetime")]
        public DateTime DateTime { get; set; }

        [Column("latitude")]
        public double Latitude { get; set; }

        [Column("longitude")]
        public double Longitude { get; set; }

        [Column("speed")]
        public double Speed { get; set; }

        [Column("angle")]
        public int CourseAngle { get; set; }
    }
}