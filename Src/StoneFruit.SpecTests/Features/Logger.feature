Feature: Logger

Scenario: Can write log messages to output
    Given I use the LoggingHandler
    When I run headless with input "logging Information test"
    Then The output should contain:
        | Line |
        | test |
