using CsQuery;
using Scraper.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Scraper
{
    public class XScores
    {
        private RemoteWebDriver web;
        Dictionary<string, string> myTeamDic;
        public enum MyWeb
        {
            Chrome,
            Phantom
        }
        /// <summary>
        /// Constructor to create a new XScore scraper.
        /// </summary>
        /// <param name="web">This is a string value to choose a browser, either Chrome or Phantom</param>
        /// <param name="broswerArgs">Browser arguments, example: headless, user data profile, load images, ssl protocol...</param>
        /// <param name="myLeagueDic">(string, string) dictionary to provide (xscores coutry name - xscores league name) and the league id</param>
        /// <param name="myTeamDic">(string, string) dictionary to provide (xscores team name) and team id</param>
        public XScores(MyWeb web,string[] browserArgs)
        {
            if (web == MyWeb.Chrome)
            {
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                var options = new ChromeOptions();
                options.AddArguments(browserArgs);
                //options.AddArgument("--window-position=-32000,-32000");
                //options.AddArgument("user-data-dir=" + chromeProfilePath + "\\ChromeDP");
                //options.AddArgument("--headless");
                //options.AddArgument("--disable-gpu");
                this.web = new ChromeDriver(service,options);
            }
            else
            {
                //var driverService = PhantomJSDriverService.CreateDefaultService();
                //driverService.AddArguments(browserArgs);
                ////driverService.HideCommandPromptWindow = true;
                ////driverService.LoadImages = false;
                ////driverService.ProxyType = "none";
                ////driverService.SslProtocol = "tlsv1";
                //var options = new PhantomJSOptions();
                //options.AddAdditionalCapability("phantomjs.page.settings.userAgent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:54.0) Gecko/20100101 Firefox/54.0");
                //this.web = new PhantomJSDriver(driverService, options);
            }
        }
        
        public List<Fixtures> getFixtures(string dt, string url, Dictionary<string, string> myLeagueDic, Dictionary<string, string> myTeamDic)
        {
            List<Fixtures> Fixtures = new List<Fixtures>();
            this.myTeamDic = myTeamDic;
            FixdateAndNavigate(dt,url);
            TimeSpan t = new TimeSpan(0, 0, 0, 30, 0);
            WebDriverWait wait = new WebDriverWait(web, t);
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tableWrapper")));
            var html = web.FindElementById("tableWrapper").GetAttribute("innerHTML");
            CQ dom = html;
            var match = dom["div.match_line"];

            foreach (var title in match)
            {
                try
                {
                    string league;
                    if (myLeagueDic.TryGetValue((title.GetAttribute("data-country-name").ToLower() + " - " + title.GetAttribute("data-league-name").ToLower()), out league) && title.GetAttribute("data-game-status")=="Sched")
                    {
                        Fixtures fixture = new Fixtures();
                        fixture.league = league;
                        fixture.Home = CheckAndChangeTeamName(title.GetAttribute("data-home-team").ToLower().Trim());
                        fixture.Away = CheckAndChangeTeamName(title.GetAttribute("data-away-team").ToLower().Trim());
                        fixture.KickOff_UTC2 = title.Cq().Find(".score_ko").Text();
                        fixture.positionH = title.Cq().Find(".score_home").Find(".lp").Text().Trim();
                        fixture.positionA = title.Cq().Find(".score_away").Find(".lp").Text().Trim();
                        fixture.Date = DateTime.ParseExact(dt,"dd/MM/yyyy",CultureInfo.InvariantCulture);
                        fixture.versus = "VS";
                        Fixtures.Add(fixture);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("The error happened on getResults: \n date:" + dt + " \n League Name - Country Name:" + title.GetAttribute("data-country-name") + " - " + title.GetAttribute("data-league-name")
                        + "\n Home Team:" + title.GetAttribute("data-home-team") + "\n Away Team:" + title.GetAttribute("data-away-team") + "\n" + ex.Message);
                }
            }
            Quit();
            return Fixtures;
        }
        public List<Results> getResults(string dt, string url, Dictionary<string, string> myLeagueDic, Dictionary<string, string> myTeamDic, Dictionary<string,string> myYearDic)
        {
            List<Results> Results = new List<Results>();
            this.myTeamDic = myTeamDic;
            FixdateAndNavigate(dt,url);
            TimeSpan t = new TimeSpan(0, 0, 0, 30, 0);
            WebDriverWait wait = new WebDriverWait(web, t);
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tableWrapper")));
            var html = web.FindElementById("tableWrapper").GetAttribute("innerHTML");
            CQ dom = html;
            var match = dom["div.match_line"];
            foreach (var title in match)
            {
                try
                {
                    string league;
                    if (myLeagueDic.TryGetValue((title.GetAttribute("data-country-name").ToLower() + " - " + title.GetAttribute("data-league-name").ToLower()), out league) && title.GetAttribute("data-game-status") == "Fin")
                    {
                        Results result = new Results();
                        result.League = league;
                        result.Home_Team = CheckAndChangeTeamName(title.GetAttribute("data-home-team").ToLower().Trim());
                        result.Date = DateTime.ParseExact(dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        result.Away_Team = CheckAndChangeTeamName(title.GetAttribute("data-away-team").ToLower().Trim());
                        result.ScoreFT = title.Cq().Find(".scoreh_ft").Text() + " - " + title.Cq().Find(".scorea_ft").Text();
                        result.ScoreHT = title.Cq().Find(".scoreh_ht").Text() + " - " + title.Cq().Find(".scorea_ht").Text();
                        result.Year = myYearDic[title.GetAttribute("data-country-name").ToLower() + " - " + title.GetAttribute("data-league-name").ToLower()];
                        result.FTHG = result.GetFTHG(result.ScoreFT);
                        result.FTAG = result.GetFTAG(result.ScoreFT);
                        result.FTGoals = result.GetFTGoals(result.ScoreFT);
                        result.FTResult = result.GetFTResult(result.ScoreFT);
                        result.HTHG = result.GetHTHG(result.ScoreHT);
                        result.HTAG = result.GetHTAG(result.ScoreHT);
                        result.HTResult = result.GetHTResult(result.ScoreHT);
                        result.HTGoals = result.GetHTGoals(result.ScoreHT);
                        result.Goals = result.GetGoals(result.FTGoals, result.HTGoals);
                        result.BTS = result.GetBTS(result.FTHG, result.FTAG);
                        Results.Add(result);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("The error happened on getResults, line number: " + ex.StackTrace + "\n date:" + dt + " \n League Name - Country Name:" + title.GetAttribute("data-country-name") + " - " + title.GetAttribute("data-league-name")
                        + "\n Home Team:" + title.GetAttribute("data-home-team") + "\n Away Team:" + title.GetAttribute("data-away-team") + "\n" + ex.Message);
                }

            }
            Quit();
            return Results;
        }
        //public string getFixtures(string dt,string url)
        //{
        //    List<Fixtures> Fixtures = new List<Fixtures>();
        //    FixdateAndNavigate(dt,url);
        //    var html = web.FindElementById("tableWrapper").GetAttribute("innerHTML");
        //    web.ExecuteScript("window.stop()");
        //    CQ dom = html;
        //    var match = dom["div.match_line"];
            
        //    foreach (var title in match)
        //    {
        //        try
        //        {
        //            string league;
        //            if (myLeagueDic.TryGetValue((title.GetAttribute("data-country-name") + " - " + title.GetAttribute("data-league-name")), out league))
        //            {
        //                Fixtures fixture = new Fixtures();
        //                fixture.league = league;
        //                fixture.Home= CheckAndChangeTeamName(title.GetAttribute("data-home-team"));
        //                fixture.Away = CheckAndChangeTeamName(title.GetAttribute("data-away-team"));
        //                fixture.KickOff_UTC2 = title.Cq().Find(".score_ko").Text();
        //                fixture.positionH = title.Cq().Find(".score_home").Find(".lp").Text();
        //                fixture.positionA = title.Cq().Find(".score_away").Find(".lp").Text();
        //                fixture.Date = DateTime.ParseExact(dt, "dd/MM/yyyy", CultureInfo.InvariantCulture); ;
        //                Fixtures.Add(fixture);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("The error happened on getResults: \n date:" + dt + " \n League Name - Country Name:" + title.GetAttribute("data-country-name") + " - " + title.GetAttribute("data-league-name")
        //                + "\n Home Team:" + title.GetAttribute("data-home-team") + "\n Away Team:" + title.GetAttribute("data-away-team") + "\n" + ex.Message);
        //        }
        //    }
        //    return  JsonConvert.SerializeObject(Fixtures);
        //}
        private void FixdateAndNavigate(string dt,string url)
        {
            string date;          
            date = dt.Substring(0, 2) + "-" + dt.Substring(3, 2);
            web.Navigate().GoToUrl(url + date);
            if (IsAlertPresent()) BypassAlert();
            try
            {
                IWebElement btn = web.FindElementByClassName("sortBtn");
                if (btn.Text == "By League")
                {
                    btn.Click();
                }
            }
            catch
            {
                
            }
        }
        private string CheckAndChangeTeamName(string TeamName)
        {
            string team;
            int teamid;
            if (myTeamDic.TryGetValue(TeamName, out team))
            {
                if (int.TryParse(team, out teamid))
                {
                    return team;
                }
                else
                {
                    return team.ToLower().Trim();
                }
            }
            else
            {
                return TeamName;
            }
        }
        private bool IsAlertPresent()
        {
            try
            {
                web.SwitchTo().Alert();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void BypassAlert()
        {
            try
            {
                if (IsAlertPresent())
                    web.SwitchTo().Alert().Accept();

            }
            catch
            {
                ///Log your errors however you must.
            }
        }

        private void Quit()
        {
            this.web.Quit();
            this.web = null;
        }
    }
}
