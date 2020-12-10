// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.PowerApps.TestAutomation.Browser;
using Microsoft.PowerApps.TestAutomation.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using OpenQA.Selenium;
using System.Data.SqlClient;
using System.Threading;

namespace Microsoft.PowerApps.TestAutomation.Tests
{
    [TestClass]
    public class Proposal
    {
        private static string _username = "";
        private static string _password = "";
        private static BrowserType _browserType;
        private static Uri _xrmUri;
        private static Uri _testAutomationUri;
        private static string _loginMethod;
        private static string _resultsDirectory = "";
        private static string _driversPath = "";
        private static string _usePrivateMode;
        private static string _testAutomationURLFilePath = "";
        private static int _globalTestCount = 0;
        private static int _globalPassCount = 0;
        private static int _globalFailCount = 0;
        private static int _testMaxWaitTimeInSeconds = 600;

        public TestContext TestContext { get; set; }
        private static TestContext _testContext;

        [ClassInitialize]
        public static void Initialize(TestContext TestContext)
        {
            _testContext = TestContext;

            _username = _testContext.Properties["OnlineUsername"].ToString();
            _password = _testContext.Properties["OnlinePassword"].ToString();
            _xrmUri = new Uri(_testContext.Properties["OnlineUrl"].ToString());
            _loginMethod = _testContext.Properties["LoginMethod"].ToString();
            _resultsDirectory = _testContext.Properties["ResultsDirectory"].ToString();
            _browserType = (BrowserType)Enum.Parse(typeof(BrowserType), _testContext.Properties["BrowserType"].ToString());
            _driversPath = _testContext.Properties["DriversPath"].ToString();
            _usePrivateMode = _testContext.Properties["UsePrivateMode"].ToString();
            _testAutomationURLFilePath = _testContext.Properties["TestAutomationURLFilePath"].ToString();
            _testMaxWaitTimeInSeconds = Convert.ToInt16(_testContext.Properties["TestMaxWaitTimeInSeconds"]);
        }

        [TestCategory("PowerAppsTestAutomation")]
        [Priority(1)]
        [TestMethod]
        public void Test1_Create_New_Proposal()
        {
            BrowserOptions options = RunTestSettings.Options;
            options.BrowserType = _browserType;
            options.DriversPath = _driversPath;
            
            var ProgrammeCountBefore = ExcecuteSQLCommand("SELECT COUNT(programme_id) AS row_count FROM [dbo].programmes");
            Console.WriteLine("Starting No. of Programmes:");
            Console.WriteLine(ProgrammeCountBefore);

            using (var appBrowser = new PowerAppBrowser(options))
            {
                RunTest(appBrowser, 0);
                
                var ProgrammeCountAfter = ExcecuteSQLCommand("SELECT COUNT(programme_id) AS row_count FROM [dbo].programmes");
                Console.WriteLine("Final No. of Programmes:");
                Console.WriteLine(ProgrammeCountAfter);

                if (_globalPassCount > 0 && _globalFailCount > 0)
                {
                    string message = ("\n"
                        + "Inconclusive Test Automation Result: " + "\n"
                        + $"Total Test Count: {_globalTestCount}" + "\n"
                        + $"Total Pass Count: {_globalPassCount}" + "\n"
                        + $"Total Fail Count: {_globalFailCount}" + "\n"
                        + "Please see the console log for more information.");
                    Assert.Fail(message);
                }
                else if (_globalFailCount > 0)
                {
                    string message = ("\n"
                        + "Test Failed: " + "\n"
                        + $"Total Fail Count: {_globalFailCount}" + "\n"
                        + "Please see the console log for more information.");
                    Assert.Fail(message);
                }
                else if (Int32.Parse(ProgrammeCountBefore) >= Int32.Parse(ProgrammeCountAfter))
                {
                    string message = ("\n"
                            + "Test Failed: " + "\n"
                            + $"Total Fail Count: {_globalFailCount}" + "\n"
                            + "New event not created in DB");
                    Assert.Fail(message);
                }
                else if (_globalPassCount > 0)
                {
                    var success = true;
                    string message = ("\n"
                        + "Success: " + "\n"
                        + $"Total Pass Count: {_globalPassCount}");
                    Assert.IsTrue(success, message);

                }
                
            }
        }

