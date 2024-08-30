using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Classes
{
    public class GamePrice
    {
        public int Match_ID { get; set; }
        public DateTime Price_Date { get; set; }
        public List<Bookmaker> Bookmaker { get; set; } = new List<Bookmaker>();
    }
}
