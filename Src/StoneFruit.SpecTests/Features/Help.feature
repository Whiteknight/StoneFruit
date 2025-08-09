Feature: Help

Rule: List Built-In Verbs

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

Rule: List Instance Methods

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

Rule: List Instance Methods in Sections

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

    Scenario: Instance methods sections help starts with 1
        Given I use ObjectWithHandlerMethod handlers in section "methods"
        When I run headless with input "help -startswith methods"
        Then The help output should contain these verbs:
            | Group   | Verb                                    |
            | methods | methods simple-method                   |
            | methods | methods method-with-one-flag-arg        |
            | methods | methods method-with-one-named-arg       |
            | methods | methods simple-method-async             |
            | methods | methods method-with-one-named-arg-async |

    Scenario: Instance methods sections help starts with 2
        Given I use ObjectWithHandlerMethod handlers in section "methods"
        When I run headless with input "help -startswith methods simple"
        Then The help output should contain these verbs:
            | Group   | Verb                                    |
            | methods | methods simple-method                   |
            | methods | methods simple-method-async             |

Rule: List Scripts

    Scenario: Scripts help list
        Given I have a script "test-script" with lines:
            | Line      |
            | echo test |
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group | Verb        |
            |       | test-script |

    Scenario: Scripts in section help list
        Given I have a script "test-script" in section "section" with lines:
            | Line      |
            | echo test |
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group   | Verb                |
            | section | section test-script |

Rule: List Delegates

    Scenario: Delegate handler
        Given I register a simple delegate handler "test"
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group | Verb |
            |       | test |

    Scenario: Delegate handler in section
        Given I register a simple delegate handler "test" in section "delegate"
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group    | Verb          |
            | delegate | delegate test |

    Scenario: Async delegate handler
        Given I register a simple async delegate handler "test"
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group | Verb |
            |       | test |

    Scenario: Async delegate handler in section
        Given I register a simple async delegate handler "test" in section "delegate"
        When I run headless with input "help"
        Then The help output should contain these verbs:
            | Group    | Verb          |
            | delegate | delegate test |

Rule: Details

    Scenario: Script help details
        Given I have a script "test-script" with:
            | Field       | Value                   |
            | Line        | echo test               |
            | Description | test script description |
            | Usage       | test script usage       |
            | Group       |                         |
        When I run headless with input "help test-script"
        Then The output should contain:
            | Line              |
            | test script usage |

    Scenario: Help help
        When I run headless with input "help help"
        Then The output should contain at least:
            | Line                                              |
            | help [ -showall ]                                 |
            | Get overview information for all available verbs. |
