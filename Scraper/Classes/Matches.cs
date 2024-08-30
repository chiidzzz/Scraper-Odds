using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Classes
{
    class Matches
    {
        private int match_id;
        private int season_id;
        private DateTime match_date;
        private int home_teamid;
        private int away_teamid;
        private int league_id;
        private int real_league_id;
        private string ftscore;
        private int fthg;
        private int ftag;
        private short ftgoals;
        private string ftresult;
        private string htscore;
        private short hthg;
        private short htag;
        private short htgoals;
        private string htresult;
        private short goals;
        private bool bts;
        private string kickoff_utc2;
        private short positionh;
        private short positiona;
        private bool computation;
        private string match_oddsportal_link;
        private string algo;

        public int Match_Id
        {
            get { return match_id; }
            set { match_id = value; }
        }
        public int Season_Id
        {
            get { return season_id; }
            set { season_id = value; }
        }
        public DateTime Match_Date
        {
            get { return match_date; }
            set { match_date = value; }
        }
        public int Home_Teamid
        {
            get { return home_teamid; }
            set { home_teamid = value; }
        }
        public int Away_Teamid
        {
            get { return away_teamid; }
            set { away_teamid = value; }
        }
        public int League_Id
        {
            get { return league_id; }
            set { league_id = value; }
        }
        public int Real_League_Id
        {
            get { return real_league_id; }
            set { real_league_id = value; }
        }
        public string Ftscore
        {
            get { return ftscore; }
            set { ftscore = value; }
        }
        public int Fthg
        {
            get { return fthg; }
            set { fthg = value; }
        }
        public int Ftag
        {
            get { return ftag; }
            set { ftag = value; }
        }
        public short Ftgoals
        {
            get { return ftgoals; }
            set { ftgoals = value; }
        }
        public string Ftresult
        {
            get { return ftresult; }
            set { ftresult = value; }
        }
        public string Htscore
        {
            get { return htscore; }
            set { htscore = value; }
        }
        public short Hthg
        {
            get { return hthg; }
            set { hthg = value; }
        }
        public short Htag
        {
            get { return htag; }
            set { htag = value; }
        }
        public short Htgoals
        {
            get { return htgoals; }
            set { htgoals = value; }
        }
        public string Htresult
        {
            get { return htresult; }
            set { htresult = value; }
        }
        public short Goals
        {
            get { return goals; }
            set { goals = value; }
        }
        public bool Bts
        {
            get { return bts; }
            set { bts = value; }
        }
        public string Kickoff_Utc2
        {
            get { return kickoff_utc2; }
            set { kickoff_utc2 = value; }
        }
        public short Positionh
        {
            get { return positionh; }
            set { positionh = value; }
        }
        public short Positiona
        {
            get { return positiona; }
            set { positiona = value; }
        }
        public bool Computation
        {
            get { return computation; }
            set { computation = value; }
        }
        public string Match_Oddsportal_Link
        {
            get { return match_oddsportal_link; }
            set { match_oddsportal_link = value; }
        }
        public string Algo
        {
            get { return algo; }
            set { algo = value; }
        }
    }
}
