using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V128.Network;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using System;
using System.Threading.Tasks;

public class ResponseCode
{
  ChromeDriver driver;
  //setup
  [SetUp]
  public void Setup()
  {
    new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig());
    driver = new ChromeDriver();

    driver.Manage().Window.Maximize();
    var devTools = driver as IDevTools;
    //driver.Navigate().GoToUrl("https://blog.executeautomation.com/");
    /*var options = new ChromeOptions();
    options.AddArgument("--auto-open-devtools-for-tabs");*/

  }
  [Test]
  public async Task ResponsereceiveTests()

    {
    if (driver == null)
    {
      throw new Exception("WebDriver is not initialized!");
    }
    var session = ((IDevTools)driver).GetDevToolsSession();
      var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V128.DevToolsSessionDomains>();
      await domains.Network.Enable(new OpenQA.Selenium.DevTools.V128.Network.EnableCommandSettings());


      // Enable the Network domain

      domains.Network.RequestWillBeSent += (sender, e) =>
      {
        Console.WriteLine($"URL sent: {e.Request.Url}");
      };

      // Subscribe to the ResponseReceived event
      domains.Network.ResponseReceived += (sender, e) =>
      {
        Console.WriteLine($"URL received: {e.Response.Url}, Status: {e.Response.Status}, StatusText: {e.Response.StatusText}");
      };

      // Navigate to a webpage
      driver.Navigate().GoToUrl("https://example.com");

      // Wait for a few seconds to capture network responses
      await Task.Delay(5000);

      // Close the browser

    }
  [TearDown]
  public void TearDown()
  {
    driver.Dispose();
    driver.Quit();
  }

  }
