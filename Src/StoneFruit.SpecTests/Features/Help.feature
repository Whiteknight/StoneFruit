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

Rule: Instance Methods in Sections

    Scenario: Instance methods sections help
        Given I use ObjectWithHandlerMethod handlers in section "methods"
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group   | Verb                                    |
            | methods | methods simple-method                   |
            | methods | methods method-with-one-flag-arg        |
            | methods | methods method-with-one-named-arg       |
            | methods | methods simple-method-async             |
            | methods | methods method-with-one-named-arg-async |

Rule: Scripts

    Scenario: Scripts help
        Given I have a script "test-script" with lines:
            | Line      |
            | echo test |
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group | Verb        |
            |       | test-script |

    Scenario: Scripts in section help
        Given I have a script "test-script" in section "section" with lines:
            | Line      |
            | echo test |
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group   | Verb        |
            | section | section test-script |