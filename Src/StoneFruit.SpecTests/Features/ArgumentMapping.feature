Feature: ArgumentMapping


Scenario: I can map positional arguments to object properties
    Given I use the PositionalMapping handler
    When I run headless with input "positional mapping a b c"
    Then The output should contain:
        | Line |
        | a    |
        | b    |
        | c    |
