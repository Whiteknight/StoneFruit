Feature: Verbs

Rule: Verb which doesn't exist causes error

    Scenario: Verb does not exist
        When I run headless with input "does-not-exist"
        Then The output should contain at least:
            | Line                           |
            | Verb does-not-exist not found. Please check your spelling or help output and try again. |
