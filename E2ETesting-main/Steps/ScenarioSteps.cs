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
    
    // IDialog for handling alert, saved so the logic follows Gherkin
    private IDialog _lastDialog;

    [BeforeScenario]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 500 });
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
        // have two different fields (Login & Form) that uses email input field with different selectors
        // now I have one method for both (this works because they are on separate pages)
        var emailField = _page.Locator("input[name='email'], input#email");
        await emailField.First.FillAsync(email);
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
    }

    [When(@"I click the ""(.*)"" button")]
    public async Task WhenIClickTheButton(string button)
    {
        // different buttons have different selectors and I want one method to handle all cases
        var selectedButton = _page.Locator($"input[value=\"{button}\"], button:has-text(\"{button}\")");
        await selectedButton.First.ClickAsync();
    }

    [Then(@"I am redirected to the form page")]
    public async Task ThenIAmRedirectedToTheFormPage()
    {
        await _page.WaitForURLAsync("http://localhost:5173/form/2");
        var title = await _page.InnerTextAsync("div > h1");
        Assert.Equal("Komplett form", title);
    }

    [Given(@"I am on the form page")]
    public async Task GivenIAmOnTheFormPage()
    {
        await _page.GotoAsync("http://localhost:5173/form/2");
    }

    [When(@"I enter ""(.*)"" as the firstname")]
    public async Task WhenIEnterAsTheFirstname(string firstname)
    {
        await _page.FillAsync("input[id=firstname]", firstname);
    }

    [When(@"I enter ""(.*)"" as the lastname")]
    public async Task WhenIEnterAsTheLastname(string lastname)
    {
        await _page.FillAsync("input[id=lastname]", lastname);
    }

    [When(@"I check the ""(.*)"" radiobutton")]
    public async Task WhenICheckTheRadiobutton(string radioOption)
    {
        await _page.CheckAsync($"input[type='radio'][value=\"{radioOption.ToUpper()}\"]");
    }

    [When(@"I enter ""(.*)"" as the message")]
    public async Task WhenIEnterAsTheMessage(string message)
    {
        await _page.FillAsync("#message", message);
    }

    [When(@"I click the ""(.*)"" button to get an alert")]
    public async Task WhenIClickTheButtonToGetAnAlert(string button)
    {
        // Prepare to capture the alert
        var tcs = new TaskCompletionSource<IDialog>();

        _page.Dialog += (_, dialog) =>
        {
            _lastDialog = dialog;
            tcs.SetResult(dialog);
        };

        // Click the button that triggers the alert
        await _page.ClickAsync($"input[value=\"{button}\"]");

        // Wait for the alert to appear
        await tcs.Task;
    }

    [Then(@"I should see an alert with the text ""(.*)""")]
    public async Task  ThenIShouldSeeAnAlertWithTheText(string expectedText)
    {
        Assert.NotNull(_lastDialog);
        Assert.Contains(expectedText, _lastDialog.Message);

        // Close the alert
        await _lastDialog.AcceptAsync();
    }

    [Given(@"I am on the chat page")]
    public async Task GivenIAmOnTheChatPage()
    {
        await _page.GotoAsync("http://localhost:5173/chat/14");
    }

    [When(@"I enter ""(.*)"" as the chat message")]
    public async Task WhenIEnterAsTheChatMessage(string message)
    {
        await _page.FillAsync("input", message);
    }

    [When(@"I click the send icon")]
    public async Task WhenIClickTheSendIcon()
    {
        await _page.Locator("img.sendbutton").ClickAsync();
    }

    [Then(@"I should see my message ""(.*)"" in the chat")]
    public async Task ThenIShouldSeeMyMessageInTheChat(string message)
    {
        var successMessage = _page.Locator($"li > div:has-text(\"{message}\")").First;
        // Wait for it to appear
        await successMessage.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible, 
            Timeout = 5000
        });

        var text = await successMessage.InnerTextAsync();
        Assert.NotNull(text);
        Assert.Equal(message, text);
        
        // confirm that the list item has customer styling
    }
}
