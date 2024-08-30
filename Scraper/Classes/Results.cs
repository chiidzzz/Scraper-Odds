using System;
using System.ComponentModel;

namespace Scraper.Classes
{
    public class Results
    {
        public string Year { get; set; }
        public string League { get; set; }
        public DateTime Date { get; set; }
        public string Home_Team { get; set; }
        public string Away_Team { get; set; }
        public string ScoreFT { get; set; }
        public int FTHG { get; set; }
        public int FTAG { get; set; }
        public int FTGoals { get; set; }
        public string FTResult { get; set; }
        public string ScoreHT { get; set; }
        public int HTHG { get; set; }
        public int HTAG { get; set; }
        public string HTResult { get; set; }
        public int HTGoals { get; set; }
        public int Goals { get; set; }
        public string BTS { get; set; }

        private int ParseValue(string txt)
        {
            int val;
            if (int.TryParse(txt, out val))
            {
                return val;
            }
            else
            {
                return 0;
            }
        }
        public int GetFTHG(string score)
        {
            string[] fthg = score.Split('-');
            return ParseValue(fthg[0].Trim());
        }
        public int GetFTAG(string score)
        {
            string[] ftag = score.Split('-');
            return ParseValue(ftag[1].Trim());
        }
        public int GetFTGoals(string score)
        {
            string[] ftGoals = score.Split('-');
            return (Convert.ToInt32(ftGoals[1]) + Convert.ToInt32(ftGoals[0]));
        }
        public string GetFTResult(string score)
        {
            string[] ftResult = score.Split('-');
            if (Convert.ToInt32(ftResult[0]) > Convert.ToInt32(ftResult[1]))
            {
                return "H";
            }
            else if (Convert.ToInt32(ftResult[0]) < Convert.ToInt32(ftResult[1]))
            {
                return "A";
            }
            else
            {
                return "D";
            }
        }
        public int GetHTHG(string score)
        {
            try
            {
                string[] hthg = score.Split('-');
                return ParseValue(hthg[0].Trim());
            }
            catch
            {
                return 0;
            }
        }
        public int GetHTAG(string score)
        {
            try
            {
                string[] htag = score.Split('-');
                return ParseValue(htag[1].Trim());
            }
            catch
            {
                return 0;
            }
        }
        public int GetHTGoals(string score)
        {
            try
            {
                string[] htGoals = score.Split('-');
                return (Convert.ToInt32(htGoals[1]) + Convert.ToInt32(htGoals[0]));
            }
            catch
            {
                return 0;
            }
        }
        public string GetHTResult(string score)
        {
            string[] htResult = score.Split('-');
            if (Convert.ToInt32(htResult[0]) > Convert.ToInt32(htResult[1]))
            {
                return "H";
            }
            else if (Convert.ToInt32(htResult[0]) < Convert.ToInt32(htResult[1]))
            {
                return "A";
            }
            else
            {
                return "D";
            }
        }
        public string GetBTS(int FTHG, int FTAG)
        {
            if (FTHG > 0 && Convert.ToInt32(FTAG) > 0)
            {
                return "yes";
            }
            else
            {
                return "no";
            }
        }
        public int GetGoals(int FTGoals, int HTGoals)
        {
            return (FTGoals - HTGoals);
        }
    }
}
