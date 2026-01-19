Feature: Show

Scenario: Show exit codes
    When I run headless with input "_show exitcodes"
    Then The output should contain:
        | Line                   |
        | Ok : 0                 |
        | HeadlessHelp : 0       |
        | UnhandledException : 1 |
        | HeadlessNoVerb : 2     |
        | CascadeError : 3       |
        | MaximumCommands : 4    |
        | Unknown : 100          |

Scenario: Show settings
    When I run headless with input "_show settings"
    Then The output should contain:
        | Line                         |
        | MaxInputlessCommands : 20    |
        | MaxExecuteTimeout : 00:01:00 |

Scenario: Show version
    When I run headless with input "_show version"
    Then The output should not be empty