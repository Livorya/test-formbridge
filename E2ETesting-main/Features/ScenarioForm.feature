Feature: Fill out form as a customer

    Scenario: Navigate to company selector page
        Given I am on the startpage
        When I click the "[Demo] Customer Forms" link
        Then I am redirected to the company selector page

    Scenario: Select company and navigate to form page
        Given I am on the company selector page
        When I select the "Komplett" option in the dropdown
        And I click the "Pick" button
        Then I am redirected to the form page

    Scenario: Fill out form
        Given I am on the form page
        When I enter "Firstname" as the firstname
        And I enter "Lastname" as the lastname
        And I enter "email@email.email" as the email
        And I check the "product" radiobutton
        And I enter "Testing Playwright" as the message
        And I click the "Submit" button to get an alert
        Then I should see an alert with the text "Thank you for submitting your inquiry!"

