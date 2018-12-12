using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DaOAuthV2.Gui.Front.Test
{
    [TestClass]
    public class LoginTest
    {
        private IWebDriver _webDriver;

        [TestMethod]
        public void LoginSuccessfull()
        {
            try
            {
                _webDriver = new ChromeDriver();

                _webDriver.Navigate().GoToUrl(@"http://front.daoauth.fr");
                _webDriver.FindElement(By.Id("UserName")).SendKeys("david");
                _webDriver.FindElement(By.Id("Password")).SendKeys("azertyui");
                _webDriver.FindElement(By.ClassName("form-signin")).Submit();

                // check if connected
                var clientCountElem = _webDriver.FindElement(By.Id("clientCount"));
                Assert.IsNotNull(clientCountElem);
                Assert.AreEqual("5", clientCountElem.Text);
            }
            catch
            {
                throw;
            }
            finally
            {
                _webDriver.Close();
            }
        }
    }
}
