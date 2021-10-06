using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace nftGamesBot
{

    [TestFixture]
    public class MarketBuySearch
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;

        [SetUp]
        public void SetupTest()
        {
            ChromeOptions opt = new ChromeOptions();
            opt.AddExtensions(@"C:\Users\lp_tp\source\repos\nftGamesBot\nftGamesBot\10.1.1_0.crx");

            driver = new ChromeDriver(@"C:\Users\lp_tp\source\repos\nftGamesBot\nftGamesBot\", opt);
            baseURL = "https://www.google.com/";
            verificationErrors = new StringBuilder();
        }

        [Test]
        public void Login()
        {
            driver.Navigate().GoToUrl("https://splinterlands.com/");
            driver.FindElement(By.XPath("//li[@id='log_in_button']/button")).Click();
            Thread.Sleep(3000);

            driver.FindElement(By.Id("email")).Clear();
            Thread.Sleep(3000);

            driver.FindElement(By.Id("email")).SendKeys("pedroka");
            Thread.Sleep(3000);

            driver.FindElement(By.Id("password")).Clear();
            Thread.Sleep(3000);

            driver.FindElement(By.Id("password")).SendKeys("abHQEYjPWb0Qnk0kwU1PFpc24yNyzXdM");
            Thread.Sleep(3000);

            driver.FindElement(By.Id("email")).Click();
            Thread.Sleep(3000);

            // ERROR: Caught exception [ERROR: Unsupported command [doubleClick | id=email | ]]
            driver.FindElement(By.XPath("//div[@id='login_dialog_v2']/div/div/div[2]/div/div[2]/form[2]/div[3]/div/button")).Click();
            Thread.Sleep(3000);

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
            driver.Navigate().GoToUrl("https://splinterlands.com/?p=market&tab=cards");
            Thread.Sleep(3000);
            driver.FindElement(By.XPath("//img[contains(@src,'https://d36mxiodymuqjm.cloudfront.net/website/nav/icon_nav_market_active@2x.png')]")).Click();
            Thread.Sleep(3000);
            driver.FindElement(By.XPath("//form[@id='filterForm']/div[2]/div/fieldset/div/div[2]/label")).Click();
            Thread.Sleep(3000);
            driver.FindElement(By.XPath("//form[@id='filterForm']/div[3]/div/fieldset/div/div[3]/label")).Click();
            Thread.Sleep(3000);
            driver.FindElement(By.XPath("//form[@id='filterForm']/div[3]/div/fieldset/div/div[4]/label")).Click();
            Thread.Sleep(3000);


            while (true)
            {
                driver.FindElement(By.Id("market_tab_rentals")).Click();
                Thread.Sleep(5000);
                driver.FindElement(By.Id("market_tab_cards")).Click();
                Thread.Sleep(5000);
            }
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