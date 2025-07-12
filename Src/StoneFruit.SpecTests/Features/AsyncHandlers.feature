Feature: AsyncHandlers

Scenario: I can call an async handler
    Given I Use the SimpleAsync handler
    When I run headless with input "simple async"
    Then The output should contain:
        | Line                 |
        | Simple async invoked |