        [TestCategory("PowerAppsTestAutomation")]
        [Priority(1)]
        [TestMethod]
        public void Test2_Complete_A2()
        {
            BrowserOptions options = RunTestSettings.Options;
            options.BrowserType = _browserType;
            options.DriversPath = _driversPath;
            using (var appBrowser = new PowerAppBrowser(options))
            {
                Debug.WriteLine("Test Started");
                RunTest(appBrowser, 1);
                Debug.WriteLine("UI Test Ran");
                Debug.WriteLine("Pass Count:" + _globalPassCount);
                Debug.WriteLine("Fail Count:" + _globalFailCount);
                if (_globalPassCount > 0 && _globalFailCount > 0)
                {
                    string message = ("\n"
                        + "Inconclusive Test Automation Result: " + "\n"
                        + $"Total Test Count: {_globalTestCount}" + "\n"
                        + $"Total Pass Count: {_globalPassCount}" + "\n"
                        + $"Total Fail Count: {_globalFailCount}" + "\n"
                        + "Please see the console log for more information.");
                    Assert.Fail(message);
                }
                else if (_globalFailCount > 0)
                {
                    string message = ("\n"
                        + "Test Failed: " + "\n"
                        + $"Total Fail Count: {_globalFailCount}" + "\n"
                        + "Please see the console log for more information.");
                    Assert.Fail(message);
                } else if (_globalPassCount > 0)
                {
                    var EventInstanceStatus = ExcecuteSQLCommand("SELECT top 1 processing_status FROM [dbo].event_instances WHERE event_key = 'complete-the-a2-form' order by created_on desc").Replace("\n", "").Replace("\r", "");
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    while (EventInstanceStatus != "complete"  && stopWatch.ElapsedMilliseconds <= 900000)
                    {
                        EventInstanceStatus = ExcecuteSQLCommand("SELECT top 1 processing_status FROM [dbo].event_instances WHERE event_key = 'complete-the-a2-form' order by created_on desc").Replace("\n", "").Replace("\r", "");
                        Debug.WriteLine("'" + EventInstanceStatus + "'");
                        Thread.Sleep(10000);
                    }
                    if (EventInstanceStatus == "complete")
                    {
                        var success = true;
                        string message = ("\nSuccess: \n" + $"Total Pass Count: {_globalPassCount}");
                        Assert.IsTrue(success, message);
                    }
                    else
                    {
                        string message = ("\nTest Failed: " + "\n" + $"Total Fail Count: {_globalFailCount}" + "\nEvent creation timed out - Please see the console log for more information.");
                        Assert.Fail(message);
                    }
                }

            }
        }

        [TestCategory("PowerAppsTestAutomation")]
        [Priority(1)]
        [TestMethod]
        public void Test3_Complete_Desiscion()
        {
            BrowserOptions options = RunTestSettings.Options;
            options.BrowserType = _browserType;
            options.DriversPath = _driversPath;
            using (var appBrowser = new PowerAppBrowser(options))
            {
                RunTest(appBrowser, 2);

                if (_globalPassCount > 0 && _globalFailCount > 0)
                {
                    string message = ("\n"
                        + "Inconclusive Test Automation Result: " + "\n"
                        + $"Total Test Count: {_globalTestCount}" + "\n"
                        + $"Total Pass Count: {_globalPassCount}" + "\n"
                        + $"Total Fail Count: {_globalFailCount}" + "\n"
                        + "Please see the console log for more information.");
                    Assert.Fail(message);
                }
                else if (_globalFailCount > 0)
                {
                    string message = ("\n"
                        + "Test Failed: " + "\n"
                        + $"Total Fail Count: {_globalFailCount}" + "\n"
                        + "Please see the console log for more information.");
                    Assert.Fail(message);
                }
                else if (_globalPassCount > 0)
                {
                    var success = true;
                    string message = ("\n"
                        + "Success: " + "\n"
                        + $"Total Pass Count: {_globalPassCount}");
                    Assert.IsTrue(success, message);


                }
            }
        }

