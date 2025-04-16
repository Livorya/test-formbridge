Feature: Admin

Scenario: Add a user as an admin
    Given I am logged in as "admin1"
    And I am on the admin dashboard page
    When I click the "Add User" button in the menu
    And I enter "Test" as the firstname
    And I enter "Test" as the lastname
    And I enter "gui@test.test" as the email
    And I click the checkmark button
    Then I should see the new user with the email "gui@test.test"
    