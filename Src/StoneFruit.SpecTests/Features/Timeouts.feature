Feature: Timeouts

Scenario: Async handler will timeout gracefully
    Given I use the AsyncTimeout handler
    And I set the MaxExecuteTimeout setting to "00:00:01"
    When I run headless with input "async-timeout"
    Then The output should contain at least:
        | Line    |
        | started |
        | stopped |