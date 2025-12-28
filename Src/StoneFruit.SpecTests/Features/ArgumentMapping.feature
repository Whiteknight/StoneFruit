Feature: ArgumentMapping

Rule: Can map simple string and int property values by name and position

    Scenario: I can map positional arguments to object properties
        Given I use the ArgumentMapping handlers
        When I run headless with input "simple map to object a 2 c"
        Then The output should contain:
            | Line  |
            | a     |
            | 2     |
            | c     |
            | False |

    Scenario: I can map positional arguments to object properties with flag
        Given I use the ArgumentMapping handlers
        When I run headless with input "simple map to object a 2 c -Flag1"
        Then The output should contain:
            | Line |
            | a    |
            | 2    |
            | c    |
            | True |

    Scenario: I can map positional arguments to object properties with named as bool
        Given I use the ArgumentMapping handlers
        When I run headless with input "simple map to object a 2 c Flag1=true"
        Then The output should contain:
            | Line |
            | a    |
            | 2    |
            | c    |
            | True |

    Scenario: I can map named arguments to object properties
        Given I use the ArgumentMapping handlers
        When I run headless with input "simple map to object first=a second=2 third=c"
        Then The output should contain:
            | Line  |
            | a     |
            | 2     |
            | c     |
            | False |

    Scenario: I can map named arguments to object properties with flag
        Given I use the ArgumentMapping handlers
        When I run headless with input "simple map to object first=a second=2 third=c -Flag1"
        Then The output should contain:
            | Line |
            | a    |
            | 2    |
            | c    |
            | True |

Rule: Can use IValueTypeParser to map complex objects

    Scenario: I can map positional arguments to mapped types
        Given I use the ArgumentMapping handlers
        When I run headless with input "complex map to object '/path/to/file.txt' '/directory/name' '12345678-1234-5678-9000-123456789000'"
        Then The output should contain:
            | Line                   |
            | As FileInfo: True      |
            | As DirectoryInfo: True |
            | As Guid: True          |

    Scenario: I can map named arguments to mapped types
        Given I use the ArgumentMapping handlers
        When I run headless with input "complex map to object first='/path/to/file.txt' second='/directory/name' third='12345678-1234-5678-9000-123456789000'"
        Then The output should contain:
            | Line                   |
            | As FileInfo: True      |
            | As DirectoryInfo: True |
            | As Guid: True          |

Rule: I can use AddHandlerArgumentType to map custom argument types

    Scenario: I can map custom argument type
        Given I use the ArgumentMapping handlers
        When I run headless with input "type mapped object 'a' 1 'c'"
        Then The output should contain:
            | Line  |
            | a     |
            | 1     |
            | c     |
            | False |

    Scenario: Required properties must be provided
        Given I use the ArgumentMapping handlers
        When I run headless with input "type mapped object"
        Then The output should contain:
            | Line                                                  |
            | Missing required argument values for properties: Arg1 |