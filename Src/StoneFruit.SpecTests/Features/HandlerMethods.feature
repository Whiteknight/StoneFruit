Feature: HandlerMethods

Rule: Sync methods

    Background: 
        Given I use ObjectWithHandlerMethod handlers with value "test"

    Scenario: Can invoke a simple handler method
        When I run headless with input "simple-method"
        Then The output should contain:
            | Line        |
            | test Simple |

    Scenario: Can invoke a handler method with named argument
        When I run headless with input "method-with-one-named-arg name=value"
        Then The output should contain:
            | Line              |
            | test Named: value |

    Scenario: Can invoke a handler method with positional argument
        When I run headless with input "method-with-one-named-arg value"
        Then The output should contain:
            | Line              |
            | test Named: value |

    Scenario: Can invoke a handler method with positional argument2
        When I run headless with input "method-with-one-named-arg2 value"
        Then The output should contain:
            | Line              |
            | test Named: value |

    Scenario: Can invoke a handler method with flag argument true
        When I run headless with input "method-with-one-flag-arg -flag"
        Then The output should contain:
            | Line            |
            | test Flag: True |

    Scenario: Can invoke a handler method with flag argument false
        When I run headless with input "method-with-one-flag-arg"
        Then The output should contain:
            | Line             |
            | test Flag: False |

    Scenario: Can invoke an injected-dependency handler method
        When I run headless with input "injected-method"
        Then The output should contain:
            | Line          |
            | test Injected |

Rule: Async methods

    Background: 
        Given I use ObjectWithHandlerMethod handlers with value "test"

    Scenario: Can invoke a simple handler method async
        When I run headless with input "simple-method-async"
        Then The output should contain:
            | Line        |
            | test Simple |

    Scenario: Can invoke a handler method with named argument async
        When I run headless with input "method-with-one-named-arg-async name=value"
        Then The output should contain:
            | Line              |
            | test Named: value | 

Rule: Sections

    Background: 
        Given I use ObjectWithHandlerMethod handlers in section "test"

    Scenario: Can invoke a simple handler method in a section
        When I run headless with input "test simple-method"
        Then The output should contain:
            | Line        |
            | test Simple |

    Scenario: Can invoke a simple handler method async in a section
        When I run headless with input "test simple-method-async"
        Then The output should contain:
            | Line        |
            | test Simple |

    Scenario: Can invoke an injected-dependency handler method in a section
        When I run headless with input "test injected-method"
        Then The output should contain:
            | Line          |
            | test Injected |

Rule: Value Type Parsing

    Scenario: Can parse positional path as FileInfo
        Given I use the value type parsing handler methods
        When I run headless with input "as file info 'path/to/file.txt'"
        Then The output should contain:
            | Line |
            | True |

    Scenario: Can parse named path as FileInfo
        Given I use the value type parsing handler methods
        When I run headless with input "as file info file='path/to/file.txt'"
        Then The output should contain:
            | Line |
            | True |

    Scenario: Can parse positional path as Guid
        Given I use the value type parsing handler methods
        When I run headless with input "as guid '12345678-1234-5678-9000-123456789000'"
        Then The output should contain:
            | Line |
            | True |

    Scenario: Can parse named path as Guid
        Given I use the value type parsing handler methods
        When I run headless with input "as guid guid='12345678-1234-5678-9000-123456789000'"
        Then The output should contain:
            | Line |
            | True |
