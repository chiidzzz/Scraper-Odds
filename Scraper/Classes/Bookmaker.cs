using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Classes
{
    public class Bookmaker
    {
        public int Bookmaker_ID { get; set; }
        public Prices Prices { get; set; } = new Prices();
    }
}
