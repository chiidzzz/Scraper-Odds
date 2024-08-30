using System.ComponentModel;

namespace Scraper.Classes
{
    public class Fixtures
    {
        [DisplayName("League")]
        public string league { get; set; }
        [DisplayName("")]
        public string KickOff_UTC2 { get; set; }
        [DisplayName("Date")]
        public System.DateTime Date { get; set; }
        [DisplayName("Home")]
        public string Home { get; set; }
        [DisplayName("League_Position_H")]
        public string positionH { get; set; }
        [DisplayName("Versus")]
        public string versus { get; set; }
        [DisplayName("Away")]
        public string Away { get; set; }
        [DisplayName("League_Position_A")]
        public string positionA { get; set; }
    }
}
