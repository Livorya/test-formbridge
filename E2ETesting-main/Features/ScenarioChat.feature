Feature: Chat

Scenario: Write chat message as customer
    Given I am on the chat page
    When I enter "Customer Message" as the chat message
    And I click the send icon
    Then I should see my message "Customer Message" in the chat with customer styling

Scenario: Give a rating on chat as customer
    Given I am on the chat page
    When I click the 2 star
    Then I should see the 2 stars selected

Scenario: Enter chat as support
    Given I am logged in as "support2"
    And I am on the support dashboard page
    When I click the chat icon for the row with the email "email@email.email"
    Then I should redirect to the chat page for customer with "Firstname Lastname" name 

Scenario: Write chat message as support
    Given I am logged in as "support2"
    And I am on the chat page
    When I enter "Support Message" as the chat message
    And I click the send icon
    Then I should see my message "Support Message" in the chat with support styling

Scenario: Resolve chat as support
    Given I am logged in as "support2"
    And I am on the support dashboard page
    When I click the resolve icon for the row with the email "email@email.email" and message "Testing Playwrigh"
    Then I should see the status icon as "resolved" for the row with the email "email@email.email" and message "Testing Playwrigh"

