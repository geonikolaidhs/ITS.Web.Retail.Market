using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using Xunit;

namespace ITS.Retail.Platform.Tests
{
    public class SeleniumTest
    {
        public SeleniumTest()
        {
            
        }

        [Fact]
        public void TestLogin()
        {
            using(ChromeDriver driver = new ChromeDriver())
            {
                Actions actions = new Actions(driver);
                driver.Navigate().GoToUrl("http://localhost/ITS.Retail.WebClient/Login");

                driver.FindElementById("UserName").SendKeys("admin");
                driver.FindElementById("Password").SendKeys("m@s0ut1s1ts");
                driver.FindElementById("Login").Click();

                Assert.NotNull(driver.FindElementById("menuLabel"));

                actions.MoveToElement(driver.FindElementById("LayoutCompanySelection")).Perform();                
                driver.FindElementById("79e4b4fc-2532-47fb-8c27-98afceb226c3").Click();

                actions.MoveToElement(driver.FindElementById("LayoutStoreSelection")).Perform();
                driver.FindElementById("de0a8ad0-f194-42b4-9f72-583ee34ee880").Click();

                driver.FindElementById("menuLabel").Click();

                //driver.Navigate().GoToUrl("http://localhost/ITS.Retail.WebClient/ITS.Retail.WebClient/Home/Settings");
                int i = 0;
            }
        }

        [Fact]
        public void TestFailedLogin()
        {
            using(ChromeDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("http://localhost/ITS.Retail.WebClient/Login");
                driver.FindElementById("UserName").SendKeys("admin");
                driver.FindElementById("Password").SendKeys("m@s0ut1s1ts123");
                driver.FindElementByName("Login").Click();

                Assert.NotNull(driver.FindElementById("UserName"));           
            }
        }

        


    }
}
