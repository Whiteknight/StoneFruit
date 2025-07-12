Feature: DelegateHandlers

A short summary of the feature

Scenario: Use a simple delegate handler
    Given I register a simple delegate handler "test"
    When I run headless with input "test"
    Then The output should contain:
        | Line          |
        | Invoked: test |

Scenario: Use a simple async delegate handler
    Given I register a simple async delegate handler "test"
    When I run headless with input "test"
    Then The output should contain:
        | Line          |
        | Invoked: test |
