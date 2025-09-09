Feature: Headless

Rule: Headless start and stop events execute

    Scenario: I can change headless start and stop events
        Given I set the EngineStartHeadless script to:
            | Line       |
            | echo start |
        And I set the EngineStopHeadless script to:
            | Line      |
            | echo stop |
        When I run headless with input "echo 'test'"
        Then The output should contain:
            | Line  |
            | start |
            | test  |
            | stop  |

Rule: Headless help
    
    Scenario: Headless help event runs
        Given I set the HeadlessHelp script to:
            | Line               |
            | echo headless help |
        When I run headless with input "help"
        Then The output should contain:
            | Line          |
            | headless help |
        And The exit code is 0

Rule: Headless unhandled exception

   Scenario: Headless unhandled exception event runs
        Given I use the ThrowExceptionHandler
        When I run headless with input "throw exception"
        Then The exit code is 1

Rule: Headless unhandled exception

   Scenario: Headless no verb
        Given I use the ThrowExceptionHandler
        When I run headless with input ""
        Then The exit code is 2