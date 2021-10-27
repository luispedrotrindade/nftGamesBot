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
    public class AxieInfinity
    {
        private IWebDriver driver;

        private string mainUrl = "https://marketplace.axieinfinity.com";
        private int tempoEspera = 500;
        private double precoMinimo = 0.028;

        [SetUp]
        public void Setup()
        {

            ChromeOptions opt = new ChromeOptions();
            opt.DebuggerAddress = "127.0.0.1:9222";

            driver = new ChromeDriver(@$"{Environment.CurrentDirectory}", opt);

            BuscarAxies();
        }

        [Test]
        public void BuscarAxies()
        {

            while (true)
            {
                try
                {
                    var url = "";
                    driver.Navigate().GoToUrl(mainUrl);
                    Thread.Sleep(tempoEspera);



                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                    var h5List = driver.FindElements(By.TagName("h5"));

                    double price = 0;

                    var i = 0;

                    foreach (var h5 in h5List)
                    {
                        var span = h5.FindElement(By.TagName("span"));
                        price = Convert.ToDouble(span.GetAttribute("innerHTML").Replace(".", ","));

                        Console.WriteLine(price);

                        if (price <= precoMinimo)
                        {
                            h5.Click();
                            Thread.Sleep(tempoEspera * 3);
                            var buttons = driver.FindElements(By.TagName("button"));
                            buttons[0].Click();
                            Thread.Sleep(tempoEspera);
                        }

                        i++;
                        if (i >= 9)
                            break;
                    }

                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
