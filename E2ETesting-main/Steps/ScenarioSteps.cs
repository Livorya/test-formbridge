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
        // have three different fields (Login & Form & Add User) that uses email input field with different selectors
        // now I have one method for all (this works because they are all on separate pages)
        var emailField = _page.Locator("input[name='email'], input#email, input[placeholder='Email']");
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
        // have two different fields (Form & Add User) that uses firstname input field with different selectors
        // now I have one method for all (this works because they are all on separate pages)
        var nameField = _page.Locator("input[id=firstname], input[placeholder='First name']");
        await nameField.First.FillAsync(firstname);
    }

    [When(@"I enter ""(.*)"" as the lastname")]
    public async Task WhenIEnterAsTheLastname(string lastname)
    {
        // have two different fields (Form & Add User) that uses firstname input field with different selectors
        // now I have one method for all (this works because they are all on separate pages)
        var nameField = _page.Locator("input[id=lastname], input[placeholder='Last name']");
        await nameField.First.FillAsync(lastname);
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
        await _page.GotoAsync("http://localhost:5173/chat/9");
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

    [Then(@"I should see my message ""(.*)"" in the chat with customer styling")]
    public async Task ThenIShouldSeeMyMessageInTheChatWithCustomerStyling(string message)
    {
        // I only check for the first occurrence of the message and the idea is to enter a unique message
        var successMessage = _page.Locator($"li > div:has-text(\"{message}\")").First;
        // Wait for it to appear
        await successMessage.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible, 
            Timeout = 5000
        });
        // need the parent li element to check if th message has customer styling
        var liOfMessage = successMessage.Locator("..");
        var classList = await liOfMessage.GetAttributeAsync("class");
        
        var text = await successMessage.InnerTextAsync();
        Assert.NotNull(text);
        Assert.Equal(message, text);
        
        Assert.Contains("customer", classList);
    }

    [When(@"I click the (.*) star")]
    public async Task WhenIClickTheStar(int rating)
    {
        int totalStars = 5;
        int domIndex = totalStars - rating;
        
        var starButton = _page.Locator("div.stars button").Nth(domIndex);
        await starButton.ClickAsync();
    }

    [Then(@"I should see the (.*) stars selected")]
    public async Task ThenIShouldSeeTheStarsSelected(int rating)
    {
        var selectedStars = _page.Locator("div.stars button.star.selected");
        var count = await selectedStars.CountAsync();
        
        int totalStars = 5;
        int domIndex = totalStars - rating;
        var theStar = _page.Locator("div.stars button").Nth(domIndex);
        var classList = await theStar.GetAttributeAsync("class");

        Assert.Equal(rating, count);
        Assert.Contains("selected", classList);
    }
    
    [Given(@"I am logged in as ""(.*)""")]
    public async Task GivenIAmLoggedInAs(string email)
    {
        await GivenIAmOnTheLoginPage();
        await WhenIEnterAsTheEmail(email);
        // all test accounts have the same password
        await WhenIEnterAsThePassword("a");
        await WhenIClickTheButton();
    }

    [Then(@"I should see my message ""(.*)"" in the chat with support styling")]
    public async Task ThenIShouldSeeMyMessageInTheChatWithSupportStyling(string message)
    {
        // I only check for the first occurrence of the message and the idea is to enter a unique message
        var successMessage = _page.Locator($"li > div:has-text(\"{message}\")").First;
        // Wait for it to appear
        await successMessage.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible, 
            Timeout = 5000
        });
        // need the parent li element to check if th message has customer styling
        var liOfMessage = successMessage.Locator("..");
        var classList = await liOfMessage.GetAttributeAsync("class");
        
        var text = await successMessage.InnerTextAsync();
        Assert.NotNull(text);
        Assert.Equal(message, text);
        
        Assert.Contains("support", classList);
    }

    [Given(@"I am on the support dashboard page")]
    public async Task GivenIAmOnTheSupportDashboardPage()
    {
        await _page.GotoAsync("http://localhost:5173/support");
    }

    [When(@"I click the chat icon for the row with the email ""(.*)""")]
    public async Task WhenIClickTheChatIconForTheRowWithTheEmail(string email)
    {
        // there might be more tickets with the same email, so I locate all rows containing the email
        var matchingRows = _page.Locator("table tr", new PageLocatorOptions { HasTextString = email });
        // and selects the first one for this test
        // var firstRow = matchingRows.Nth(0);
        
        // and select the last one for this test
        int totalMatches = await matchingRows.CountAsync();
        var lastRow = matchingRows.Nth(totalMatches - 1);

        var chatIcon = lastRow.Locator("img.ticket-chaticon");

        await chatIcon.ClickAsync();
    }

    [Then(@"I should redirect to the chat page for customer with ""(.*)"" name")]
    public async Task ThenIShouldRedirectToTheChatPageForCustomerWithName(string name)
    {
        await _page.WaitForURLAsync("http://localhost:5173/chat/14");
        var welcomeMessage = _page.Locator($"div.text strong:has-text(\"{name}\")");
        var actualText = await welcomeMessage.InnerTextAsync();
        Assert.Equal($"Welcome {name}!", actualText);
    }

    [When(@"I click the resolve icon for the row with the email ""(.*)"" and message ""(.*)""")]
    public async Task WhenIClickTheResolveIconForTheRowWithTheEmailAndMessage(string email, string message)
    {
        // check for both the email and the message because the same customer can have multiple tickets
        var row = _page.Locator("tr", new PageLocatorOptions { HasTextString = email });
        // targets the first occurrence of the email with the corresponding message
        var targetRow = row.Filter(new LocatorFilterOptions { HasTextString = message }).First;
        var resolveIcon = targetRow.Locator("img.ticket-checkicon");
        await resolveIcon.ClickAsync();
    }

    [Then(@"I should see the status icon as ""(.*)"" for the row with the email ""(.*)"" and message ""(.*)""")]
    public async Task ThenIShouldSeeTheStatusIconAsForTheRowWithTheEmailAndMessage(string status, string email, string message)
    {
        // find the row from the last step
        var row = _page.Locator("tr", new PageLocatorOptions { HasTextString = email });
        var targetRow = row.Filter(new LocatorFilterOptions { HasTextString = message }).First;
        
        var statusIcon = targetRow.Locator(".status-icon");
        
        await statusIcon.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 5000
        });
        
        var src = await statusIcon.GetAttributeAsync("src");

        Assert.Contains(status, src);
    }

    [Given(@"I am on the admin dashboard page")]
    public async Task GivenIAmOnTheAdminDashboardPage()
    {
        await _page.GotoAsync("http://localhost:5173/admin");
    }

    [When(@"I click the ""(.*)"" button in the menu")]
    public async Task WhenIClickTheButtonInTheMenu(string buttonName)
    {
        await WhenIClickTheButton(buttonName);
        
        var addUserRow = _page.Locator("tr#add_user");

        await addUserRow.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 5000
        });

        Assert.True(await addUserRow.IsVisibleAsync());
    }
    
    [When(@"I click the checkmark button")]
    public async Task WhenIClickTheCheckmarkButton()
    {
        var button = _page.Locator("tr#add_user > td.table-actions-admin > img");
        await button.ClickAsync();
    }

    [Then(@"I should see the new user with the email ""(.*)""")]
    public async Task ThenIShouldSeeTheNewUserWithTheEmail(string email)
    {
        var emailCell = _page.Locator($"td:has-text(\"{email}\")");
        await emailCell.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 5000
        });
        // assert it's actually found
        Assert.True(await emailCell.IsVisibleAsync());
        // assert that the text found matches the email
        var text = await emailCell.InnerTextAsync();
        Assert.Equal(email, text);
    }
}
