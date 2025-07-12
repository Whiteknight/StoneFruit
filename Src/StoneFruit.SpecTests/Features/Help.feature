Feature: Help

Rule: Built-In Verbs

    Scenario: Basic help
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group          | Verb |
            | Built-In Verbs | env  |
            | Built-In Verbs | exit |
            | Built-In Verbs | help |

    Scenario: Basic help show all
        When I run headless with input "help -showall"
        Then The help output should contain these verbs:
            | Group          | Verb      |
            | Built-In Verbs | _args     |
            | Built-In Verbs | echo      |
            | Built-In Verbs | env       |
            | Built-In Verbs | exit      |
            | Built-In Verbs | quit      |
            | Built-In Verbs | help      |
            | Built-In Verbs | _metadata |
            | Built-In Verbs | _show     |

Rule: Instance Methods

    Scenario: Instance methods help
        Given I use ObjectWithHandlerMethod handlers
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group | Verb                            |
            |       | simple-method                   |
            |       | method-with-one-flag-arg        |
            |       | method-with-one-named-arg       |
            |       | simple-method-async             |
            |       | method-with-one-named-arg-async |

    Scenario: Instance methods help camel case
        Given I use ObjectWithHandlerMethod handlers
        And I use the CamelCaseVerbExtractor
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group | Verb                            |
            |       | simple method                   |
            |       | method with one flag arg        |
            |       | method with one named arg       |
            |       | simple method async             |
            |       | method with one named arg async |
    
    Scenario: Instance methods help lower case
        Given I use ObjectWithHandlerMethod handlers
        And I use the ToLowerNameVerbExtractor
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group | Verb                       |
            |       | simplemethod               |
            |       | methodwithoneflagarg       |
            |       | methodwithonenamedarg      |
            |       | simplemethodasync          |
            |       | methodwithonenamedargasync |
