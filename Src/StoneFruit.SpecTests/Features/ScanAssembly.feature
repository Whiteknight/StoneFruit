Feature: ScanAssembly

Rule: Without Prefixes

    Scenario: Scan an assembly
        Given I scan the assembly
        When I run headless with input "scanned"
        Then The output should contain:
            | Line    |
            | Scanned |

    Scenario: Scan the current assembly
        Given I scan the current assembly
        When I run headless with input "scanned"
        Then The output should contain:
            | Line    |
            | Scanned |

    Scenario: Scan assembly containing type
        Given I scan the assembly containing the scanned handler type
        When I run headless with input "scanned"
        Then The output should contain:
            | Line    |
            | Scanned |

Rule: With Prefixes

    Scenario: Scan an assembly prefixed
        Given I scan the assembly with prefix 'prefix'
        When I run headless with input "prefix scanned"
        Then The output should contain:
            | Line    |
            | Scanned |

    Scenario: Scan the current assembly prefixed
        Given I scan the current assembly with prefix 'prefix'
        When I run headless with input "prefix scanned"
        Then The output should contain:
            | Line    |
            | Scanned |

    Scenario: Scan assembly containing type prefixed
        Given I scan the assembly containing the scanned handler type with prefix 'prefix'
        When I run headless with input "prefix scanned"
        Then The output should contain:
            | Line    |
            | Scanned |

