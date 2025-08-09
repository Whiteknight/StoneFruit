Feature: ScriptFile

Scenario: Can load a script file by filename
    Given I use script file 'ScriptFile.txt' by file name
    When I run headless with input 'ScriptFile'
    Then The output should contain:
        | Line             |
        | From Script File |

Scenario: Can load a script file by stream
    Given I use script file 'ScriptFile.txt' by stream
    When I run headless with input 'ScriptFile'
    Then The output should contain:
        | Line             |
        | From Script File |

