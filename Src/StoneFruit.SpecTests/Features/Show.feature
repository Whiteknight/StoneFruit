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

Scenario: Show metadata
    When I run headless with input "_show metadata"
    Then The output should be empty
