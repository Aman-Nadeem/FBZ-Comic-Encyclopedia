using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ComicApp.Tests
{
    [Collection("Sequential")]
    public class SeleniumTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl = "https://localhost:7167";

        public SeleniumTests()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        public void Dispose()
        {
            _driver.Quit();
        }

        // HOME PAGE LOADS
        [Fact]
        public void ComicsPage_Loads_Successfully()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics");
            Assert.Contains("Comics", _driver.Title);
        }

        // BASIC SEARCH
        [Fact]
        public void BasicSearch_ValidTitle_ReturnsResults()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics");

            var searchBox = _driver.FindElement(By.Name("title"));
            searchBox.SendKeys("Batman");

            var searchButton = _driver.FindElement(By.CssSelector("form[action*='Search'] button"));
            searchButton.Click();

            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Total comics loaded", body);
        }

        // LOGIN PAGE LOADS
        [Fact]
        public void LoginPage_Loads_Successfully()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Account/Login");
            Assert.Contains("Login", _driver.Title);
        }

        // REGISTER PAGE LOADS
        [Fact]
        public void RegisterPage_Loads_Successfully()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Account/Register");
            Assert.Contains("Register", _driver.Title);
        }

        // INVALID LOGIN
        [Fact]
        public void Login_InvalidCredentials_ShowsError()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Account/Login");

            _driver.FindElement(By.Name("username")).SendKeys("fakeuser");
            _driver.FindElement(By.Name("password")).SendKeys("wrongpassword");
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElement(By.TagName("body")));

            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Invalid username or password", body);
        }

        // REPORTS REDIRECTS PUBLIC USERS
        [Fact]
        public void Reports_PublicUser_RedirectsAway()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics/Reports");
            Assert.DoesNotContain("Reports", _driver.Url.Contains("Reports") ? "Reports" : "");
        }

        // REGISTER NEW USER
        [Fact]
        public void Register_NewUser_RedirectsToLogin()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Account/Register");

            string uniqueUser = $"testuser{Guid.NewGuid().ToString().Substring(0, 5)}";

            _driver.FindElement(By.Name("username")).SendKeys(uniqueUser);
            _driver.FindElement(By.Name("password")).SendKeys("password123");

            var roleDropdown = new SelectElement(_driver.FindElement(By.Name("role")));
            roleDropdown.SelectByValue("Public");

            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            Assert.Contains("Login", _driver.Url);
        }

        // SORT A-Z
        [Fact]
        public void Sort_AZ_LoadsSuccessfully()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics");

            var sortButton = _driver.FindElement(
                By.CssSelector("button[value='asc']"));
            sortButton.Click();

            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Total comics loaded", body);
        }

        // SORT Z-A
        [Fact]
        public void Sort_ZA_LoadsSuccessfully()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics");

            var sortButton = _driver.FindElement(
                By.CssSelector("button[value='desc']"));
            sortButton.Click();

            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Total comics loaded", body);
        }

        // GROUP BY AUTHOR
        [Fact]
        public void GroupByAuthor_LoadsSuccessfully()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics");

            var groupButton = _driver.FindElement(
                By.CssSelector("form[action*='GroupByAuthor'] button"));
            groupButton.Click();

            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Author", body);
        }

        // GROUP BY YEAR
        [Fact]
        public void GroupByYear_LoadsSuccessfully()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics");

            _driver.FindElement(
                By.CssSelector("form[action*='GroupByYear'] button")).Click();

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            wait.Until(d => d.FindElement(By.TagName("body")));

            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Year", body);
        }

        // GENRE FILTER
        [Fact]
        public void GenreFilter_Fantasy_ReturnsResults()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics");

            var genreDropdown = new SelectElement(
                _driver.FindElement(By.CssSelector("form[action*='FilterByGenre'] select[name='genre']")));
            genreDropdown.SelectByValue("Fantasy");

            _driver.FindElement(By.CssSelector("form[action*='FilterByGenre'] button")).Click();

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.TagName("body")));

            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Total comics loaded", body);
        }

        // ADVANCED SEARCH
        [Fact]
        public void AdvancedSearch_ByTitle_ReturnsResults()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics");

            _driver.FindElement(By.CssSelector("form[action*='AdvancedSearch'] input[name='title']"))
                .SendKeys("Batman");

            _driver.FindElement(By.CssSelector("form[action*='AdvancedSearch'] button")).Click();

            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Total comics loaded", body);
        }

        // STAFF LOGIN AND ACCESS REPORTS
        [Fact]
        public void StaffUser_CanAccessReports()
        {
            string staffUser = $"staff{Guid.NewGuid().ToString().Substring(0, 5)}";

            _driver.Navigate().GoToUrl($"{_baseUrl}/Account/Register");
            _driver.FindElement(By.Name("username")).SendKeys(staffUser);
            _driver.FindElement(By.Name("password")).SendKeys("password123");
            var roleDropdown = new SelectElement(_driver.FindElement(By.Name("role")));
            roleDropdown.SelectByValue("Staff");
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.Url.Contains("Login"));

            _driver.FindElement(By.Name("username")).SendKeys(staffUser);
            _driver.FindElement(By.Name("password")).SendKeys("password123");
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            wait.Until(d => d.Url.Contains("Comics"));

            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics/Reports");

            wait.Until(d => d.FindElement(By.TagName("body")));
            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Reports", body);
        }

        // SAVED SEARCHES PAGE LOADS FOR LOGGED IN USER
        [Fact]
        public void SavedSearches_LoggedInUser_PageLoads()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Account/Register");

            string uniqueUser = $"testuser{Guid.NewGuid().ToString().Substring(0, 5)}";

            _driver.FindElement(By.Name("username")).SendKeys(uniqueUser);
            _driver.FindElement(By.Name("password")).SendKeys("password123");

            var roleDropdown = new SelectElement(_driver.FindElement(By.Name("role")));
            roleDropdown.SelectByValue("Public");

            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.Url.Contains("Login"));

            _driver.FindElement(By.Name("username")).SendKeys(uniqueUser);
            _driver.FindElement(By.Name("password")).SendKeys("password123");
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            wait.Until(d => d.Url.Contains("Comics"));

            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics/SavedSearches");

            wait.Until(d => d.FindElement(By.TagName("body")));
            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Saved", body);
        }

        // COMICVINE SEARCH PAGE LOADS
        [Fact]
        public void ComicVinePage_Loads_Successfully()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/ComicVine/Search");
            Assert.Contains("ComicVine", _driver.Title);
        }

        // COMICVINE SEARCH RETURNS RESULTS
        [Fact]
        public void ComicVineSearch_ValidQuery_ReturnsResults()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/ComicVine/Search?query=batman");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            wait.Until(d => d.FindElement(By.TagName("body")));

            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Batman", body);
        }

        // OPEN LIBRARY SEARCH PAGE LOADS
        [Fact]
        public void OpenLibraryPage_Loads_Successfully()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/OpenLibrary/Search");
            Assert.Contains("Open Library", _driver.Title);
        }

        // OPEN LIBRARY SEARCH RETURNS RESULTS
        [Fact]
        public void OpenLibrarySearch_ValidQuery_ReturnsResults()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/OpenLibrary/Search?query=batman");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            wait.Until(d => d.FindElement(By.TagName("body")));

            var body = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Batman", body);
        }

        // NAV CONTAINS COMICVINE LINK
        [Fact]
        public void Navigation_ContainsComicVineLink()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}");
            var nav = _driver.FindElement(By.TagName("nav")).Text;
            Assert.Contains("ComicVine", nav);
        }

        // NAV CONTAINS OPEN LIBRARY LINK
        [Fact]
        public void Navigation_ContainsOpenLibraryLink()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}");
            var nav = _driver.FindElement(By.TagName("nav")).Text;
            Assert.Contains("Open Library", nav);
        }
    }
}