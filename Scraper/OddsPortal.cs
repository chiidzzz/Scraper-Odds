using CsQuery;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Scraper.Classes;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    public class OddsPortal
    {

        private string[] browserArgs;
        private Dictionary<string, int> BookmakerTypeDic;
        ChromeDriver web;
        public OddsPortal(string[] browserArgs, Dictionary<string, int> BookmakerTypeDic)
        {
            this.browserArgs = browserArgs;
            this.BookmakerTypeDic = BookmakerTypeDic;
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            var options = new ChromeOptions();
            options.AddArguments(browserArgs);
            web = new ChromeDriver(service, options);
            web.Navigate().GoToUrl("http://www.oddsportal.com/");
            if (!CheckLogin(web))
            {
                doLogin(web);
            }
        }
        private GamePrice createGame(int matchID)
        {
            GamePrice gamePrice = new GamePrice();
            foreach (var dicbookmaker in BookmakerTypeDic)
            {
                Bookmaker bookmaker = new Bookmaker();
                bookmaker.Bookmaker_ID = dicbookmaker.Value;
                gamePrice.Bookmaker.Add(bookmaker);
            }
            return gamePrice;
        }
        private void getLinks()
        {
            //notFoundLinks = new string[Links.Count];
            //string tab = "";
            //int totalpages = 0;
            //TimeSpan t = new TimeSpan(0, 0, 0, 10, 0);
            //WebDriverWait wait = new WebDriverWait(web, t);
            //HideMyLeagues();
            //for (int i = 0; i < Links.Count; i++)
            //{
            //    if (chkResults.Checked)
            //    {
            //        tab = "results";
            //    }
            //    else
            //    {
            //        tab = "";
            //    }
            //    retry:
            //    try
            //    {
            //        getresults:
            //        totalpages = getPagesFromTab(i, tab);
            //        if (!checkLinkDate(totalpages, i, ThePages[Links[i].country + Links[i].league + Links[i].season + tab]))
            //        {
            //            if (tab != "results")
            //            {
            //                tab = "results";
            //                goto getresults;
            //            }
            //            else
            //            {
            //                Game ogame = new Game();
            //                ogame.gameLink = "Could not find this match :" + Links[i].country + "," + Links[i].league + "," + Links[i].season + "," + Links[i].ddate + "," + Links[i].home + "," + Links[i].away;
            //                myLinks.Add(ogame);
            //                notFoundLinks[i] = "Could not find this match :" + Links[i].country + "," + Links[i].league + "," + Links[i].season + "," + Links[i].ddate + "," + Links[i].home + "," + Links[i].away;
            //            }
            //        }
            //        else
            //        {
            //            tab = "";
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        if (MessageBox.Show("An error occured, please take a screenshot and send it to me. \n Press ok to retry the execution of the current task \n" + ex.Message, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            //        {
            //            goto retry;
            //        }
            //        else
            //        {
            //            return;
            //        }
            //    }
            //}

        }
        public GamePrice getPrice(string oddsLink, int matchID)
        {
            GamePrice gamePrice = createGame(matchID);
            web.Navigate().GoToUrl(oddsLink);
            if (!OddsExist(web))
            {
                web.Close();
                //throw new Exception("Odds does not exist");
            }
            try
            {
                GetPrices(web,gamePrice, "FT", "1x2");
                //web.Url= url + "/#1X2;3";
                if (ClickLinkIfPresent(web,"1st Half"))
                {
                    GetPrices(web, gamePrice, "FH", "1x2");
                }
                if (ClickLinkIfPresent(web,"DC"))
                {
                    GetPrices(web, gamePrice, "FT", "DC");
                }
                else
                {
                    if (ClickLinkIfPresent(web,"More bets"))
                    {
                        if (ClickLinkIfPresent(web,"Double Chance"))
                        {
                            GetPrices(web, gamePrice, "FT", "DC");
                        }
                    }
                }
                //web.Url = url + "/#double;2";
                //web.Url = url + "/#double;2";
                if (ClickLinkIfPresent(web,"1st Half"))
                {
                    GetPrices(web, gamePrice, "FH", "DC");
                }
                //web.Url = url + "/#double;3";
                //web.Url = url + "/#double;3";
                if (ClickLinkIfPresent(web,"DNB"))
                {
                    GetPrices(web, gamePrice, "FT", "DNB");
                }
                //web.Url = url + "/#dnb;2";
                //web.Url = url + "/#dnb;2";
                if (ClickLinkIfPresent(web,"More bets"))
                {
                    if (ClickLinkIfPresent(web,"Both Teams to Score"))
                    {
                        GetPrices(web, gamePrice, "FT", "BTS");
                    }
                }
                //web.Url = url + "/#bts;2";
                //web.Url = url + "/#bts;2";
                if (ClickLinkIfPresent(web,"O/U"))
                {
                    GetOverUnderFT(web,gamePrice);//10
                }
                //web.Url = url + "/#over-under;2";
                //web.Url = url + "/#over-under;2";
                if (ClickLinkIfPresent(web,"1st Half"))
                {
                    GetOverUnder(web, gamePrice, "FH_");//4
                }
                //web.Url = url + "/#over-under;3";
                //web.Url = url + "/#over-under;3";
                if (ClickLinkIfPresent(web,"2nd Half"))
                {
                    GetOverUnder(web,gamePrice,"SH_");//4
                }
                //web.Url = url + "/#over-under;4";
                //web.Url = url + "/#over-under;4";
                gamePrice.Match_ID = matchID;
                gamePrice.Price_Date = DateTime.Today;
                //web.Quit();
                return gamePrice;
            }
            catch(Exception ex)
            {
                web.Quit();
                throw new Exception(ex.Message + ex.StackTrace);
            }
        }
        private void GetPrices(ChromeDriver web,GamePrice gamePrice, string time, string typ)
        {
            TimeSpan t = new TimeSpan(0, 0, 0, 30, 0);
            WebDriverWait wait = new WebDriverWait(web, t);
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("odds-data-table")));
            var html = web.FindElementById("odds-data-table").GetAttribute("innerHTML");
            //web.ExecuteScript("window.stop()");
            CQ dom = html;
            var prices = dom["tr"];
            foreach (var price in prices)
            {
                int val = CheckBookmakerExistsInDic(price.Cq().Find(".name").Text());
                if (val == -1) continue;
                if (time == "FT" && typ == "1x2")
                {
                    gamePrice.Bookmaker[val].Prices.Home = ParseValue(price.Cq().Find(".right").First().Text());
                    gamePrice.Bookmaker[val].Prices.FT_Draw = ParseValue(price.Cq().Find(".right").First().Next().Text());
                    gamePrice.Bookmaker[val].Prices.Away = ParseValue(price.Cq().Find(".right").Last().Text());
                }
                else if (time == "FH" && typ == "1x2")
                {
                    gamePrice.Bookmaker[val].Prices.FH_Home = ParseValue(price.Cq().Find(".right").First().Text());
                    gamePrice.Bookmaker[val].Prices.FH_Draw = ParseValue(price.Cq().Find(".right").First().Next().Text());
                    gamePrice.Bookmaker[val].Prices.FH_Away = ParseValue(price.Cq().Find(".right").Last().Text());
                }
                else if (time == "FT" && typ == "DC")
                {
                    gamePrice.Bookmaker[val].Prices.Lay_Home = ParseValue(price.Cq().Find(".right").First().Text());
                    gamePrice.Bookmaker[val].Prices.Lay_FT_Draw = ParseValue(price.Cq().Find(".right").First().Next().Text());
                    gamePrice.Bookmaker[val].Prices.Lay_Away = ParseValue(price.Cq().Find(".right").Last().Text());
                }
                else if (time == "FH" && typ == "DC")
                {
                    gamePrice.Bookmaker[val].Prices.Lay_FH_Home = ParseValue(price.Cq().Find(".right").First().Text());
                    gamePrice.Bookmaker[val].Prices.Lay_FH_Draw = ParseValue(price.Cq().Find(".right").First().Next().Text());
                    gamePrice.Bookmaker[val].Prices.Lay_FH_Away = ParseValue(price.Cq().Find(".right").Last().Text());
                }
                else if (time == "FT" && typ == "DNB")
                {
                    gamePrice.Bookmaker[val].Prices.DNB = ParseValue(price.Cq().Find(".right").First().Text());
                    gamePrice.Bookmaker[val].Prices.DNB_Away = ParseValue(price.Cq().Find(".right").Last().Text());
                }
                else if (time == "FT" && typ == "BTS")
                {
                    gamePrice.Bookmaker[val].Prices.BTS = ParseValue(price.Cq().Find(".right").First().Text());
                }
            }
        }
        private void GetOverUnderFT(ChromeDriver web, GamePrice gamePrice)
        {
            TimeSpan t = new TimeSpan(0, 0, 0, 30, 0);
            WebDriverWait wait = new WebDriverWait(web, t);
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("odds-data-table")));
            bool LinkPresent;
                for (int i = 0; i < 5; i++)
                {
                    LinkPresent = ClickLinkIfPresent(web,"Over/Under +" + (i) + ".5");
                    var html = web.FindElementById("odds-data-table").GetAttribute("innerHTML");
                    web.ExecuteScript("window.stop()");
                    CQ dom = html;
                    var prices = dom["tr"];
                    foreach (var price in prices)
                    {
                        int val = CheckBookmakerExistsInDic(price.Cq().Find(".name").Text());
                        if (val == -1) continue;
                        if (LinkPresent)
                        {
                            //if (val == 0)
                            //{
                            //    gamePrice.Bookmaker[val].Prices.GetType().GetProperty("OUFT" + i + "5O").SetValue(gamePrice.Bookmaker[val].Prices, ParseValue(price.Cq().Find(".right").First().Next().Text()), null);
                            //    gamePrice.Bookmaker[val].Prices.GetType().GetProperty("OUFT" + i + "5U").SetValue(gamePrice.Bookmaker[val].Prices, ParseValue(price.Cq().Find(".right").Last().Text()), null);
                            //}
                            //else
                            //{
                                gamePrice.Bookmaker[val].Prices.GetType().GetProperty("O_" + i + "5").SetValue(gamePrice.Bookmaker[val].Prices, ParseValue(price.Cq().Find(".right").First().Text()), null);
                                gamePrice.Bookmaker[val].Prices.GetType().GetProperty("U_" + i + "5").SetValue(gamePrice.Bookmaker[val].Prices, ParseValue(price.Cq().Find(".right").Last().Text()), null);
                            //}
                        }
                        else
                        {
                            gamePrice.Bookmaker[val].Prices.GetType().GetProperty("O_" + i + "5").SetValue(gamePrice.Bookmaker[val].Prices, 0, null);
                            gamePrice.Bookmaker[val].Prices.GetType().GetProperty("U_" + i + "5").SetValue(gamePrice.Bookmaker[val].Prices, 0, null);
                        }
                    }
                    ClickLinkIfPresent(web,"Over/Under +" + (i) + ".5");
                }
        }
        private void GetOverUnder(ChromeDriver web,GamePrice gamePrice, string time)
        {
            TimeSpan t = new TimeSpan(0, 0, 0, 30, 0);
            WebDriverWait wait = new WebDriverWait(web, t);
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("odds-data-table")));
            bool LinkPresent;
            for (int i = 0; i < 2; i++)
            {
                LinkPresent = ClickLinkIfPresent(web,"Over/Under +" + (i) + ".5");
                var html = web.FindElementById("odds-data-table").GetAttribute("innerHTML");
                web.ExecuteScript("window.stop()");
                CQ dom = html;
                var prices = dom["tr"];
                foreach (var price in prices)
                {
                    int val = CheckBookmakerExistsInDic(price.Cq().Find(".name").Text());
                    if (val == -1) continue;
                    if (LinkPresent)
                    {
                        //if (val == 0)
                        //{
                        //    gamePrice.Bookmaker[val].Prices.GetType().GetProperty("OU" + time + i + "5O").SetValue(gamePrice.Bookmaker[val].Prices, ParseValue(price.Cq().Find(".right").First().Next().Text()), null);
                        //    gamePrice.Bookmaker[val].Prices.GetType().GetProperty("OU" + time + i + "5U").SetValue(gamePrice.Bookmaker[val].Prices, ParseValue(price.Cq().Find(".right").Last().Text()), null);
                        //}
                        //else
                        //{
                            gamePrice.Bookmaker[val].Prices.GetType().GetProperty(time+"O_" + i + "5").SetValue(gamePrice.Bookmaker[val].Prices, ParseValue(price.Cq().Find(".right").First().Text()), null);
                            gamePrice.Bookmaker[val].Prices.GetType().GetProperty(time+"U_" + i + "5").SetValue(gamePrice.Bookmaker[val].Prices, ParseValue(price.Cq().Find(".right").Last().Text()), null);
                        //}
                    }
                    else
                    {
                        gamePrice.Bookmaker[val].Prices.GetType().GetProperty(time+"O_" + i + "5").SetValue(gamePrice.Bookmaker[val].Prices, 0, null);
                        gamePrice.Bookmaker[val].Prices.GetType().GetProperty(time+"U_" + i + "5").SetValue(gamePrice.Bookmaker[val].Prices, 0, null);
                    }
                }
                ClickLinkIfPresent(web,"Over/Under +" + (i) + ".5");
            }
        }
        private double ParseValue(string txt)
        {
            double val;
            if (double.TryParse(txt, out val) && (txt != ""))
            {
                return val;
            }
            else
            {
                return 0;
            }
        }
        private int CheckBookmakerExistsInDic(string txt)
        {
            int val;
            if (BookmakerTypeDic.TryGetValue(txt, out val))
            {
                return val-1;
            }
            return -1;
        }
        protected bool ClickLinkIfPresent(ChromeDriver web, string ele)
        {
            try
            {
                IWebElement lnk;
                lnk = web.FindElementByLinkText(ele);
                lnk.Click();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool OddsExist(ChromeDriver web)
        {
            try
            {
                if (web.FindElementByClassName("message-info").Displayed)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }
        }
        private void doLogin(ChromeDriver web)
        {
            web.Navigate().GoToUrl("http://www.oddsportal.com/login/");
            try
            {
                IWebElement Username = web.FindElementById("login-username1");
                IWebElement Password = web.FindElementById("login-password1");
                if (Username.GetAttribute("value") == "")
                {
                    Username.Click();
                    Username.SendKeys("chiidzzz");
                }
                Password.Click();
                Password.SendKeys("chadichadi12");
                IList<IWebElement> btnlogin = web.FindElementsByName("login-submit");
                for (int i = 0; i < btnlogin.Count; i++)
                {
                    if (btnlogin[i].GetAttribute("type") == "submit")
                    {
                        btnlogin[i].Click();
                    }
                }
                CheckLogin(web);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message + ex.StackTrace);
            }
        }
        private bool CheckLogin(ChromeDriver web)
        {
            try
            {
                IWebElement username = web.FindElementByLinkText("chiidzzz");
                if (username.Displayed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public void QuitWeb()
        {
            web.Quit();
        }
    }
}
