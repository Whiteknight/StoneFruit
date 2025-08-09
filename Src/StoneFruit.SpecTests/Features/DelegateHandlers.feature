Feature: DelegateHandlers

Scenario: Use a simple delegate handler
    Given I register a simple delegate handler "test"
    When I run headless with input "test"
    Then The output should contain:
        | Line          |
        | Invoked: test |

Scenario: Use a simple delegate handler section
    Given I register a simple delegate handler "test" in section "delegate"
    When I run headless with input "delegate test"
    Then The output should contain:
        | Line          |
        | Invoked: test |

Scenario: Use a simple async delegate handler
    Given I register a simple async delegate handler "test"
    When I run headless with input "test"
    Then The output should contain:
        | Line          |
        | Invoked: test |

Scenario: Use a simple async delegate handler section
    Given I register a simple async delegate handler "test" in section "delegate"
    When I run headless with input "delegate test"
    Then The output should contain:
        | Line          |
        | Invoked: test |
