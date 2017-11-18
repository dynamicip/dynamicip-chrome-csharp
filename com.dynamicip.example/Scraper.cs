using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace com.dynamicip.example
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Running web scraper...");

                using (var driver = CreateDriver())
                {
                    // Load a javascript-enabled page.
                    driver.Navigate().GoToUrl("https://examples.dynamicip.com/single-page-apps/basic");

                    // Wait for javascript. This particular solution is specific to jQuery.
                    WaitForJQuery(driver);

                    // Extract the DOM.
                    var renderedHTML = driver.ExecuteScript("return document.documentElement.outerHTML");
                    
                    // Display the result.
                    Console.WriteLine("Page response:");
                    Console.WriteLine(renderedHTML);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static ChromeDriver CreateDriver()
        {
            var options = new ChromeOptions();

            // Configure Chrome to use DynamicIP as a proxy.
            options.AddArgument("--proxy-server=https://dynamicip.dom:443");

            // Perform proxy authentication via a custom plugin (see 'scripts/chrome_extension' dir).
            options.AddArgument($"--load-extension={Path.Combine(Directory.GetCurrentDirectory(), "scripts", "chrome_extension")}");
            
            // ChromeDriver's default behaviour is to allow invalid certificates.
            // To improve security, we explicitly unset this flag here.
            options.AddExcludedArgument("ignore-certificate-errors");

            return new ChromeDriver(options);
        }

        private static void WaitForJQuery(IJavaScriptExecutor driver)
        {
            try
            {
                var jsTimeoutSecs = 30;
                for (var i = 0; i < jsTimeoutSecs; i++)
                {
                    var isJQueryComplete = (bool)driver.ExecuteScript("return jQuery.active === 0");
                    if (isJQueryComplete) return;
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load page. Problem may be temporary - please try again.", ex);
            }
        }
    }
}
