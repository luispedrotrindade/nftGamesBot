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
    public class UntitledTestCase
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
            driver.FindElement(By.XPath("//img[contains(@src,'https://d36mxiodymuqjm.cloudfront.net/website/nav/icon_nav_cards_active@2x.png')]")).Click();

            Thread.Sleep(3000);

            driver.FindElement(By.Id("filter-owned")).Click();
            
            Thread.Sleep(3000);

            new SelectElement(driver.FindElement(By.Id("filter-owned"))).SelectByText("OWNED");
            
            Thread.Sleep(3000);

            driver.FindElement(By.XPath("//img[contains(@src,'https://d36mxiodymuqjm.cloudfront.net/cards_untamed/Magma%20Troll.png')]")).Click();

            Thread.Sleep(3000);

            driver.FindElement(By.XPath("//div[@id='tab_container']/div/div[2]/div[2]/div/div/table/tbody/tr/td/div")).Click();

            Thread.Sleep(3000);

            driver.FindElement(By.Id("btn_market")).Click();
            
            Thread.Sleep(3000);

            driver.FindElement(By.Id("tab_rent")).Click();

            Thread.Sleep(3000);

            driver.FindElement(By.Id("rent_price")).Click();

            Thread.Sleep(3000);

            acceptNextAlert = true;
            driver.FindElement(By.Id("rent_price")).Clear();

            Thread.Sleep(3000);

            driver.FindElement(By.Id("rent_price")).SendKeys("0.10");

            Thread.Sleep(3000);

            driver.FindElement(By.Id("btn_rent")).Click();

            Thread.Sleep(3000);

            Assert.AreEqual("Please confirm that you wish to list this card for rent at a rate of 0.1 DEC per day.", CloseAlertAndGetItsText());

            Thread.Sleep(3000);

            driver.FindElement(By.XPath("//img[contains(@src,'https://d36mxiodymuqjm.cloudfront.net/website/nav/icon_nav_cards_active@2x.png')]")).Click();

            Thread.Sleep(3000);

            driver.FindElement(By.Id("filter-owned")).Click();

            Thread.Sleep(3000);

            new SelectElement(driver.FindElement(By.Id("filter-owned"))).SelectByText("OWNED");

            Thread.Sleep(3000);

            acceptNextAlert = true;
            driver.FindElement(By.XPath("//img[contains(@src,'https://d36mxiodymuqjm.cloudfront.net/cards_untamed/Magma%20Troll.png')]")).Click();

            Thread.Sleep(3000);

            driver.FindElement(By.XPath("//img[contains(@src,'https://d36mxiodymuqjm.cloudfront.net/website/ui_elements/card_details/small/icon_market_yellow.png')]")).Click();

            Thread.Sleep(3000);

            Assert.IsTrue(Regex.IsMatch(CloseAlertAndGetItsText(), "^Are you sure you want to cancel this active rental and remove the card from the rental market[\\s\\S]$"));

            Thread.Sleep(3000);

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