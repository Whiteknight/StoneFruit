Feature: Script Arguments

A short summary of the feature

Rule: I can parse script arguments

    Scenario: Argument literals
        Given I have a script "test" with lines:
            | Line           |
            | _args a b=c -d |
        When I run headless with input "test x y z"
        Then The output should contain: 
            | Line    |
            | 0: a    |
            | 'b': c  |
            | flag: d |

    Scenario: Positionals
        Given I have a script "test" with lines:
            | Line                    |
            | _args [1] [0] ['x'] [3] |
        When I run headless with input "test a b x=c"
        Then The output should contain: 
            | Line |
            | 0: b |
            | 1: a |
            | 2: c |

    Scenario: Positional with default value
        Given I have a script "test" with lines:
            | Line           |
            | _args [0:test] |
        When I run headless with input "test"
        Then The output should contain: 
            | Line    |
            | 0: test |

    Scenario: Positional with single-quoted default value
        Given I have a script "test" with lines:
            | Line             |
            | _args [0:'test'] |
        When I run headless with input "test"
        Then The output should contain: 
            | Line    |
            | 0: test |

    Scenario: Positional with double-quoted default value
        Given I have a script "test" with lines:
            | Line             |
            | _args [0:"test"] |
        When I run headless with input "test"
        Then The output should contain: 
            | Line    |
            | 0: test |

    Scenario: Positional named with default value
        Given I have a script "test" with lines:
            | Line             |
            | _args ['x':test] |
        When I run headless with input "test"
        Then The output should contain: 
            | Line    |
            | 0: test |

    Scenario: All positionals
        Given I have a script "test" with lines:
            | Line          |
            | _args [1] [*] |
        When I run headless with input "test a b c"
        Then The output should contain: 
            | Line |
            | 0: b |
            | 1: a |
            | 2: c |

    Scenario: Named
        Given I have a script "test" with lines:
            | Line                    |
            | _args a=['b'] {c} d=[0] |
        When I run headless with input "test x b=y c=z"
        Then The output should contain: 
            | Line   |
            | 'a': y |
            | 'c': z |
            | 'd': x |

     Scenario: Named with default value
        Given I have a script "test" with lines:
            | Line           |
            | _args {c:test} |
        When I run headless with input "test"
        Then The output should contain: 
            | Line      |
            | 'c': test |

    Scenario: All named
        Given I have a script "test" with lines:
            | Line          |
            | _args {b} {*} |
        When I run headless with input "test a=x b=y c=z"
        Then The output should contain: 
            | Line   |
            | 'b': y |
            | 'a': x |
            | 'c': z |

    Scenario: Literal name named value with default value
        Given I have a script "test" with lines:
            | Line               |
            | _args a=['b':test] |
        When I run headless with input "test"
        Then The output should contain: 
            | Line      |
            | 'a': test |

    Scenario: Literal name positional value with default value
        Given I have a script "test" with lines:
            | Line             |
            | _args d=[0:test] |
        When I run headless with input "test"
        Then The output should contain: 
            | Line      |
            | 'd': test |

    Scenario: Flags
        Given I have a script "test" with lines:
            | Line        |
            | _args ?x ?y |
        When I run headless with input "test -x"
        Then The output should contain: 
            | Line    |
            | flag: x |

    Scenario: Flags renamed
        Given I have a script "test" with lines:
            | Line            |
            | _args ?x:a ?y:b |
        When I run headless with input "test -x"
        Then The output should contain: 
            | Line    |
            | flag: a |

    Scenario: All flags
        Given I have a script "test" with lines:
            | Line        |
            | _args ?y -* |
        When I run headless with input "test -x -y -z"
        Then The output should contain: 
            | Line    |
            | flag: y |
            | flag: x |
            | flag: z |

Rule: Required script arguments must be provided

    Scenario: Required positional not provided
        Given I have a script "test" with lines:
            | Line       |
            | _args [0]! |
        When I run headless with input "test"
        Then The output should contain at least: 
            | Line                                                     |
            | Line 0: Required argument at position 0 was not provided |

    Scenario: Required positional not provided multiple
        Given I have a script "test" with lines:
            | Line                 |
            | _args [0]! [1]! [2]! |
        When I run headless with input "test"
        Then The output should contain at least: 
            | Line                                                     |
            | Line 0: Required argument at position 0 was not provided |
            | Line 0: Required argument at position 1 was not provided |
            | Line 0: Required argument at position 2 was not provided |

    Scenario: Required positional not provided multiple lines
        Given I have a script "test" with lines:
            | Line       |
            | _args [0]! |
            | _args [1]! |
            | _args [2]! |
        When I run headless with input "test"
        Then The output should contain at least: 
            | Line                                                     |
            | Line 0: Required argument at position 0 was not provided |
            | Line 1: Required argument at position 1 was not provided |
            | Line 2: Required argument at position 2 was not provided |

    Scenario: Required positional named not provided
        Given I have a script "test" with lines:
            | Line         |
            | _args ['x']! |
        When I run headless with input "test"
        Then The output should contain at least: 
            | Line                                                 |
            | Line 0: Required argument named 'x' was not provided |

    Scenario: Required positional named not provided multiple
        Given I have a script "test" with lines:
            | Line                       |
            | _args ['x']! ['y']! ['z']! |
        When I run headless with input "test"
        Then The output should contain at least: 
            | Line                                                 |
            | Line 0: Required argument named 'x' was not provided |
            | Line 0: Required argument named 'y' was not provided |
            | Line 0: Required argument named 'z' was not provided |

     Scenario: Required literal name named value not provided
        Given I have a script "test" with lines:
            | Line           |
            | _args a=['b']! |
        When I run headless with input "test"
        Then The output should contain at least: 
            | Line                                                 |
            | Line 0: Required argument named 'b' was not provided |

    Scenario: Required literal name positional value not provided
        Given I have a script "test" with lines:
            | Line         |
            | _args a=[0]! |
        When I run headless with input "test"
        Then The output should contain at least: 
            | Line                                                     |
            | Line 0: Required argument at position 0 was not provided |

    Scenario: Required named not provided
        Given I have a script "test" with lines:
            | Line       |
            | _args {c}! |
        When I run headless with input "test"
        Then The output should contain at least: 
            | Line                                                 |
            | Line 0: Required argument named 'c' was not provided |

Rule: Script arguments can become a verb

    Scenario: Single word verb from positional
        Given I have a script "test" with lines:
            | Line    |
            | [0] [1] |
        When I run headless with input "test echo value"
        Then The output should contain at least: 
            | Line  |
            | value |

    Scenario: Multi word verb from positionals
        Given I use the MultiWordVerb handler
        Given I have a script "test" with lines:
            | Line        |
            | [2] [1] [0] |
        When I run headless with input "test verb word multi"
        Then The output should contain at least: 
            | Line                    |
            | multi word verb invoked |
