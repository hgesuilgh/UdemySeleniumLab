using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace UdemySeleniumTests
{
    [TestFixture]  // Это обязательно!
    public class PageTitleTests  // Имя класса важно
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.udemy.com/");
        }

        [Test]  // Это тестовый метод
        public void TitleShouldContainUdemy()
        {
            string title = driver.Title;
            Assert.That(title, Does.Contain("Udemy").IgnoreCase);
        }

        [Test]
        public void TitleShouldNotBeEmpty()
        {
            Assert.That(driver.Title, Is.Not.Empty);
        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}
