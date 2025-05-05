using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace BookshelfTests
{
    [TestFixture]
    public class AddBookTest
    {
        private IWebDriver? driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
        }

        [TearDown]
        public void Teardown()
        {
            driver!.Quit();
        }

        [Test]
        public void AddBookTestMethod()
        {
            driver!.Navigate().GoToUrl("http://localhost:5173");

            driver.FindElement(By.LinkText("Add Book")).Click();
            driver.FindElement(By.Name("Title")).SendKeys("The Hobbit");
            driver.FindElement(By.Name("Author")).SendKeys("J.R.R. Tolkien");
            driver.FindElement(By.Name("Tag")).SendKeys("Fantasy");
            driver.FindElement(By.Name("Status")).SendKeys("Reading");
            driver.FindElement(By.Name("Rating")).SendKeys("5");
            driver.FindElement(By.CssSelector("form button[type='submit']")).Click();

            Thread.Sleep(1000);

            var addedBook = driver.FindElement(By.XPath("//h2[text()='The Hobbit']"));
            Assert.That(addedBook, Is.Not.Null);
        }

        [Test]
        public void DeleteBookTestMethod()
        {
            driver!.Navigate().GoToUrl("http://localhost:5173");

            driver.FindElement(By.LinkText("Add Book")).Click();
            driver.FindElement(By.Name("Title")).SendKeys("1984");
            driver.FindElement(By.Name("Author")).SendKeys("George Orwell");
            driver.FindElement(By.Name("Tag")).SendKeys("Dystopian");
            driver.FindElement(By.Name("Status")).SendKeys("To Read");
            driver.FindElement(By.Name("Rating")).SendKeys("4");
            driver.FindElement(By.CssSelector("form button[type='submit']")).Click();

            Thread.Sleep(1000); 

            var deleteButton = driver.FindElement(By.XPath("//h2[text()='1984']/following-sibling::div[@class='buttons']/button[text()='Delete']"));
            deleteButton.Click();

            Thread.Sleep(500);

            var books = driver.FindElements(By.XPath("//h2[text()='1984']"));
            Assert.That(books.Count, Is.EqualTo(0));
        }

        [Test]
        public void EditBookTestMethod()
        {
            driver!.Navigate().GoToUrl("http://localhost:5173");

            driver.FindElement(By.LinkText("Add Book")).Click();
            driver.FindElement(By.Name("Title")).SendKeys("Brave New World");
            driver.FindElement(By.Name("Author")).SendKeys("Aldous Huxley");
            driver.FindElement(By.Name("Tag")).SendKeys("Sci-Fi");
            driver.FindElement(By.Name("Status")).SendKeys("Reading");
            driver.FindElement(By.Name("Rating")).SendKeys("3");
            driver.FindElement(By.CssSelector("form button[type='submit']")).Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//h2[text()='Brave New World']")));

            var editButton = driver.FindElement(
                By.XPath("//h2[text()='Brave New World']/following-sibling::div[contains(@class,'buttons')]/button[text()='Edit']")
            );
            editButton.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("Author")));

            var authorField = driver.FindElement(By.Name("Author"));
            authorField.Clear();
            authorField.SendKeys("Aldous Leonard Huxley");
            driver.FindElement(By.CssSelector("form button[type='submit']")).Click();

            wait.Until(d => d.PageSource.Contains("Aldous Leonard Huxley"));

            Assert.That(driver.PageSource.Contains("Aldous Leonard Huxley"), Is.True, "Автор не оновився в DOM після редагування.");
        }
    }
}
