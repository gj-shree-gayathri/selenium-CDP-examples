namespace SeleniumCDP
{
  using OpenQA.Selenium;
  using OpenQA.Selenium.Chrome;
  using OpenQA.Selenium.DevTools;
  using OpenQA.Selenium.Support.UI;
  using OpenQA.Selenium.DevTools.V128.Network;
  using OpenQA.Selenium.DevTools.V128.Performance;

  using DevToolsSessionDomains = OpenQA.Selenium.DevTools.V128.DevToolsSessionDomains;
  using System.Net;

    public class SampleExample
    {
        ChromeDriver driver;
        [SetUp]
        public void Setup()
        {

            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            var devTools = driver as IDevTools;
            //driver.Navigate().GoToUrl("https://blog.executeautomation.com/");
            /*var options = new ChromeOptions();
            options.AddArgument("--auto-open-devtools-for-tabs");*/

        }
       [Test]
        public async Task Consolelog()
        {
            driver.Url = "https://www.selenium.dev/selenium/web/bidi/logEntryAdded.html";

            using IJavaScriptEngine monitor = new JavaScriptEngine(driver);
            var messages = new List<string>();
            monitor.JavaScriptConsoleApiCalled += (_, e) =>
            {
                messages.Add(e.MessageContent);
            };
            await monitor.StartEventMonitoring();

            driver.FindElement(By.Id("consoleLog")).Click();
            driver.FindElement(By.Id("consoleError")).Click();
            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(_ => messages.Count > 1);
            monitor.StopEventMonitoring();

            Assert.IsTrue(messages.Contains("Hello, world!"));
            Assert.IsTrue(messages.Contains("I am console error"));
        }
        [Test]
        public async Task SetCookie()
        {
            var session = ((IDevTools)driver).GetDevToolsSession();
            var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V128.DevToolsSessionDomains>();
            await domains.Network.Enable(new OpenQA.Selenium.DevTools.V128.Network.EnableCommandSettings());

            var cookieCommandSettings = new SetCookieCommandSettings
            {
                Name = "cheese",
                Value = "gouda",
                Domain = "www.selenium.dev",
                Secure = true
            };
            await domains.Network.SetCookie(cookieCommandSettings);

            driver.Url = "https://www.selenium.dev";
            OpenQA.Selenium.Cookie cheese = driver.Manage().Cookies.GetCookieNamed("cheese");
            Assert.AreEqual("gouda", cheese.Value);
        }

        [Test]
        public async Task PerformanceMetrics()
        {
            driver.Url = "https://www.selenium.dev/selenium/web/frameset.html";

            var session = ((IDevTools)driver).GetDevToolsSession();
      var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V128.DevToolsSessionDomains>(); // Update to V128

      // var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V127.DevToolsSessionDomains>();

      await domains.Performance.Enable(new OpenQA.Selenium.DevTools.V128.Performance.EnableCommandSettings());
            var metricsResponse =
                await session.SendCommand<GetMetricsCommandSettings, GetMetricsCommandResponse>(
                    new GetMetricsCommandSettings()
                );

            var metrics = metricsResponse.Metrics.ToDictionary(
                dict => dict.Name,
                dict => dict.Value
            );

            Assert.IsTrue(metrics["DevToolsCommandDuration"] > 0);
            Assert.AreEqual(12, metrics["Frames"]);
        }
    /*[Test]
    public async Task ResponseCode()

    {
      // Navigate to a webpage

      var session = ((IDevTools)driver).GetDevToolsSession();
      await Task.Delay(5000);
      // Enable the Network domain
      var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V128.DevToolsSessionDomains>();
      await domains.Network.Enable(new OpenQA.Selenium.DevTools.V128.Network.EnableCommandSettings()); //v128->browser version
      // Initialize list to store network responses
      networkResponses = new List<ResponseData>();

      // Subscribe to ResponseReceived event
      domains.Network.ResponseReceived += (sender, e) =>
      {
        networkResponses.Add(new ResponseData
        {
          Url = e.Response.Url,
          Status = e.Response.Status,
          StatusText = e.Response.StatusText
        });
      };
      driver.Navigate().GoToUrl("https://example.com");
      // Wait for network responses to be captured
      await Task.Delay(5000);

      // Assert specific network responses
      var exampleResponse = networkResponses.Find(r => r.Url == "https://example.com/");
      Assert.IsNotNull(exampleResponse, "Response for https://example.com/ not found.");
      Assert.AreEqual(200, exampleResponse.Status, $"Expected status code 200, but got {exampleResponse.Status}.");
      Assert.AreEqual("OK", exampleResponse.StatusText, $"Expected status text 'OK', but got '{exampleResponse.StatusText}'.");

      // Example: Assert all responses have status code 200
      foreach (var response in networkResponses)
      {
        Assert.AreEqual(200, response.Status, $"Unexpected status code {response.Status} for URL: {response.Url}");
      }
    

    // Subscribe to the ResponseReceived event
    domains.Network.ResponseReceived += (sender, e) =>
      {
        Console.WriteLine($"URL: {e.Response.Url}, Status: {e.Response.Status}, StatusText: {e.Response.StatusText}");
      };



      // Wait for a few seconds to capture network responses


    }*/

    [TearDown]
        public void TearDown()
        {
            driver.Dispose();
        }
    }
}
