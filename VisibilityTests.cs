using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace UdemySeleniumTests
{
    [TestFixture]
    public class VisibilityTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            driver.Navigate().GoToUrl("https://www.udemy.com/");
            Thread.Sleep(3000); // Пауза для загрузки

            // Принимаем куки если есть
            AcceptCookies();
            Thread.Sleep(2000);
        }

        private void AcceptCookies()
        {
            try
            {
                var buttons = driver.FindElements(By.TagName("button"));
                foreach (var button in buttons)
                {
                    try
                    {
                        string text = button.Text.ToLower();
                        if (text.Contains("accept") || text.Contains("принять"))
                        {
                            if (button.Displayed)
                            {
                                button.Click();
                                Console.WriteLine("Куки приняты");
                                Thread.Sleep(1000);
                                return;
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        [Test]
        public void Test2_CheckElementVisibility()
        {
            Console.WriteLine("=== ТЕСТ 2: Проверка видимости элементов на Udemy ===");

            // Список для хранения результатов проверок
            var results = new List<string>();

            // 1. Проверяем заголовок страницы (мета-тег)
            string pageTitle = driver.Title;
            Console.WriteLine($"1. Заголовок страницы: '{pageTitle}'");
            Assert.IsFalse(string.IsNullOrEmpty(pageTitle), "Заголовок страницы не должен быть пустым");
            results.Add($"✓ Заголовок: {pageTitle}");

            // 2. Проверяем видимость логотипа Udemy
            Console.WriteLine("\n2. Ищем логотип Udemy...");
            bool logoVisible = CheckLogoVisibility();
            Assert.IsTrue(logoVisible, "Логотип Udemy должен быть видим");
            results.Add(logoVisible ? "✓ Логотип видим" : "✗ Логотип не найден");

            // 3. Проверяем видимость поля поиска
            Console.WriteLine("\n3. Ищем поле поиска...");
            bool searchVisible = CheckSearchFieldVisibility();
            Assert.IsTrue(searchVisible, "Поле поиска должно быть видимым");
            results.Add(searchVisible ? "✓ Поле поиска видимо" : "✗ Поле поиска не найдено");

            // 4. Проверяем видимость навигационного меню
            Console.WriteLine("\n4. Ищем навигационное меню...");
            bool navVisible = CheckNavigationVisibility();
            Assert.IsTrue(navVisible, "Навигационное меню должно быть видимым");
            results.Add(navVisible ? "✓ Навигация видима" : "✗ Навигация не найдена");

            // 5. Проверяем видимость основного контента
            Console.WriteLine("\n5. Проверяем основной контент...");
            bool contentVisible = CheckMainContentVisibility();
            Assert.IsTrue(contentVisible, "Основной контент должен быть видимым");
            results.Add(contentVisible ? "✓ Контент видим" : "✗ Контент не найден");

            // 6. Проверяем видимость футера
            Console.WriteLine("\n6. Ищем футер...");
            bool footerVisible = CheckFooterVisibility();
            Assert.IsTrue(footerVisible, "Футер должен быть видимым");
            results.Add(footerVisible ? "✓ Футер видим" : "✗ Футер не найден");

            // 7. Проверяем видимость кнопок действия
            Console.WriteLine("\n7. Проверяем кнопки действия...");
            bool buttonsVisible = CheckActionButtonsVisibility();
            Assert.IsTrue(buttonsVisible, "Должны быть видимые кнопки действия");
            results.Add(buttonsVisible ? "✓ Кнопки видимы" : "✗ Кнопки не найдены");

            // 8. Делаем скриншот для доказательства
            Console.WriteLine("\n8. Делаем скриншот видимых элементов...");
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                string fileName = $"visibility_test_{DateTime.Now:HHmmss}.png";
                screenshot.SaveAsFile(fileName);
                Console.WriteLine($"✓ Скриншот сохранен: {fileName}");
                results.Add($"✓ Скриншот: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка скриншота: {ex.Message}");
                results.Add("✗ Ошибка скриншота");
            }

            // Выводим итоговый отчет
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("ИТОГ ПРОВЕРКИ ВИДИМОСТИ:");
            Console.WriteLine(new string('=', 50));

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            Console.WriteLine(new string('=', 50));

            // Подсчитываем успешные проверки
            int passedCount = 0;
            foreach (var result in results)
            {
                if (result.StartsWith("✓")) passedCount++;
            }

            Console.WriteLine($"Успешно: {passedCount}/{results.Count}");

            Assert.Pass($"Проверка видимости завершена. Успешно: {passedCount}/{results.Count}");
        }

        private bool CheckLogoVisibility()
        {
            try
            {
                // Ищем логотип разными способами
                var logoSelectors = new List<By>
                {
                    By.CssSelector("a[href='/']"), // Главная ссылка
                    By.CssSelector("[class*='logo']"),
                    By.CssSelector("[alt*='Udemy']"),
                    By.XPath("//a[contains(@href, 'udemy.com')]//*[contains(@class, 'logo')]"),
                    By.XPath("//*[contains(text(), 'Udemy') and (self::a or self::span or self::div)]")
                };

                foreach (var selector in logoSelectors)
                {
                    try
                    {
                        var elements = driver.FindElements(selector);
                        foreach (var element in elements)
                        {
                            if (element.Displayed)
                            {
                                Console.WriteLine($"Логотип найден: {element.TagName}, Размер: {element.Size.Width}x{element.Size.Height}");
                                return true;
                            }
                        }
                    }
                    catch { }
                }

                Console.WriteLine("Логотип не найден стандартными способами");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка поиска логотипа: {ex.Message}");
                return false;
            }
        }

        private bool CheckSearchFieldVisibility()
        {
            try
            {
                // Ищем поле поиска
                var searchSelectors = new List<By>
                {
                    By.CssSelector("input[type='search']"),
                    By.CssSelector("input[name='q']"),
                    By.CssSelector("input[placeholder*='Search']"),
                    By.CssSelector("input[placeholder*='Поиск']"),
                    By.XPath("//input[contains(@placeholder, 'earch')]")
                };

                foreach (var selector in searchSelectors)
                {
                    try
                    {
                        var elements = driver.FindElements(selector);
                        foreach (var element in elements)
                        {
                            if (element.Displayed && element.Enabled)
                            {
                                string placeholder = element.GetAttribute("placeholder") ?? "без placeholder";
                                Console.WriteLine($"Поле поиска найдено: {placeholder}");
                                Console.WriteLine($"Координаты: X={element.Location.X}, Y={element.Location.Y}");
                                Console.WriteLine($"Размеры: Ш={element.Size.Width}, В={element.Size.Height}");
                                return true;
                            }
                        }
                    }
                    catch { }
                }

                // Проверяем все input элементы
                var allInputs = driver.FindElements(By.TagName("input"));
                Console.WriteLine($"Всего input элементов: {allInputs.Count}");

                foreach (var input in allInputs)
                {
                    try
                    {
                        if (input.Displayed)
                        {
                            string type = input.GetAttribute("type") ?? "";
                            string placeholder = input.GetAttribute("placeholder") ?? "";

                            Console.WriteLine($"Input: type='{type}', placeholder='{placeholder}', " +
                                            $"Displayed={input.Displayed}, Enabled={input.Enabled}");
                        }
                    }
                    catch { }
                }

                Console.WriteLine("Поле поиска не найдено");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка поиска поля: {ex.Message}");
                return false;
            }
        }

        private bool CheckNavigationVisibility()
        {
            try
            {
                // Ищем навигационные элементы
                var navElements = driver.FindElements(By.CssSelector(
                    "nav, header, [role='navigation'], [class*='nav'], [class*='menu']"
                ));

                Console.WriteLine($"Найдено навигационных контейнеров: {navElements.Count}");

                if (navElements.Count == 0)
                {
                    // Ищем навигационные ссылки напрямую
                    var navLinks = driver.FindElements(By.CssSelector(
                        "a[href*='/courses/'], a[href*='/topic/'], a[href*='/development/']"
                    ));

                    Console.WriteLine($"Найдено навигационных ссылок: {navLinks.Count}");

                    int visibleLinks = 0;
                    foreach (var link in navLinks)
                    {
                        if (link.Displayed)
                        {
                            visibleLinks++;
                            Console.WriteLine($"Видимая ссылка: {link.Text}");
                        }
                    }

                    return visibleLinks > 0;
                }

                // Проверяем видимость навигационных контейнеров
                foreach (var nav in navElements)
                {
                    if (nav.Displayed)
                    {
                        Console.WriteLine($"Навигация видима: {nav.TagName}, " +
                                        $"Размер: {nav.Size.Width}x{nav.Size.Height}");

                        // Проверяем, есть ли внутри видимые ссылки
                        var linksInside = nav.FindElements(By.TagName("a"));
                        int visibleLinks = 0;
                        foreach (var link in linksInside)
                        {
                            if (link.Displayed && !string.IsNullOrEmpty(link.Text))
                            {
                                visibleLinks++;
                            }
                        }

                        Console.WriteLine($"Видимых ссылок внутри: {visibleLinks}");
                        return visibleLinks > 0;
                    }
                }

                Console.WriteLine("Видимой навигации не найдено");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка поиска навигации: {ex.Message}");
                return false;
            }
        }

        private bool CheckMainContentVisibility()
        {
            try
            {
                // Ищем основной контент
                var contentSelectors = new List<By>
                {
                    By.CssSelector("main, [role='main'], [class*='content'], [class*='container']"),
                    By.CssSelector("section, article, .main-content"),
                    By.TagName("h1"), // Заголовки
                    By.TagName("h2")
                };

                int visibleElements = 0;

                foreach (var selector in contentSelectors)
                {
                    try
                    {
                        var elements = driver.FindElements(selector);
                        foreach (var element in elements)
                        {
                            if (element.Displayed && element.Size.Height > 0)
                            {
                                visibleElements++;

                                if (visibleElements == 1) // Выводим информацию только о первом найденном
                                {
                                    Console.WriteLine($"Контент найден: {element.TagName}");
                                    Console.WriteLine($"Текст: {element.Text.Substring(0, Math.Min(50, element.Text.Length))}...");
                                    Console.WriteLine($"Размер: {element.Size.Width}x{element.Size.Height}");
                                }
                            }
                        }
                    }
                    catch { }
                }

                Console.WriteLine($"Всего видимых контент-элементов: {visibleElements}");
                return visibleElements > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка поиска контента: {ex.Message}");
                return false;
            }
        }

        private bool CheckFooterVisibility()
        {
            try
            {
                // Ищем футер
                var footerElements = driver.FindElements(By.CssSelector(
                    "footer, [class*='footer'], [id*='footer']"
                ));

                Console.WriteLine($"Найдено футер-элементов: {footerElements.Count}");

                foreach (var footer in footerElements)
                {
                    if (footer.Displayed)
                    {
                        Console.WriteLine($"Футер найден: {footer.TagName}");
                        Console.WriteLine($"Размер: {footer.Size.Width}x{footer.Size.Height}");
                        Console.WriteLine($"Позиция: Y={footer.Location.Y} (должен быть внизу)");

                        // Проверяем, что футер действительно внизу
                        if (footer.Location.Y > 500) // Примерно внизу страницы
                        {
                            return true;
                        }
                    }
                }

                // Если не нашли стандартный футер, ищем элементы внизу страницы
                var allElements = driver.FindElements(By.CssSelector(
                    "div, section, footer"
                ));

                IWebElement bottomElement = null;
                int maxY = 0;

                foreach (var element in allElements)
                {
                    try
                    {
                        if (element.Displayed && element.Location.Y > maxY)
                        {
                            maxY = element.Location.Y;
                            bottomElement = element;
                        }
                    }
                    catch { }
                }

                if (bottomElement != null && maxY > 1000)
                {
                    Console.WriteLine($"Нижний элемент: {bottomElement.TagName}, Y={maxY}");
                    return true;
                }

                Console.WriteLine("Футер не найден");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка поиска футера: {ex.Message}");
                return false;
            }
        }

        private bool CheckActionButtonsVisibility()
        {
            try
            {
                // Ищем кнопки действия (логин, регистрация, поиск и т.д.)
                var buttons = driver.FindElements(By.CssSelector(
                    "button, a[class*='btn'], input[type='submit'], [role='button']"
                ));

                Console.WriteLine($"Всего кнопок/действий: {buttons.Count}");

                List<string> visibleButtons = new List<string>();

                foreach (var button in buttons)
                {
                    try
                    {
                        if (button.Displayed && button.Enabled)
                        {
                            string buttonText = button.Text.Trim();
                            if (!string.IsNullOrEmpty(buttonText) && buttonText.Length < 30)
                            {
                                visibleButtons.Add(buttonText);
                            }
                        }
                    }
                    catch { }
                }

                Console.WriteLine($"Видимых кнопок с текстом: {visibleButtons.Count}");

                if (visibleButtons.Count > 0)
                {
                    Console.WriteLine("Примеры кнопок:");
                    for (int i = 0; i < Math.Min(5, visibleButtons.Count); i++)
                    {
                        Console.WriteLine($"  - {visibleButtons[i]}");
                    }
                    return true;
                }

                Console.WriteLine("Видимых кнопок не найдено");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка поиска кнопок: {ex.Message}");
                return false;
            }
        }

        [TearDown]
        public void Cleanup()
        {
            Thread.Sleep(1000);
            driver.Quit();
        }
    }
}
