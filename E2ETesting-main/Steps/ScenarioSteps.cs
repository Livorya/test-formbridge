namespace E2ETesting.Steps;

using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;

[Binding]
public class ScenarioSteps
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;

    [BeforeScenario]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 200 });
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    [AfterScenario]
    public async Task Teardown()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    [Given(@"I am on the login page")]
    public async Task GivenIAmOnTheLoginPage()
    {
        await _page.GotoAsync("http://localhost:5173/login");
    }

    [When(@"I enter ""(.*)"" as the email")]
    public async Task WhenIEnterAsTheEmail(string email)
    {
        await _page.FillAsync("input[name='email']", email);
    }

    [When(@"I enter ""(.*)"" as the password")]
    public async Task WhenIEnterAsThePassword(string password)
    {
        await _page.FillAsync("input[name='password']", password);
    }
    
    [When(@"I click the sign in button")]
    public async Task WhenIClickTheSignInButton()
    {
        await _page.ClickAsync("button");
    }

    [Then(@"I should redirect to the admin-dashboard")]
    public async Task ThenIShouldRedirectToTheAdminDashboard()
    {
        await _page.WaitForURLAsync("http://localhost:5173/admin");
        var title = await _page.InnerTextAsync("h2");
        Assert.Equal("Admin Dashboard", title);
    }
}
