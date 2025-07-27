Feature: HandlerMethods

Rule: Sync methods

    Scenario: Can invoke a simple handler method
        Given I use ObjectWithHandlerMethod handlers with value "test"
        When I run headless with input "simple-method"
        Then The output should contain:
            | Line        |
            | test Simple |

    Scenario: Can invoke a handler method with named argument
        Given I use ObjectWithHandlerMethod handlers with value "test"
        When I run headless with input "method-with-one-named-arg name=value"
        Then The output should contain:
            | Line              |
            | test Named: value |

    Scenario: Can invoke a handler method with positional argument
        Given I use ObjectWithHandlerMethod handlers with value "test"
        When I run headless with input "method-with-one-named-arg value"
        Then The output should contain:
            | Line              |
            | test Named: value |

    Scenario: Can invoke a handler method with positional argument2
        Given I use ObjectWithHandlerMethod handlers with value "test"
        When I run headless with input "method-with-one-named-arg2 value"
        Then The output should contain:
            | Line              |
            | test Named: value |

    Scenario: Can invoke a handler method with flag argument true
        Given I use ObjectWithHandlerMethod handlers with value "test"
        When I run headless with input "method-with-one-flag-arg -flag"
        Then The output should contain:
            | Line            |
            | test Flag: True |

    Scenario: Can invoke a handler method with flag argument false
        Given I use ObjectWithHandlerMethod handlers with value "test"
        When I run headless with input "method-with-one-flag-arg"
        Then The output should contain:
            | Line             |
            | test Flag: False |

Rule: Async methods

    Scenario: Can invoke a simple handler method async
        Given I use ObjectWithHandlerMethod handlers with value "test"
        When I run headless with input "simple-method-async"
        Then The output should contain:
            | Line        |
            | test Simple |

    Scenario: Can invoke a handler method with named argument async
        Given I use ObjectWithHandlerMethod handlers with value "test"
        When I run headless with input "method-with-one-named-arg-async name=value"
        Then The output should contain:
            | Line              |
            | test Named: value | 

Rule: Sections

    Scenario: Can invoke a simple handler method in a section
        Given I use ObjectWithHandlerMethod handlers in section "test"
        When I run headless with input "test simple-method"
        Then The output should contain:
            | Line        |
            | test Simple |

    Scenario: Can invoke a simple handler method async in a section
        Given I use ObjectWithHandlerMethod handlers in section "test"
        When I run headless with input "test simple-method-async"
        Then The output should contain:
            | Line        |
            | test Simple |

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
