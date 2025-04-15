Feature: Fill out form as a customer

    Scenario: Navigate to company selector page
        Given I am on the startpage
        When I click the "[Demo] Customer Forms" link
        Then I am redirected to the company selector page

    Scenario: Select company and navigate to form page
        Given I am on the company selector page
        When I select the "Komplett" option in the dropdown
        And I click the "pick" button
        Then I am redirected to the form page
