Feature: Chat

Scenario: Write chat message as customer
    Given I am on the chat page
    When I enter "Customer new test" as the chat message
    And I click the send icon
    Then I should see my message "Customer new test" in the chat

