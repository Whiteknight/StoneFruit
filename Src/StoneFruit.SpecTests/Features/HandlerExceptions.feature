Feature: HandlerExceptions


Scenario: Handler throws an exception
    Given I use the ThrowExceptionHandler
    When I run headless with input "throw exception"
    Then The output should contain at least:
        | Line             |
        | Exception thrown |

Scenario: EngineError script changes error handling
    Given I use the ThrowExceptionHandler
    And I set the EngineError script to:
        | Line                |
        | echo Error Received |
    When I run headless with input "throw exception"
    Then The output should contain at least:
        | Line           |
        | Error Received |
