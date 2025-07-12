Feature: Parsers

Rule: Can specify which argument parser to use

    Scenario: Use the simplified argument parser
        Given I use the SimplifiedArgumentParser
        When I run headless with input "_args a b=c -d"
        Then The output should contain:
            | Line    |
            | 0: a    |
            | 'b': c  |
            | flag: d |

    Scenario: Use the posix style argument parser
        Given I use the PosixStyleArgumentParser
        When I run headless with input "_args a --b=c -d"
        Then The output should contain:
            | Line    |
            | 0: a    |
            | 'b': c  |
            | flag: d |

    Scenario: Use the powershell argument parser
        Given I use the PowershellStyleArgumentParser
        When I run headless with input "_args a -b c -d"
        Then The output should contain:
            | Line    |
            | 0: a    |
            | 1: c    |
            | flag: b |
            | flag: d |

    Scenario: Use the windows cmd argument parser
        Given I use the WindowsCmdArgumentParser
        When I run headless with input "_args a /b:c /d"
        Then The output should contain:
            | Line    |
            | 0: a    |
            | 'b': c  |
            | flag: d |
             
