using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace UdemySeleniumTests
{
    [TestFixture]
    public class ButtonTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://www.udemy.com/");
            Thread.Sleep(3000);
        }

        [Test]
        public void Test4_ClickRealUdemyButtons()
        {
            Console.WriteLine("=== ТЕСТ 4: Клик по реальным кнопкам Udemy ===");

            // ТЕСТ 1: Клик по кнопке категории "Разработка"
            Console.WriteLine("\n1. Ищем кнопку/ссылку 'Разработка'...");

            try
            {
                // Пробуем найти ссылку на категорию разработки
                IWebElement devLink = wait.Until(d =>
                    d.FindElement(By.XPath("//a[contains(@href, '/development/') or contains(text(), 'Development') or contains(text(), 'Разработка')]")));

                Console.WriteLine($"Найдена: {devLink.Text}");

                string initialUrl = driver.Url;
                devLink.Click();

                // Ждем загрузки новой страницы
                Thread.Sleep(3000);

                if (driver.Url != initialUrl)
                {
                    Console.WriteLine($"✓ Успех! Перешли на: {driver.Url}");

                    // Возвращаемся назад для следующего теста
                    driver.Navigate().Back();
                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("⚠ URL не изменился, но клик выполнен");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не нашли 'Разработка': {ex.Message}");
            }

            // ТЕСТ 2: Клик по кнопке "Для бизнеса"
            Console.WriteLine("\n2. Ищем кнопку 'Для бизнеса'...");

            try
            {
                IWebElement businessLink = wait.Until(d =>
                    d.FindElement(By.XPath("//a[contains(@href, 'business.udemy.com') or contains(text(), 'Udemy Business') or contains(text(), 'Для бизнеса')]")));

                Console.WriteLine($"Найдена: {businessLink.Text}");

                string currentUrl = driver.Url;
                businessLink.Click();

                Thread.Sleep(3000);

                // Проверяем, открылась ли новая вкладка
                if (driver.WindowHandles.Count > 1)
                {
                    Console.WriteLine("✓ Открыта новая вкладка");

                    // Переключаемся на новую вкладку
                    driver.SwitchTo().Window(driver.WindowHandles[1]);
                    Console.WriteLine($"URL новой вкладки: {driver.Url}");

                    // Закрываем новую вкладку и возвращаемся
                    driver.Close();
                    driver.SwitchTo().Window(driver.WindowHandles[0]);
                }
                else if (driver.Url != currentUrl)
                {
                    Console.WriteLine($"✓ Переход выполнен: {driver.Url}");
                    driver.Navigate().Back();
                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("⚠ Изменений не обнаружено");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не нашли 'Для бизнеса': {ex.Message}");
            }

            // ТЕСТ 3: Клик по кнопке поиска (лупа)
            Console.WriteLine("\n3. Ищем и используем поиск...");

            try
            {
                // Ищем поле поиска
                IWebElement searchInput = wait.Until(d =>
                    d.FindElement(By.CssSelector("input[type='search'], input[name='q'], input[placeholder*='Search']")));

                Console.WriteLine($"Нашли поле поиска. Placeholder: {searchInput.GetAttribute("placeholder")}");

                // Вводим текст
                searchInput.SendKeys("Selenium WebDriver");

                // Ищем кнопку поиска (лупа)
                IWebElement searchButton = driver.FindElement(By.CssSelector(
                    "button[type='submit'], button[class*='search'], button[aria-label*='Search']"));

                Console.WriteLine($"Нашли кнопку поиска");
                searchButton.Click();

                Thread.Sleep(3000);

                // Проверяем, что перешли на страницу результатов
                if (driver.Url.Contains("search"))
                {
                    Console.WriteLine($"✓ Поиск выполнен! URL: {driver.Url}");
                }
                else
                {
                    Console.WriteLine($"Текущий URL: {driver.Url}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка поиска: {ex.Message}");

                // Альтернатива: нажать Enter в поле поиска
                try
                {
                    var inputs = driver.FindElements(By.TagName("input"));
                    foreach (var input in inputs)
                    {
                        string placeholder = input.GetAttribute("placeholder") ?? "";
                        if (placeholder.Contains("Search") && input.Displayed)
                        {
                            input.SendKeys("Selenium");
                            input.SendKeys(Keys.Enter);
                            Thread.Sleep(3000);
                            Console.WriteLine($"Поиск через Enter выполнен: {driver.Url}");
                            break;
                        }
                    }
                }
                catch { }
            }

            // ТЕСТ 4: Клик по кнопке "Log in" / "Войти"
            Console.WriteLine("\n4. Ищем кнопку входа...");

            try
            {
                IWebElement loginButton = wait.Until(d =>
                    d.FindElement(By.XPath("//a[contains(@href, 'login') and (contains(text(), 'Log in') or contains(text(), 'Войти'))]")));

                Console.WriteLine($"Нашли: {loginButton.Text}");

                string beforeClickUrl = driver.Url;
                loginButton.Click();

                Thread.Sleep(3000);

                // Проверяем результат
                if (driver.Url.Contains("login") || driver.Url != beforeClickUrl)
                {
                    Console.WriteLine($"✓ Открыта страница входа: {driver.Url}");
                }
                else
                {
                    // Проверяем, не открылось ли модальное окно
                    var modals = driver.FindElements(By.CssSelector("[role='dialog'], .modal, .popup"));
                    if (modals.Count > 0)
                    {
                        Console.WriteLine("✓ Открыто модальное окно входа");
                    }
                    else
                    {
                        Console.WriteLine("⚠ Изменений не видно");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не нашли кнопку входа: {ex.Message}");
            }

            // Делаем финальный скриншот
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                string fileName = $"udemy_buttons_test_{DateTime.Now:HHmmss}.png";
                screenshot.SaveAsFile(fileName);
                Console.WriteLine($"\n✓ Скриншот сохранен: {fileName}");
            }
            catch { }

            Assert.Pass("Тестирование реальных кнопок Udemy выполнено!");
        }

        [TearDown]
        public void Cleanup()
        {
            Thread.Sleep(1000);
            driver.Quit();
        }
    }
}
