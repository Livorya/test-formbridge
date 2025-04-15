Feature: Login

    Scenario: Login as admin
        Given I am on the login page
        When I enter "admin1" as the email
        And I enter "a" as the password
        And I click the button
        Then I should redirect to the admin-dashboard

    Scenario: Login as support
        Given I am on the login page
        When I enter "support1" as the email
        And I enter "a" as the password
        And I click the button
        Then I should redirect to the support-dashboard
