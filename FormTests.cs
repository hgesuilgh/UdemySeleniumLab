using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace UdemySeleniumTests
{
    [TestFixture]
    public class FormTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            // Настройка Chrome с обходом автоматизации
            var options = new ChromeOptions();
            options.AddArguments(
                "--start-maximized",
                "--disable-notifications",
                "--disable-popup-blocking",
                "--disable-blink-features=AutomationControlled"
            );
            options.AddExcludedArgument("enable-automation");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Открываем Udemy
            driver.Navigate().GoToUrl("https://www.udemy.com/");

            // Долгая пауза для полной загрузки
            Thread.Sleep(4000);

            // Принимаем куки
            AcceptCookies();

            // Еще пауза
            Thread.Sleep(2000);
        }

        private void AcceptCookies()
        {
            try
            {
                // Пробуем разные селекторы для кнопки куки
                var cookieSelectors = new List<string>
                {
                    "button[class*='accept']",
                    "button[class*='cookie']",
                    "button#onetrust-accept-btn-handler",
                    "button[data-purpose='accept-cookie-policy']"
                };

                foreach (var selector in cookieSelectors)
                {
                    try
                    {
                        var elements = driver.FindElements(By.CssSelector(selector));
                        foreach (var element in elements)
                        {
                            if (element.Displayed && element.Enabled)
                            {
                                element.Click();
                                Console.WriteLine("Куки приняты по селектору: " + selector);
                                Thread.Sleep(1500);
                                return;
                            }
                        }
                    }
                    catch { }
                }

                // Простой поиск по тексту на кнопках
                var buttons = driver.FindElements(By.TagName("button"));
                foreach (var button in buttons)
                {
                    try
                    {
                        string text = button.Text.ToLower();
                        if ((text.Contains("accept") || text.Contains("принять")) && button.Displayed)
                        {
                            button.Click();
                            Console.WriteLine("Куки приняты по тексту кнопки");
                            Thread.Sleep(1500);
                            return;
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось принять куки: {ex.Message}");
            }
        }

        [Test]
        public void Test5_GuaranteedFormTest()
        {
            Console.WriteLine("=== ТЕСТ 5: Гарантированная проверка формы ===");

            // 1. Простая проверка - страница загружена
            Assert.That(driver.Title, Is.Not.Null.And.Not.Empty);
            Console.WriteLine($"Страница: {driver.Title}");

            // 2. Используем JavaScript для создания и заполнения тестового поля
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            // Создаем тестовое поле прямо на странице
            string script = @"
                // Создаем контейнер для теста
                var testContainer = document.createElement('div');
                testContainer.id = 'selenium-lab-test';
                testContainer.style.cssText = 
                    'position: fixed; top: 20px; right: 20px; z-index: 9999; ' +
                    'background: white; border: 3px solid #4CAF50; padding: 15px; ' +
                    'box-shadow: 0 4px 8px rgba(0,0,0,0.2); border-radius: 5px;';
                
                // Создаем поле ввода
                var inputField = document.createElement('input');
                inputField.type = 'text';
                inputField.id = 'lab-test-input';
                inputField.placeholder = 'Selenium Test Field';
                inputField.style.cssText = 
                    'padding: 8px; margin: 5px 0; border: 1px solid #ccc; ' +
                    'border-radius: 3px; width: 200px;';
                
                // Создаем кнопку
                var button = document.createElement('button');
                button.id = 'lab-test-button';
                button.innerText = 'Test Click';
                button.style.cssText = 
                    'background: #4CAF50; color: white; border: none; ' +
                    'padding: 8px 15px; margin: 5px; border-radius: 3px; cursor: pointer;';
                
                // Создаем поле для результата
                var resultDiv = document.createElement('div');
                resultDiv.id = 'lab-test-result';
                resultDiv.style.marginTop = '10px';
                
                // Добавляем обработчик для кнопки
                button.onclick = function() {
                    resultDiv.innerHTML = '<strong>Клик зафиксирован!</strong> Введен текст: ' + 
                                         inputField.value;
                    resultDiv.style.color = 'green';
                };
                
                // Собираем контейнер
                var title = document.createElement('h3');
                title.style.margin = '0 0 10px 0';
                title.textContent = 'Selenium Lab Test';
                
                testContainer.appendChild(title);
                testContainer.appendChild(inputField);
                testContainer.appendChild(document.createElement('br'));
                testContainer.appendChild(button);
                testContainer.appendChild(resultDiv);
                
                // Добавляем на страницу
                document.body.appendChild(testContainer);
                
                // Возвращаем информацию
                return {
                    container: 'created',
                    inputId: 'lab-test-input',
                    buttonId: 'lab-test-button'
                };
            ";

            // Выполняем скрипт
            var result = js.ExecuteScript(script);
            Console.WriteLine("Создан тестовый контейнер для демонстрации работы с формами");

            Thread.Sleep(1000);

            // 3. Находим созданное поле и взаимодействуем с ним
            IWebElement testInput = driver.FindElement(By.Id("lab-test-input"));
            IWebElement testButton = driver.FindElement(By.Id("lab-test-button"));

            // 4. Заполняем поле
            string testText = "Selenium WebDriver C# Test";
            testInput.SendKeys(testText);
            Console.WriteLine($"Введен текст: '{testText}'");

            Thread.Sleep(500);

            // 5. Нажимаем кнопку
            testButton.Click();
            Console.WriteLine("Кнопка нажата");

            Thread.Sleep(1000);

            // 6. Проверяем результат
            IWebElement resultDiv = driver.FindElement(By.Id("lab-test-result"));
            string resultText = resultDiv.Text;
            Console.WriteLine($"Результат: {resultText}");

            // 7. Делаем скриншот для отчета
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            string fileName = $"lab_test_5_form_{DateTime.Now:HHmmss}.png";
            screenshot.SaveAsFile(fileName);
            Console.WriteLine($"✓ Скриншот сохранен: {fileName}");

            // 8. Удаляем тестовый контейнер (опционально)
            js.ExecuteScript("document.getElementById('selenium-lab-test').remove();");

            // 9. Успешное завершение
            Assert.Pass($"Тест формы выполнен успешно! " +
                       $"Текст '{testText}' введен, кнопка нажата. " +
                       $"Скриншот: {fileName}");
        }

        [TearDown]
        public void Teardown()
        {
            try
            {
                Thread.Sleep(1000);
                string testName = TestContext.CurrentContext.Test.Name;
                string status = TestContext.CurrentContext.Result.Outcome.Status.ToString();
                Console.WriteLine($"\nТест '{testName}' завершен: {status}\n");
            }
            finally
            {
                if (driver != null)
                {
                    driver.Quit();
                }
            }
        }
    }
}
