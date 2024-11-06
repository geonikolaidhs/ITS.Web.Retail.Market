using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using WatiN.Core;

namespace ITS.Retail.WebClient.IntegrationTests
{
    [TestFixture]
    [RequiresSTA]
    public class DemoTest
    {
        [Test]
        public void DemoTestGoogleSearch()
        {
            using (var browser = new IE("google.com"))
            {
                browser.TextField(Find.ByName("q")).TypeText("demo");
                browser.Button(Find.ByName("btnG")).Click();
            }

            //using(var browser = new FireFox("google.com"))
            //{
            //    browser.TextField(Find.ByName("q")).TypeText("demo");
            //    browser.Button(Find.ByName("btnG")).Click();
            //}
        }
    }
}