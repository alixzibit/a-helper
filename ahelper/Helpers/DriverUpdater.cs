//using OpenQA.Selenium;
//using OpenQA.Selenium.Edge;
//using System;
//using System.Windows;
//using System.IO;


//namespace ahelper.Helpers
//{
//    public class DriverUpdater
//    {
//        //automate the process of fetching the driver link TODO for future use
//        public static void FetchDriverLink()
//        {
//            string driverPath = AppDomain.CurrentDomain.BaseDirectory;
//            // Setup WebDriver for Edge
//            EdgeOptions options = new EdgeOptions();
//            // Run headless 
//            options.AddArgument("--headless");
//            EdgeDriverService service = EdgeDriverService.CreateDefaultService(driverPath, "msedgedriver.exe");
//            IWebDriver driver = new EdgeDriver(service, options);

//            try
//            {
//                // Navigate to the AMD driver download page
//                driver.Navigate()
//                    .GoToUrl(
//                        "https://www.amd.com/en/support/downloads/drivers.html/processors/ryzen/ryzen-7000-series/amd-ryzen-7-7840u.html");

//                // Allow some time for the page to load completely
//                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

//                // Find the download link for the Windows 11 driver
//                IWebElement downloadLink = driver.FindElement(By.PartialLinkText("win11"));

//                // Display the link
//                MessageBox.Show("Download link: " + downloadLink.GetAttribute("href"));
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error occurred: " + ex.Message);
//            }
//            finally
//            {
//                driver.Quit(); // Clean up and close the browser
//            }

//        }
//    }
//}
