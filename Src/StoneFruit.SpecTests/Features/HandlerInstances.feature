Feature: HandlerInstances

Scenario: Use a predefined handler instance
    Given I use a predefined handler instance
    When I run headless with input "predefined"
    Then The output should contain:
        | Line     |
        | Executed |

Scenario: Use a predefined handler instance async
    Given I use a predefined handler instance
    When I run headless with input "predefinedasync"
    Then The output should contain:
        | Line     |
        | Executed |


