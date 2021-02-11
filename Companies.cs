using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace CompanyCollector
{
    public class Companies
    {
        private ChromeOptions _options;
        private IWebDriver _driver;
        private DelLogger _logger;
        public Companies(DelLogger logger)
        {
            //Initialize options for Driver
            _options = new ChromeOptions();
            _options.AddArguments(/*"headless",*/"--blink-settings=imagesEnabled=false");

            //Initialize the instance
            _driver = new ChromeDriver(_options);
            _logger = logger;
        }


        public List<string> GetCompanies(int pageAmount)
        {
            string url = "https://www.europages.de/unternehmen/Deutschland/fenster.html";

            var companyList = new List<string>();

            try
            {
                for (int i = 0; i < pageAmount; i++)
                {
                    if (i > 0)
                    {
                        companyList.AddRange(GetCompaniesFromPage($"https://www.europages.de/unternehmen/pg-{i}/Deutschland/fenster.html"));
                    }
                    else
                    {
                        companyList.AddRange(GetCompaniesFromPage(url));
                    }

                    _logger("Proceed page nr:" + i);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                //close the driver instance.
                _driver.Close();

                //quit
                _driver.Quit();
            }

            return companyList;

        }

        private List<string> GetCompaniesFromPage(string url)
        {
            //launch gmail.com
            _driver.Navigate().GoToUrl(url);

            //maximize the browser
            //_driver.Manage().Window.Minimize();

            //find the element by xpath and enter the email address which you want to login.
            //driver.FindElement(By.XPath("//input[@aria-label='Email or phone']")).SendKeys("email adress);

            //wait for a seconds
            Task.Delay(1000).Wait();

            //find the Next Button and click on it.
            
            var pages = _driver.FindElements(By.XPath("//a[contains(@class,'company-name')]"));

             List<string> lst = new List<string>();

            foreach(var a in pages)
            {
                var temp = _driver.CurrentWindowHandle;
                string link = a.GetAttribute("href");
                ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
                _driver.SwitchTo().Window(_driver.WindowHandles[1]);
                _driver.Navigate().GoToUrl(link);
                Task.Delay(100).Wait();
                lst.Add(string.Join(",", GetDataPerCompany()));
                _driver.Close();
                _driver.SwitchTo().Window(_driver.WindowHandles[0]);
                Task.Delay(100).Wait();
            }          

            return lst;
        }

        private List<string> GetDataPerCompany()
        {
            List<string> line = new List<string>();
            line.Add(_driver.FindElement(By.XPath("//div[contains(@class, 'company-content')]/h3[contains(@itemprop, 'name')]")).Text);
            //line.Add(_driver.FindElement(By.XPath("//h3[contains(@itemprop, 'addressCountry')]")).Text);
            //line.Add(_driver.FindElement(By.XPath("//h3[contains(@itemprop, 'addressLocality')]")).Text);
            return line;
        }

    }
}
