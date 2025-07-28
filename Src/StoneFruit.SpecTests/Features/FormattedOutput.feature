Feature: FormattedOutput

Scenario: Can write formatted output no tags
    Given I use the FormatHandler
    When I run headless with input "format 'this is a test'"
    Then The output should contain:
        | Line           |
        | this is a test |

Scenario: Format pipeline one property
    Given I use the FormatHandler
    When I run headless with input "format '{{.Prop1}}'"
    Then The output should contain:
        | Line  |
        | prop1 |

Scenario: Format pipeline one property between text
    Given I use the FormatHandler
    When I run headless with input "format 'a {{.Prop1}} b'"
    Then The output should contain:
        | Line      |
        | a prop1 b |

Scenario: Format pipeline nested properties
    Given I use the FormatHandler
    When I run headless with input "format '{{.Prop2.Prop2_1}}'"
    Then The output should contain:
        | Line    |
        | prop2_1 |

Scenario: Format pipeline if then success
    Given I use the FormatHandler
    When I run headless with input "format '{{if .Prop1}}ok{{end}}'"
    Then The output should contain:
        | Line |
        | ok   |

Scenario: Format pipeline if then fail
    Given I use the FormatHandler
    When I run headless with input "format '{{if .PropX}}ok{{end}} b'"
    Then The output should contain:
        | Line |
        | b    |

Scenario: Format pipeline if then else success
    Given I use the FormatHandler
    When I run headless with input "format '{{if .Prop1}}ok{{else}}fail{{end}}'"
    Then The output should contain:
        | Line |
        | ok   |

Scenario: Format pipeline if then else fail
    Given I use the FormatHandler
    When I run headless with input "format '{{if .PropX}}ok{{else}}fail{{end}}'"
    Then The output should contain:
        | Line |
        | fail |