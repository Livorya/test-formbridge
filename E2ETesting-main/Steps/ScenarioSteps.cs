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
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 400 });
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
    
    [When(@"I click the button")]
    public async Task WhenIClickTheButton()
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

    [Then(@"I should redirect to the support-dashboard")]
    public async Task ThenIShouldRedirectToTheSupportDashboard()
    {
        await _page.WaitForURLAsync("http://localhost:5173/support");
        var title = await _page.InnerTextAsync("h2");
        Assert.Equal("Support Dashboard", title);
    }

    [Given(@"I am on the startpage")]
    public async Task GivenIAmOnTheStartpage()
    {
        await _page.GotoAsync("http://localhost:5173");
    }

    [When(@"I click the ""(.*)"" link")]
    public async Task WhenIClickTheLink(string link)
    {
        await _page.ClickAsync($"Span:Text(\"{link}\")");
    }

    [Then(@"I am redirected to the company selector page")]
    public async Task ThenIAmRedirectedToTheCompanySelectorPage()
    {
        await _page.WaitForURLAsync("http://localhost:5173/customer");
        var title = await _page.InnerTextAsync("h1");
        Assert.Equal("Choose Company", title);
    }

    [Given(@"I am on the company selector page")]
    public async Task GivenIAmOnTheCompanySelectorPage()
    {
        await _page.GotoAsync("http://localhost:5173/customer");
    }

    [When(@"I select the ""(.*)"" option in the dropdown")]
    public async Task WhenISelectTheOptionInTheDropdown(string selectedOption)
    {
        //await _page.FillAsync("option:has-text(\"{selectedOption}\")", selectedOption);
        //await _page.SelectOptionAsync(".company-select", new SelectOptionValue { Label = selectedOption });
        //await _page.SelectOptionAsync(".company-select", new[] { "2" });
        
        var options = _page.Locator("select.company-select option");
        int count = await options.CountAsync();

        string? valueToSelect = null;

        for (int i = 0; i < count; i++)
        {
            var text = await options.Nth(i).InnerTextAsync();
            if (text.Contains(selectedOption))
            {
                valueToSelect = await options.Nth(i).GetAttributeAsync("value");
                break;
            }
        }

        if (valueToSelect != null)
        {
            await _page.SelectOptionAsync(".company-select", new[] { valueToSelect });
        }
        else
        {
            Console.WriteLine($"Option containing text '{selectedOption}' not found.");
        }
    }

    [When(@"I click the ""(.*)"" button")]
    public async Task WhenIClickTheButton(string button)
    {
        await _page.ClickAsync($"button:Text(\"{button}\")");
    }

    [Then(@"I am redirected to the form page")]
    public async Task ThenIAmRedirectedToTheFormPage()
    {
        await _page.WaitForURLAsync("http://localhost:5173/form/2");
        var title = await _page.InnerTextAsync("div > h1");
        Assert.Equal("Komplett form", title);
    }
}
