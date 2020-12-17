using System.ComponentModel.DataAnnotations;

namespace FspScraper.Common.Models
{
    public class FspTimes
    {
        [Key]
        public string RegistrationNum { get; set; }
        public decimal? Hobbs { get; set; }
        public int? AirTime { get; set; }
        public decimal? Prop1Total { get; set; }
        public decimal? Prop2Total { get; set; }
        public decimal? Engine1Total { get; set; }
        public decimal? Engine2Total { get; set; }
    }
}