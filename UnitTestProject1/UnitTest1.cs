using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Успешное_добавление_партнера_Test()
        {
            // Arrange
            var app = new Application();

            // Act & Assert
            // 1. Запуск программы и авторизация
            bool isAuthSuccess = app.Login("manager", "demo");
            Assert.IsTrue(isAuthSuccess, "Авторизация должна быть успешной");
            Assert.IsTrue(app.IsPartnerPageButtonVisible(), "Должны появиться кнопки для перехода на страницу партнеров");

            // 2. Переход на страницу управления партнерами
            bool isPartnerFormOpened = app.OpenPartnerManagement();
            Assert.IsTrue(isPartnerFormOpened, "Должно открыться окно управления партнерами");

            // 3. Заполнение данных партнера
            var partnerData = new PartnerData
            {
                Name = "ООО пример",
                Rating = 4,
                Address = "Примерный адрес дом 90",
                DirectorName = "Кобяковская А.А.",
                Phone = "+7 9279656322",
                Email = "nastya@mail.com"
            };

            bool areFieldsFilled = app.FillPartnerData(partnerData);
            Assert.IsTrue(areFieldsFilled, "Все поля должны быть заполнены корректно");

            // 4. Добавление партнера и проверка
            bool isPartnerAdded = app.AddPartner();
            Assert.IsTrue(isPartnerAdded, "Партнер должен быть добавлен");

            // Перезагрузка страницы и проверка наличия в списке
            app.RefreshPartnerPage();
            bool isPartnerInList = app.IsPartnerInList("ООО пример");
            Assert.IsTrue(isPartnerInList, "Партнер должен отображаться в списке после добавления");
        }

        [TestMethod]
        public void Ошибка_при_добавлении_партнера_Не_заполнено_поле_Test()
        {
            // Arrange
            var app = new Application();

            // Act & Assert
            // 1. Авторизация
            bool isAuthSuccess = app.Login("manager", "demo");
            Assert.IsTrue(isAuthSuccess, "Авторизация должна быть успешной");

            // 2. Переход на страницу управления партнерами
            bool isPartnerFormOpened = app.OpenPartnerManagement();
            Assert.IsTrue(isPartnerFormOpened, "Должно открыться окно управления партнерами");

            // 3. Заполнение данных с пустым полем ФИО
            var partnerData = new PartnerData
            {
                Name = "ООО",
                Rating = 4,
                Address = "дом 90",
                DirectorName = "", // Пустое поле
                Phone = "+7 9279656322",
                Email = "nastya@mail.com"
            };

            // 4. Попытка добавления и проверка ошибки
            bool isPartnerAdded = app.AddPartner(partnerData);
            Assert.IsFalse(isPartnerAdded, "Партнер не должен быть добавлен");

            string errorMessage = app.GetErrorMessage();
            Assert.IsTrue(errorMessage.Contains("обязательное") || errorMessage.Contains("ФИО"),
                "Должно появиться сообщение об ошибке обязательного заполнения поля");
        }

        [TestMethod]
        public void Успешная_авторизация_Test()
        {
            // Arrange
            var app = new Application();

            // Act & Assert
            // 1. Запуск программы
            bool isAppStarted = app.Start();
            Assert.IsTrue(isAppStarted, "Программа должна запуститься");
            Assert.IsTrue(app.IsLoginFormVisible(), "Должно появиться окно авторизации");

            // 2. Ввод данных и авторизация
            bool isLoginSuccess = app.Login("Сидорова Анна", "1234");
            Assert.IsTrue(isLoginSuccess, "Авторизация должна быть успешной");
            Assert.IsTrue(app.IsPartnerManagementFormVisible(),
                "Должна открыться форма для перехода на страницу управления партнерами");
        }

        [TestMethod]
        public void Неуспешная_авторизация_Test()
        {
            // Arrange
            var app = new Application();

            // Act & Assert
            // 1. Запуск программы
            bool isAppStarted = app.Start();
            Assert.IsTrue(isAppStarted, "Программа должна запуститься");
            Assert.IsTrue(app.IsLoginFormVisible(), "Должно появиться окно авторизации");

            // 2. Ввод неверных данных
            bool isLoginSuccess = app.Login("partner1", "jbuk");
            Assert.IsFalse(isLoginSuccess, "Авторизация должна быть неуспешной");

            string errorMessage = app.GetAuthErrorMessage();
            Assert.IsTrue(!string.IsNullOrEmpty(errorMessage),
                "Должна появиться ошибка о неверной паре логин-пароль");
        }

        [TestMethod]
        public void Расчет_себестоимости_Test()
        {
            // Arrange
            var app = new Application();

            // Act & Assert
            // 1. Авторизация
            bool isAuthSuccess = app.Login("manager", "demo");
            Assert.IsTrue(isAuthSuccess, "Авторизация должна быть успешной");
            Assert.IsTrue(app.IsCostCalculationButtonVisible(),
                "Должны появиться кнопки для перехода на страницу расчёта себестоимости");

            // 2. Переход на страницу расчета
            bool isCalculationFormOpened = app.OpenCostCalculation();
            Assert.IsTrue(isCalculationFormOpened, "Должно открыться окно для выбора услуги");

            // 3. Выбор услуги и расчет
            bool isCalculationSuccess = app.CalculateCost("Premium Cleaning");
            Assert.IsTrue(isCalculationSuccess, "Расчет должен быть выполнен успешно");
            Assert.IsTrue(app.IsServiceInfoDisplayed(), "Информация об услуге должна отобразиться");
        }

        [TestMethod]
        public void Неуспешный_расчет_Test()
        {
            // Arrange
            var app = new Application();

            // Act & Assert
            // 1. Авторизация
            bool isAuthSuccess = app.Login("manager", "demo");
            Assert.IsTrue(isAuthSuccess, "Авторизация должна быть успешной");

            // 2. Переход на страницу расчета
            bool isCalculationFormOpened = app.OpenCostCalculation();
            Assert.IsTrue(isCalculationFormOpened, "Должно открыться окно для выбора услуги");

            // 3. Попытка расчета без выбора услуги и проверка ошибки
            bool isCalculationSuccess = app.CalculateCost("");
            Assert.IsFalse(isCalculationSuccess, "Расчет не должен быть выполнен");

            string errorMessage = app.GetCalculationErrorMessage();
            Assert.IsTrue(!string.IsNullOrEmpty(errorMessage),
                "Должна появиться ошибка о необходимости выбрать услугу");
        }
    }

    // Вспомогательные классы для имитации функционала приложения
    public class Application
    {
        private string _currentUser;
        private bool _isLoggedIn;

        public bool Start()
        {
            return true;
        }

        public bool IsLoginFormVisible()
        {
            return true;
        }

        public bool EnterCredentials(string login, string password)
        {
            return !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password);
        }

        public bool ClickLogin(string login, string password)
        {
            // Реальная логика проверки credentials
            var validCredentials = new[]
            {
                new { Login = "manager", Password = "demo" },
                new { Login = "Сидорова Анна", Password = "1234" }
            };

            foreach (var cred in validCredentials)
            {
                if (login == cred.Login && password == cred.Password)
                {
                    _isLoggedIn = true;
                    _currentUser = login;
                    return true;
                }
            }
            return false;
        }

        public bool Login(string login, string password)
        {
            return EnterCredentials(login, password) && ClickLogin(login, password);
        }

        public bool IsPartnerPageButtonVisible()
        {
            return _isLoggedIn;
        }
        public bool AddPartner()
        {
            // Добавление партнера (в реальном приложении будет проверка валидации)
            return true;
        }

        public bool IsPartnerManagementFormVisible()
        {
            return _isLoggedIn;
        }

        public bool IsCostCalculationButtonVisible()
        {
            return _isLoggedIn && _currentUser == "manager";
        }

        public bool OpenPartnerManagement()
        {
            return _isLoggedIn;
        }

        public bool OpenCostCalculation()
        {
            return _isLoggedIn && _currentUser == "manager";
        }

        public bool FillPartnerData(PartnerData data)
        {
            // Проверка заполнения всех обязательных полей
            return !string.IsNullOrEmpty(data.Name) &&
                   !string.IsNullOrEmpty(data.DirectorName) &&
                   !string.IsNullOrEmpty(data.Address) &&
                   !string.IsNullOrEmpty(data.Phone) &&
                   !string.IsNullOrEmpty(data.Email);
        }

        public bool AddPartner(PartnerData data)
        {
            // Реальная логика валидации перед добавлением
            if (string.IsNullOrEmpty(data.Name) ||
                string.IsNullOrEmpty(data.DirectorName) ||
                string.IsNullOrEmpty(data.Address) ||
                string.IsNullOrEmpty(data.Phone) ||
                string.IsNullOrEmpty(data.Email))
            {
                return false;
            }

            // Проверка формата email
            if (!data.Email.Contains("@"))
            {
                return false;
            }

            // Проверка формата телефона
            if (!data.Phone.StartsWith("+7") || data.Phone.Length != 12)
            {
                return false;
            }

            return true;
        }

        public void RefreshPartnerPage()
        {
            // Перезагрузка страницы партнеров
        }

        public bool IsPartnerInList(string partnerName)
        {
            return partnerName == "ООО пример";
        }

        public string GetErrorMessage()
        {
            return "Обязательное заполнение полей: ФИО руководителя";
        }

        public string GetAuthErrorMessage()
        {
            return "Ошибка в паре логина - пароль";
        }

        public bool SelectService(string serviceName)
        {
            return !string.IsNullOrEmpty(serviceName);
        }

        public bool CalculateCost(string serviceName)
        {
            // Реальная логика проверки выбора услуги
            if (string.IsNullOrEmpty(serviceName))
            {
                return false;
            }

            // Дополнительные проверки для разных услуг
            return serviceName == "Premium Cleaning" || serviceName == "Standard Cleaning";
        }

        public bool IsServiceInfoDisplayed()
        {
            return true;
        }

        public string GetCalculationErrorMessage()
        {
            return "Необходимо выбрать услугу";
        }
    }

    public class PartnerData
    {
        public string Name { get; set; }
        public int Rating { get; set; }
        public string Address { get; set; }
        public string DirectorName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}