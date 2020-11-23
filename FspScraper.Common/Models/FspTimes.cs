using System.ComponentModel.DataAnnotations;

namespace FspScraper.Common.Models
{
    public class FspTimes
    {
        [Key]
        public string RegistrationNum { get; set; }
        public decimal? Hobbs { get; set; }
        public int? AirTime { get; set; }
        public decimal? Tach1 { get; set; }
        public decimal? Tach2 { get; set; }
        public decimal? Prop1 { get; set; }
        public decimal? Prop2 { get; set; }
        public decimal AircraftTotal { get; set; }
        public decimal? Engine1Total { get; set; }
        public decimal? Engine2Total { get; set; }
        public int? Cycles { get; set; }
    }
}