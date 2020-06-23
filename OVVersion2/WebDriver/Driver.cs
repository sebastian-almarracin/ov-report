using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace OVVersion2.WebDriver
{
    public static class Driver
    {
        public static readonly string _locationDrivers = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string _downloadDir = @"ChromeDownloadDirectory";
        public static IWebDriver Instance { get; set; }


        public static void InitializeChrome()
        {
            DirectoryInfo di = new DirectoryInfo(_downloadDir);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--incognito");
            options.AddArgument("--verbose");
            options.AddArgument("--no-sandbox");
            options.AddUserProfilePreference("download.default_directory", _downloadDir);
            Instance = new ChromeDriver(_locationDrivers, options, TimeSpan.FromMinutes(2));
        }

        public static void GoToUrl(string url)
        {
            Instance.Navigate().GoToUrl(url);
            Thread.Sleep(1000);
        }

        public static IWebElement GetElementById(string id)
        {
            return Instance.FindElement(By.Id(id));
        }

        public static IWebElement GetElementByClass(string className)
        {
            return Instance.FindElement(By.ClassName(className));
        }

        public static List<IWebElement> GetElementsByClass(string className)
        {
            return Instance.FindElements(By.ClassName(className)).ToList();
        }
        public static List<IWebElement> GetElementsByTags(string tag)
        {
            return Instance.FindElements(By.TagName(tag)).ToList();
        }

        public static List<IWebElement> FindChildrenByTagName(this IWebElement element, string tag)
        {
            return element.FindElements(By.TagName(tag)).ToList();
        }
        public static List<IWebElement> FindChildrenByClassName(this IWebElement element, string className)
        {
            return element.FindElements(By.ClassName(className)).ToList();
        }

        public static void ClickElementById(string id)
        {
            GetElementById(id).Click();
            WaitForLoad();
        }
        public static void JsClickById(string id)
        {
            var command = $"document.getElementById(\"{id}\").click();";
            ((IJavaScriptExecutor)Instance).ExecuteScript(command);
        }

        public static void WaitForLoad()
        {
            Thread.Sleep(1000);
        }

        public static void ScrollToElementById(string id)
        {
            var element = GetElementById(id);
            ((IJavaScriptExecutor)Instance).ExecuteScript("arguments[0].scrollIntoView(false);", element);

        }

        public static void ScrollToElementByWebElement(IWebElement element)
        {
            ((IJavaScriptExecutor)Instance).ExecuteScript("arguments[0].scrollIntoView(false);", element);
        } 
    }
}
