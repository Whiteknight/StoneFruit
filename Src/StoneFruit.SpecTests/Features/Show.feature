Feature: Show

Scenario: Show exit codes
    When I run headless with input "_show exitcodes"
    Then The output should contain:
        | Line                |
        | Ok : 0              |
        | HeadlessHelp : 0    |
        | HeadlessNoVerb : 1  |
        | CascadeError : 2    |
        | MaximumCommands : 3 |
        | Unknown : 100       |

Scenario: Show metadata
    When I run headless with input "_show metadata"
    Then The output should be empty
