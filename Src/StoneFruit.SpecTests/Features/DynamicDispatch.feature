Feature: DynamicDispatch


Scenario: Can dynamically dispatch by string
    Given I use the DynamicDispatch handlers
    When I run headless with input "dynamic-dispatch type=string"
    Then The output should contain:
        | Line          |
        | Invoked: test |

Scenario: Can dynamically dispatch by verb and arguments
    Given I use the DynamicDispatch handlers
    When I run headless with input "dynamic-dispatch type=verbargs value"
    Then The output should contain:
        | Line           |
        | Invoked: value |

Scenario: Can dynamically dispatch by arguments
    Given I use the DynamicDispatch handlers
    When I run headless with input "dynamic-dispatch type=args value"
    Then The output should contain:
        | Line           |
        | Invoked: value |

Scenario: Can dynamically dispatch async by string
    Given I use the DynamicDispatch handlers
    When I run headless with input "dynamic-dispatch-async type=string"
    Then The output should contain:
        | Line          |
        | Invoked: test |

Scenario: Can dynamically dispatch async by verb and arguments
    Given I use the DynamicDispatch handlers
    When I run headless with input "dynamic-dispatch-async type=verbargs value"
    Then The output should contain:
        | Line           |
        | Invoked: value |

Scenario: Can dynamically dispatch async by arguments
    Given I use the DynamicDispatch handlers
    When I run headless with input "dynamic-dispatch-async type=args value"
    Then The output should contain:
        | Line           |
        | Invoked: value |
