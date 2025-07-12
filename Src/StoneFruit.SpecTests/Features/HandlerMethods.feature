Feature: HandlerMethods

Rule: Sync methods

    Scenario: Can invoke a simple handler method
        Given I use ObjectWithHandlerMethod handlers
        When I run headless with input "simple-method"
        Then The output should contain:
            | Line   |
            | Simple |

    Scenario: Can invoke a handler method with named argument
        Given I use ObjectWithHandlerMethod handlers
        When I run headless with input "method-with-one-named-arg name=value"
        Then The output should contain:
            | Line         |
            | Named: value |

    Scenario: Can invoke a handler method with positional argument
        Given I use ObjectWithHandlerMethod handlers
        When I run headless with input "method-with-one-named-arg value"
        Then The output should contain:
            | Line         |
            | Named: value |

    Scenario: Can invoke a handler method with positional argument2
        Given I use ObjectWithHandlerMethod handlers
        When I run headless with input "method-with-one-named-arg2 value"
        Then The output should contain:
            | Line         |
            | Named: value |

    Scenario: Can invoke a handler method with flag argument true
        Given I use ObjectWithHandlerMethod handlers
        When I run headless with input "method-with-one-flag-arg -flag"
        Then The output should contain:
            | Line       |
            | Flag: True |

    Scenario: Can invoke a handler method with flag argument false
        Given I use ObjectWithHandlerMethod handlers
        When I run headless with input "method-with-one-flag-arg"
        Then The output should contain:
            | Line        |
            | Flag: False |

Rule: Async methods

    Scenario: Can invoke a simple handler method async
        Given I use ObjectWithHandlerMethod handlers
        When I run headless with input "simple-method-async"
        Then The output should contain:
            | Line   |
            | Simple |

    Scenario: Can invoke a handler method with named argument async
        Given I use ObjectWithHandlerMethod handlers
        When I run headless with input "method-with-one-named-arg-async name=value"
        Then The output should contain:
            | Line         |
            | Named: value |