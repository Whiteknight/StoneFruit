Feature: ArgumentMapping


Scenario: I can map positional arguments to object properties
    Given I use the ArgumentMapping handler
    When I run headless with input "argument mapping a b c"
    Then The output should contain:
        | Line  |
        | a     |
        | b     |
        | c     |
        | False |

Scenario: I can map positional arguments to object properties with flag
    Given I use the ArgumentMapping handler
    When I run headless with input "argument mapping a b c -Flag1"
    Then The output should contain:
        | Line |
        | a    |
        | b    |
        | c    |
        | True |

Scenario: I can map named arguments to object properties
    Given I use the ArgumentMapping handler
    When I run headless with input "argument mapping first=a second=b third=c"
    Then The output should contain:
        | Line  |
        | a     |
        | b     |
        | c     |
        | False |

Scenario: I can map named arguments to object properties with flag
    Given I use the ArgumentMapping handler
    When I run headless with input "argument mapping first=a second=b third=c -Flag1"
    Then The output should contain:
        | Line |
        | a    |
        | b    |
        | c    |
        | True |
