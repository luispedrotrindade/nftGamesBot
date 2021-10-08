using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using nftGamesBot.Models;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace nftGamesBot
{
    [TestFixture]
    public class PVU
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private bool acceptNextAlert = true;
        public Dictionary<string, double> Conditions { get; set; }
        public const double fatorCorrecao = 1;
        public const string mainUrl = "https://marketplace.plantvsundead.com/offering/bundle#/marketplace/plant?elements=water,metal,ice,fire,wind,electro&sort=latest&rarities=0";
        public Dictionary<string, List<Planta>> plantasVendidas { get; set; }

        [SetUp]
        public void SetupTest()

        {
            plantasVendidas = new Dictionary<string, List<Planta>>();
            Conditions = new Dictionary<string, double>();

            //////Light
            ////Conditions.Add("21_1.png", 12);
            ////Conditions.Add("19_1.png", 12);
            ////Conditions.Add("20_1.png", 12);


            ////Water
            //Conditions.Add("36_1.png", 15.5);
            //Conditions.Add("4_1.png", 15.5);
            //Conditions.Add("5_1.png", 15.5);
            //Conditions.Add("38_1.png", 15.5);
            //Conditions.Add("39_1.png", 15.5);

            ////Fire
            //Conditions.Add("1_1.png", 23);
            //Conditions.Add("17_1.png", 23);
            //Conditions.Add("0_1.png", 23);
            //Conditions.Add("30_1.png", 23);


            ////Wind
            //Conditions.Add("37_1.png", 19);
            //Conditions.Add("16_1.png", 19);
            //Conditions.Add("10_1.png", 22);
            //Conditions.Add("9_1.png", 22);


            //////Dark
            ////Conditions.Add("14_1.png", 15);
            ////Conditions.Add("31_1.png", 15);
            ////Conditions.Add("33_1.png", 15);
            ////Conditions.Add("35_1.png", 15);


            //////Parasit
            ////Conditions.Add("11_1.png", 15);
            ////Conditions.Add("12_1.png", 15);
            ////Conditions.Add("22_1.png", 15);
            ////Conditions.Add("23_1.png", 15);
            ////Conditions.Add("24_1.png", 15);


            ////Ice
            //Conditions.Add("29_1.png", 18);
            //Conditions.Add("2_1.png", 19);
            //Conditions.Add("6_1.png", 20);


            ////Metal
            //Conditions.Add("27_1.png", 14);
            //Conditions.Add("26_1.png", 14);
            //Conditions.Add("28_1.png", 14);
            //Conditions.Add("25_1.png", 14);

            ////Eletric
            //Conditions.Add("3_1.png", 27.5);
            //Conditions.Add("8_1.png", 27.5);
            //Conditions.Add("32_1.png", 27.5);
            //Conditions.Add("34_1.png", 27.5);

            ChromeOptions opt = new ChromeOptions();
            opt.DebuggerAddress = "127.0.0.1:9222";
            //opt.AddExtensions(@"C:\Users\lp_tp\source\repos\nftGamesBot\nftGamesBot\10.1.1_0.crx");

            driver = new ChromeDriver(@$"{Environment.CurrentDirectory}", opt);
            verificationErrors = new StringBuilder();

            GetCotacao();

            TheUntitledTestCaseTest();
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }

        [Test]
        public void TheUntitledTestCaseTest()
        {
            var url = "";

            List<string> listIds = new List<string>();
            int i = 0;
            while (true)
            {
                try
                {
                    driver.Navigate().GoToUrl(mainUrl);
                    Thread.Sleep(1500);

                    var uls = driver.FindElements(By.TagName("ul"));
                    var lis = uls[3].FindElements(By.CssSelector(".tw-h-80.tw-rounded-lg.tw-bg-black.tw-bg-opacity-10.tw-border.tw-border-gray-900"));


                    foreach (var li in lis)
                    {
                        var a = li.FindElement(By.TagName("a"));
                        var id = li.FindElement(By.ClassName("id")).Text;
                        var div = a.FindElement(By.CssSelector(".tw-flex.tw-justify-center"));
                        var image = div.FindElement(By.CssSelector("img"));
                        var rodape = a.FindElement(By.CssSelector(".tw-flex.tw-justify-between.tw-items-end"));
                        var twml4 = rodape.FindElement(By.CssSelector(".tw-ml-4"));
                        var divPreco = twml4.FindElement(By.TagName("div"));
                        var preco = Convert.ToDouble(divPreco.FindElement(By.TagName("p")).Text.Replace(".", ","));

                        if (listIds.Count >= 15)
                        {
                            listIds.RemoveRange(0, 5);
                        }

                        if (GetCondition(image.GetAttribute("src"), preco) && !listIds.Contains(id))
                        {
                            listIds.Add(id);

                            url = a.GetAttribute("href");

                            driver.Navigate().GoToUrl(url);

                            Thread.Sleep(1500);

                            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                            string title = (string)js.ExecuteScript("document.getElementsByClassName('btn__sell')[0].click();");

                            Thread.Sleep(750);

                            driver.Navigate().GoToUrl(mainUrl);
                        }
                    }

                    if (i++ >= 100)
                    {
                        i = 0;
                        GetCotacao();
                    }
                }
                catch (Exception ex) { }
            }
        }

        public void GetCotacao()
        {
            plantasVendidas.Clear();
            Conditions.Clear();

            string url = "https://marketplace.plantvsundead.com/offering/bundle#/";
            try
            {
                driver.Navigate().GoToUrl(url);
                Thread.Sleep(1500);

                for (int i = 0; i < 19; i++)
                {
                    var uls = driver.FindElements(By.TagName("ul"));
                    var lis = uls[4].FindElements(By.TagName("li"));

                    foreach (var li in lis)
                    {
                        var a = li.FindElement(By.TagName("a"));
                        var divs = a.FindElements(By.TagName("div"));
                        var divImagem = divs[0];
                        var image = divImagem.FindElement(By.CssSelector("img"));
                        var id = image.GetAttribute("src");
                        id = id.Substring(id.IndexOf("/plant/") + 7, (id.Length - 7) - id.IndexOf("/plant/")).Replace(".png", "").Replace(".jpg", "");
                        var divPreco = divs[2];
                        var p = divPreco.FindElement(By.TagName("p"));
                        var preco = Convert.ToDouble(p.GetAttribute("innerHTML").Replace(".", ","));

                        if (!plantasVendidas.Any(x => x.Key == id))
                            plantasVendidas.Add(id, new List<Planta> { new Planta { Id = id, Preco = preco } });
                        else
                            plantasVendidas.FirstOrDefault(x => x.Key == id).Value.Add(new Planta { Id = id, Preco = preco });
                    }

                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("document.getElementsByClassName('box tw-cursor-pointer')[1].click();");
                    Thread.Sleep(3000);
                }
                foreach (var plantaVendida in plantasVendidas)
                {
                    Conditions.Add(plantaVendida.Key, plantaVendida.Value.Average(x => x.Preco));
                }
            }
            catch (Exception ex) { }
        }
        private bool GetCondition(string src, double price)
        {
            return Conditions.Any(x => src.IndexOf($"plant/{x.Key}") > -1 && price <= ((x.Value * fatorCorrecao) - x.Value * 0.07));
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }
    }
}