        public String ExcecuteSQLCommand(String SQLCommand)
        {
            var SQLResponse = "";
            using (SqlConnection connection = new SqlConnection("Data Source=kingston-uni-curriculummgn-srv.database.windows.net;Initial Catalog=curriculum-management;Persist Security Info=True;User ID=dbadmin;Password=M3atB4ll5."))
            using (SqlCommand cmd = new SqlCommand(SQLCommand, connection))
            {
               connection.Open();
               using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            SQLResponse += reader.GetValue(i) + "\n";
                        }
                    }
                }
            }
            return SQLResponse;
        }

        public void RunTest(PowerAppBrowser appBrowser, int testRunCounter)
        {
            _globalPassCount = 0;
            _globalFailCount = 0;
            // Track list of  Test Automation URLs
            var testUrlList = appBrowser.TestAutomation.GetTestURLs(_testAutomationURLFilePath);
            // Track total number of TestURLs
            int testUrlCount = testUrlList.Value.Count();

            _testAutomationUri = testUrlList.Value[testRunCounter];

                try
                {
                    
                        //Login To PowerApps
                        Debug.WriteLine($"Attempting to authenticate to Maker Portal: {_xrmUri}");

                        for (int retryCount = 0; retryCount < Reference.Login.SignInAttempts; retryCount++)
                        {
                            try
                            {
                                // See Authentication Types: https://docs.microsoft.com/en-us/office365/servicedescriptions/office-365-platform-service-description/user-account-management#authentication
                                // CloudIdentity uses standard Office 365 sign-in service
                                if (_loginMethod == "CloudIdentity")
                                {
                                    appBrowser.OnlineLogin.Login(_xrmUri, _username.ToSecureString(), _password.ToSecureString());
                                    break;
                                }
                                // FederatedIdentity uses AD FS 2.0 or other Security Token Services
                                else if (_loginMethod == "FederatedIdentity")
                                {
                                    // Do Federated Login
                                    appBrowser.OnlineLogin.Login(_xrmUri, _username.ToSecureString(), _password.ToSecureString(), FederatedLoginAction);
                                    break;
                                }
                                // FederatedIdentity scenario -- but DevOps agent is configured with SSO capability
                                else if (_loginMethod == "PassThrough")
                                {
                                    appBrowser.OnlineLogin.Login(_xrmUri);
                                    break;
                                }
                                // Fallback to CloudIdentity experience if _loginMethod is not provided
                                else
                                {
                                    appBrowser.OnlineLogin.Login(_xrmUri, _username.ToSecureString(), _password.ToSecureString());
                                    break;
                                }

                            }
                            catch (Exception exc)
                            {
                                Console.WriteLine($"Exception on Attempt #{retryCount + 1}: {exc}");

                                if (retryCount + 1 == Reference.Login.SignInAttempts)
                                {
                                    // Login exception occurred, take screenshot
                                    _resultsDirectory = TestContext.TestResultsDirectory;
                                    string location = $@"{_resultsDirectory}\RunTestAutomation-LoginErrorAttempt{retryCount + 1}.jpeg";

                                    appBrowser.TakeWindowScreenShot(location, OpenQA.Selenium.ScreenshotImageFormat.Jpeg);
                                    _testContext.AddResultFile(location);

                                    // Max Sign-In Attempts reached
                                    Console.WriteLine($"Login failed after {retryCount + 1} attempts.");
                                    throw new InvalidOperationException($"Login failed after {retryCount + 1} attempts. Exception Details: {exc}");
                                }
                                else
                                {
                                    // Login exception occurred, take screenshot
                                    _resultsDirectory = TestContext.TestResultsDirectory;
                                    string location = $@"{_resultsDirectory}\RunTestAutomation-LoginErrorAttempt{retryCount + 1}.jpeg";

                                    appBrowser.TakeWindowScreenShot(location, OpenQA.Selenium.ScreenshotImageFormat.Jpeg);
                                    _testContext.AddResultFile(location);

                                    //Navigate away and retry
                                    appBrowser.Navigate("about:blank");

                                    Console.WriteLine($"Login failed after attempt #{retryCount + 1}.");
                                    continue;
                                }
                            }
                        }

                    Console.WriteLine($"Power Apps  Test Automation Execution Starting Test #{testRunCounter} of {testUrlCount}");

                    // Initialize TestFrameworok results JSON object
                    JObject testAutomationResults = new JObject();

                    // Execute TestAutomation and return JSON result object
                    testAutomationResults = appBrowser.TestAutomation.ExecuteTestAutomation(_testAutomationUri, testRunCounter, _testMaxWaitTimeInSeconds);

#if DEBUG
                    // Only output post execution screenshot in debug mode
                    _resultsDirectory = TestContext.TestResultsDirectory;
                    string location1 = $@"{_resultsDirectory}\TestRun{testRunCounter}-PostExecutionScreenshot.jpeg";
                    appBrowser.TakeWindowScreenShot(location1, OpenQA.Selenium.ScreenshotImageFormat.Jpeg);
                    _testContext.AddResultFile(location1);
#endif

                    // Report Results to DevOps Pipeline                    
                    var testResultCount = appBrowser.TestAutomation.ReportResultsToDevOps(testAutomationResults, testRunCounter);

                    _globalPassCount += testResultCount.Item1;
                    _globalFailCount += testResultCount.Item2;
                    _globalTestCount += (testResultCount.Item1 + testResultCount.Item2);

                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occurred during Test Run #{testRunCounter} of {testUrlCount}: {e}");

                    _resultsDirectory = TestContext.TestResultsDirectory;
                    Console.WriteLine($"Current results directory location: {_resultsDirectory}");
                    string location = $@"{_resultsDirectory}\TestRun{testRunCounter}-GenericError.jpeg";

                    appBrowser.TakeWindowScreenShot(location, OpenQA.Selenium.ScreenshotImageFormat.Jpeg);
                    _testContext.AddResultFile(location);

                    throw;
                }

                Console.WriteLine($"Power Apps  Test Automation Execution Completed for Test Run #{testRunCounter} of {testUrlCount}." + "\n");
           

        }

        public void FederatedLoginAction(LoginRedirectEventArgs args)
        {
            // Login Page details go here.  
            // You will need to find out the id of the password field on the form as well as the submit button. 
            // You will also need to add a reference to the Selenium Webdriver to use the base driver. 
            // Example

            var driver = args.Driver;

            var passwordInput = driver.WaitUntilAvailable(By.Id("passwordInput"));
            passwordInput.SendKeys(args.Password.ToUnsecureString());

            driver.ClickWhenAvailable(By.Id("submitButton"), TimeSpan.FromSeconds(5));

            // Insert any additional code as required for the SSO scenario


            // Wait for Maker Portal Page to load
            driver.WaitUntilVisible(By.XPath(Elements.Xpath[Reference.Login.MainPage])
                , new TimeSpan(0, 2, 0), 
                e =>
                {
                    try
                    {
                        e.WaitUntilVisible(By.ClassName("apps-list"), new TimeSpan(0, 0, 30));
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("The Maker Portal Apps List did not return visible.");
                        throw new InvalidOperationException($"The Maker Portal Apps List did not return visible.: {exc}");
                    }

                    e.WaitForPageToLoad();
                }, 
                f =>
                {
                    Console.WriteLine("Login.MainPage failed to load in 2 minutes using Federated Identity Login.");
                    throw new Exception("Login page failed using Federated Identity Login.");
                });
        }
    }
}
