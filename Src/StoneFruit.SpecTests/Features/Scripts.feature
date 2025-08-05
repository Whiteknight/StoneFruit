Feature: Scripts


Rule: Scripts can have multiple commands

    Scenario: Script with 3 lines
        Given I have a script "test" with lines:
            | Line             |
            | echo start       |
            | _args [0] {b} ?d |
            | echo stop        |
        When I run headless with input "test a b=c -d"
        Then The output should contain: 
            | Line    |
            | start   |
            | 0: a    |
            | 'b': c  |
            | flag: d |
            | stop    |

Rule: Scripts can call other scripts

    Scenario: One script calls another script
        Given I have a script "test1" with lines:
            | Line      |
            | _args [0] |
        And I have a script "test2" with lines:
            | Line      |
            | test1 [0] |
        When I run headless with input "test2 a"
        Then The output should contain: 
            | Line    |
            | 0: a    |

    Scenario: One script calls another script declared after
        Given I have a script "test2" with lines:
            | Line      |
            | test1 [0] |
        Given I have a script "test1" with lines:
            | Line      |
            | _args [0] |
        When I run headless with input "test2 a"
        Then The output should contain: 
            | Line    |
            | 0: a    |

Rule: Scripts can have comments

    Scenario: Full line comment bash style
        Given I have a script "test" with lines:
            | Line      |
            | # comment |
            | echo test |
        When I run headless with input "test"
        Then The output should contain: 
            | Line |
            | test |

    Scenario: Full line comment CPP style
        Given I have a script "test" with lines:
            | Line       |
            | // comment |
            | echo test  |
        When I run headless with input "test"
        Then The output should contain: 
            | Line |
            | test |

    Scenario: Part line comment bash style
        Given I have a script "test" with lines:
            | Line                |
            | echo test # comment |
        When I run headless with input "test"
        Then The output should contain: 
            | Line |
            | test |

    Scenario: Part line comment CPP style
        Given I have a script "test" with lines:
            | Line                 |
            | echo test // comment |
        When I run headless with input "test"
        Then The output should contain: 
            | Line |
            | test |
