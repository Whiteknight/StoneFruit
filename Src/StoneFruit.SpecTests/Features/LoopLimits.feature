Feature: LoopLimits


Rule: Headless loop limits

     Scenario: Can change headless loop limit
        Given I have a script "limit-test" with lines:
            | Line   |
            | echo 1 |
            | echo 2 |
            | echo 3 |
            | echo 4 |
        And I set the MaxInputlessCommands setting to 4
        When I run headless with input "limit-test"
        Then The output should contain:
            | Line |
            | 1    |
            | 2    |
            | 3    |
            | 4    |

    Scenario: Can hit modified headless loop limit
        Given I have a script "limit-test" with lines:
            | Line   |
            | echo 1 |
            | echo 2 |
            | echo 3 |
            | echo 4 |
        And I set the MaxInputlessCommands setting to 2
        When I run headless with input "limit-test"
        Then The output should contain:
            | Line    |
            | 1       |
            | 2       |
            | Maximum 2 commands executed without user input. Terminating runloop. |

Rule: Interactive loop limits

    Scenario: Can change interactive loop limit
        Given I have a script "limit-test" with lines:
            | Line   |
            | echo 1 |
            | echo 2 |
            | echo 3 |
            | echo 4 |
        And I set the MaxInputlessCommands setting to 4
        And I clear the EngineStartInteractive script
        And I input the following lines:
            | Line       |
            | limit-test |
        When I run interactively
        Then The output should contain:
            | Line |
            | 1    |
            | 2    |
            | 3    |
            | 4    |

    Scenario: Can hit modified interactive loop limit
        Given I have a script "limit-test" with lines:
            | Line   |
            | echo 1 |
            | echo 2 |
            | echo 3 |
            | echo 4 |
        And I set the MaxInputlessCommands setting to 2
        And I clear the EngineStartInteractive script
        And I input the following lines:
            | Line       |
            | limit-test |
        When I run interactively
        Then The output should contain:
            | Line    |
            | 1       |
            | 2       |