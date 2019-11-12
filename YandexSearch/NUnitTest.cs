using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using HtmlAgilityPack;

namespace YandexSearch
{
    class NUnitTest
    {
        IWebDriver driver;
        string model = "Lenovo";

        [SetUp]
        public void startBrowser()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
            driver.Url = "https://market.yandex.ru/";
            driver.FindElement(By.LinkText("Компьютерная техника")).Click();
            driver.FindElement(By.LinkText("Ноутбуки")).Click();
            string path_to_model = String.Format("//a/label/div[contains(.//span, '{0}')]",
                           model);
            driver.FindElement(By.XPath(path_to_model)).Click();
            driver.FindElement(By.Id("glpricefrom")).SendKeys("25000");
            driver.FindElement(By.Id("glpriceto")).SendKeys("30000");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector("div._1RqJzaMsJQ._1yevJbN4UM._3zMtk0OXjW")));
        }

        [Test]
        public void test()
        {
            string html_page = driver.PageSource;
            string regex = @"(<.+?>|&nbsp;)";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html_page);

            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[@class='price']"))
            {
                string value = node.InnerText.Replace("от", "").Replace("₽", "").Replace(" ", "");
                var new_value = System.Text.RegularExpressions.Regex.Replace(value, regex, "").Trim();
                int price = Convert.ToInt32(new_value);
                bool isT = (price >= 25000 && price <= 30000) ? true : false;
                Assert.IsTrue(isT);
            }
        }

        [TearDown]
        public void closeBrowser()
        {
            driver.Close();
        }
    }
}
