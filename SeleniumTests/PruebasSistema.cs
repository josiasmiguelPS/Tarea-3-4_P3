using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.IO;
using System.Threading;

namespace SeleniumTests
{
    [TestFixture]
    public class PruebasSistema
    {
        private const string BASE_URL = "http://localhost:5142/index.html";

        private IWebDriver Driver;
        private static ExtentReports extent;
        private ExtentTest test;
        private static string reportesDir;

        [OneTimeSetUp]
        public void SetupReporte()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
            reportesDir = Path.Combine(projectRoot, "Reportes");

            if (!Directory.Exists(reportesDir))
            {
                Directory.CreateDirectory(reportesDir);
            }

            var reporter = new ExtentSparkReporter(Path.Combine(reportesDir, "Reporte_Tarea4.html"));
            extent = new ExtentReports();
            extent.AttachReporter(reporter);
        }

        [OneTimeTearDown]
        public void FinalizarReporte() => extent.Flush();

        [SetUp]
        public void IniciarNavegador()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            Driver = new ChromeDriver(options);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            test = extent.CreateTest(TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void CerrarNavegador()
        {
            var foto = ((ITakesScreenshot)Driver).GetScreenshot();
            string fileName = $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now.ToString("HHmmss")}.png";
            string path = Path.Combine(reportesDir, fileName);
            foto.SaveAsFile(path);

            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                test.Fail("Prueba fallida").AddScreenCaptureFromPath(path);
            }
            else
            {
                test.Pass("Prueba exitosa").AddScreenCaptureFromPath(path);
            }

            Driver.Quit();
            Driver.Dispose();
        }

        private void HacerLogin()
        {
            Driver.Navigate().GoToUrl(BASE_URL);
            Thread.Sleep(2000);

            var user = Driver.FindElement(By.Id("username"));
            user.Clear();
            user.SendKeys("admin");

            var pass = Driver.FindElement(By.Id("password"));
            pass.Clear();
            pass.SendKeys("admin123");

            Driver.FindElement(By.Id("loginBtn")).Click();
            Thread.Sleep(2000);
        }

        [Test]
        public void Login_CaminoFeliz()
        {
            HacerLogin();
            Assert.That(Driver.FindElement(By.Id("appPage")).Displayed, Is.True);
        }

        [Test]
        public void Login_PruebaNegativa_CredencialesMalas()
        {
            Driver.Navigate().GoToUrl(BASE_URL);
            Thread.Sleep(2000);

            var user = Driver.FindElement(By.Id("username"));
            user.Clear();
            user.SendKeys("admin");

            var pass = Driver.FindElement(By.Id("password"));
            pass.Clear();
            pass.SendKeys("clave_equivocada");

            Driver.FindElement(By.Id("loginBtn")).Click();
            Thread.Sleep(1000);

            Assert.That(Driver.FindElement(By.Id("loginError")).Displayed, Is.True);
        }

        [Test]
        public void CrearEmpleado_CaminoFeliz()
        {
            HacerLogin();

            Driver.FindElement(By.Id("nombre")).SendKeys("Josias");
            Driver.FindElement(By.Id("apellido")).SendKeys("Perez");
            Driver.FindElement(By.Id("email")).SendKeys("josias@test.com");
            Driver.FindElement(By.Id("btnGuardar")).Click();

            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElement(By.Id("formFeedback")).GetAttribute("textContent").Length > 0);

            string mensaje = Driver.FindElement(By.Id("formFeedback")).GetAttribute("textContent");
            Assert.That(mensaje, Is.Not.Empty, "El mensaje de confirmacion no debe estar vacio.");
        }

        [Test]
        public void CrearEmpleado_PruebaNegativa_FaltaNombre()
        {
            HacerLogin();

            Driver.FindElement(By.Id("apellido")).SendKeys("Perez");
            Driver.FindElement(By.Id("btnGuardar")).Click();

            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElement(By.Id("formFeedback")).GetAttribute("textContent").Length > 0);

            string mensaje = Driver.FindElement(By.Id("formFeedback")).GetAttribute("textContent");
            Assert.That(mensaje, Is.Not.Empty, "Debe aparecer un error por faltar el nombre.");
        }

        [Test]
        public void CrearEmpleado_PruebaLimites_NumeroLargo()
        {
            HacerLogin();

            Driver.FindElement(By.Id("nombre")).SendKeys("Maria");
            Driver.FindElement(By.Id("apellido")).SendKeys("Gomez");
            Driver.FindElement(By.Id("numero")).SendKeys("1234567890");
            Driver.FindElement(By.Id("btnGuardar")).Click();

            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElement(By.Id("formFeedback")).GetAttribute("textContent").Length > 0);

            string mensaje = Driver.FindElement(By.Id("formFeedback")).GetAttribute("textContent");
            Assert.That(mensaje, Is.Not.Empty, "El mensaje de confirmacion no debe estar vacio.");
        }

        [Test]
        public void ListarEmpleados_CaminoFeliz()
        {
            HacerLogin();
            var tabla = Driver.FindElement(By.Id("tablaEmpleados"));
            Assert.That(tabla.Displayed, Is.True);
        }

        [Test]
        public void EliminarEmpleado_CaminoFeliz()
        {
            HacerLogin();

            var botonesEliminar = Driver.FindElements(By.CssSelector(".btn-eliminar"));
            if (botonesEliminar.Count > 0)
            {
                botonesEliminar[0].Click();
                Thread.Sleep(1000);

                Driver.FindElement(By.Id("btnConfirmDelete")).Click();
                Thread.Sleep(2000);
            }

            Assert.Pass();
        }
    }
}