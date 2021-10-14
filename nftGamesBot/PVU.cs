﻿using System;
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
        private double percentualVenda = 1.05;
        private double percentualAnomalia = 1.3;
        public const double fatorCorrecao = 0.85;
        public const string mainUrl = "https://marketplace.plantvsundead.com/offering/bundle#/marketplace/plant?sort=latest";
        public Dictionary<string, List<Planta>> PlantasVendidas { get; set; }
        List<Planta> MinhasPlantas { get; set; }
        public Dictionary<string, double> Conditions { get; set; }

        [SetUp]
        public void SetupTest()
        {
            PlantasVendidas = new Dictionary<string, List<Planta>>();
            MinhasPlantas = new List<Planta>();
            Conditions = new Dictionary<string, double>();


            ChromeOptions opt = new ChromeOptions();
            opt.DebuggerAddress = "127.0.0.1:9222";
            //opt.AddExtensions(@"C:\Users\lp_tp\source\repos\nftGamesBot\nftGamesBot\10.1.1_0.crx");

            driver = new ChromeDriver(@$"{Environment.CurrentDirectory}", opt);
            verificationErrors = new StringBuilder();

            GetMinhasPlantas();

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                    Thread.Sleep(2000);

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

                        if (MinhasPlantas.Count >= 15)
                        {
                            MinhasPlantas.RemoveRange(0, 5);
                        }

                        if (GetCondition(image.GetAttribute("src"), preco) && !listIds.Contains(id))
                        {
                            listIds.Add(id);

                            url = a.GetAttribute("href");

                            driver.Navigate().GoToUrl(url);

                            Thread.Sleep(3000);

                            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                            string title = (string)js.ExecuteScript("document.getElementsByClassName('btn__sell')[0].click();");
                            Thread.Sleep(2000);
                            MinhasPlantas.Add(new Planta { Id = id, Preco = preco });
                            driver.Navigate().GoToUrl(mainUrl);
                        }
                    }

                    if (i++ >= 100)
                    {
                        i = 0;
                        GetMinhasPlantas();
                        GetCotacao();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void GetCotacao()
        {
            PlantasVendidas.Clear();
            Conditions.Clear();

            string url = "https://marketplace.plantvsundead.com/offering/bundle#/";

            driver.Navigate().GoToUrl(url);
            while (Conditions.Count <= 0)
            {
                try
                {
                    Thread.Sleep(1000);

                    for (int i = 0; i < 49; i++)
                    {
                        var uls = driver.FindElements(By.TagName("ul"));
                        var lis = uls[4].FindElements(By.TagName("li"));

                        foreach (var li in lis)
                        {
                            var a = li.FindElement(By.TagName("a"));
                            var divs = a.FindElements(By.TagName("div"));
                            var buyer = a.FindElement(By.ClassName("username")).GetAttribute("innerHTML");
                            var divImagem = divs[0];
                            var image = divImagem.FindElement(By.CssSelector("img"));
                            var id = image.GetAttribute("src");
                            id = id.Substring(id.IndexOf("/plant/") + 7, (id.Length - 7) - id.IndexOf("/plant/")).Replace(".png", "").Replace(".jpg", "");
                            var divPreco = divs[2];
                            var p = divPreco.FindElement(By.TagName("p"));
                            var preco = Convert.ToDouble(p.GetAttribute("innerHTML").Replace(".", ","));

                            if (!PlantasVendidas.Any(x => x.Key == id))
                                PlantasVendidas.Add(id, new List<Planta> { new Planta { Id = id, Preco = preco, Buyer = buyer } });
                            else
                            {
                                var planta = PlantasVendidas.FirstOrDefault(x => x.Key == id);
                                if (!planta.Value.Any(x => x.Buyer == buyer))
                                {
                                    var result = planta.Value.Average(x => x.Preco) > preco * percentualAnomalia;
                                    while (result)
                                    {
                                        planta.Value.RemoveAll(x => x.Preco == planta.Value.Max(x => x.Preco));
                                        result = planta.Value.Count > 0 ? planta.Value.Average(x => x.Preco) > preco * percentualAnomalia : false;
                                    }
                                    if (planta.Value.Count > 0)
                                    {
                                        if (preco < (planta.Value.Average(x => x.Preco) * 1.3))
                                            planta.Value.Add(new Planta { Id = id, Preco = preco, Buyer = buyer });
                                    }
                                    else
                                        planta.Value.Add(new Planta { Id = id, Preco = preco, Buyer = buyer });
                                }
                            }
                        }

                        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                        js.ExecuteScript("document.getElementsByClassName('box tw-cursor-pointer')[1].click();");
                        Thread.Sleep(1000);
                    }
                    foreach (var plantaVendida in PlantasVendidas)
                    {
                        if (plantaVendida.Value.Count >= 5)
                            Conditions.Add(plantaVendida.Key, plantaVendida.Value.Average(x => x.Preco));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    driver.Navigate().GoToUrl(url);
                }
            }
        }

        private bool GetCondition(string src, double price)
        {
            if (Conditions.Count == 0)
                GetCotacao();

            return Conditions.Any(x => src.IndexOf($"plant/{x.Key}") > -1 && price <= ((x.Value * fatorCorrecao)));
        }

        public void GetMinhasPlantas()
        {
            var result = false;

            while (!result)
            {
                string id = null;
                var url = "https://marketplace.plantvsundead.com/farm#/profile/inventory";
                driver.Navigate().GoToUrl(url);

                Thread.Sleep(3000);


                try
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("document.getElementsByTagName('img')[20].click();");
                    js.ExecuteScript("document.getElementsByTagName('img')[21].click();");

                    Thread.Sleep(3000);

                    var div1 = driver.FindElements(By.ClassName("v-card__text"));
                    var div2 = div1[1].FindElements(By.CssSelector(".wrap.tw-relative"));
                    var div3 = div2[0].FindElements(By.CssSelector(".grid__item.tw-px-1"));


                    foreach (var div in div3)
                    {
                        id = div.FindElement(By.CssSelector(".id")).GetAttribute("innerHTML");
                        var forSale = div.FindElement(By.CssSelector(".forSale")).GetAttribute("innerHTML").ToUpper().Trim();
                        //!= null ? div.FindElement(By.CssSelector(".forSale")).GetAttribute("innerHTML").ToUpper() : null;

                        if (forSale != "FARMING" && forSale != "FOR SALE")
                        {
                            VenderPlanta(id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("Unable to locate element") > -1 && ex.Message.IndexOf(".forSale") > -1)
                    {
                        if (MinhasPlantas.Any(x => x.Id == id))
                        {
                            VenderPlanta(id);
                            Thread.Sleep(5000);
                        }
                    }

                    Console.WriteLine(ex.Message);
                }

                result = true;
            }
        }

        private void VenderPlanta(string id)
        {
            var planta = MinhasPlantas.FirstOrDefault(x => x.Id == id);
            if (planta != null)
            {
                driver.Navigate().GoToUrl($"https://marketplace.plantvsundead.com/farm#/plant/{id}");
                Thread.Sleep(2000);

                var btnSell = driver.FindElement(By.CssSelector(".btn__sell"));

                if (btnSell.GetAttribute("innerHTML").ToUpper().Trim() == "SELL NOW")
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("document.getElementsByClassName('btn__sell')[0].click();");
                    Thread.Sleep(1000);

                    driver.FindElement(By.CssSelector(".input.tw-ml-4")).Clear();
                    driver.FindElement(By.CssSelector(".input.tw-ml-4")).SendKeys(Math.Round(planta.Preco * percentualVenda, 2).ToString().Replace(".", "").Replace(",", "."));

                    js.ExecuteScript("document.getElementsByClassName('sell tw-mt-10 v-btn v-btn--is-elevated v-btn--has-bg theme--light v-size--default')[0].click();");
                    Thread.Sleep(20000);
                }
            }

            GetMinhasPlantas();
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
