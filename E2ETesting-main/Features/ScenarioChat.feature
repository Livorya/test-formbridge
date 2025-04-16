Feature: Chat

Scenario: Write chat message as customer
    Given I am on the chat page
    When I enter "Customer new test" as the chat message
    And I click the send icon
    Then I should see my message "Customer new test" in the chat

Scenario: Give a rating on chat as customer
    Given I am on the chat page
    When I click the 2 star
    Then the 2 stars should be selected
