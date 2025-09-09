Feature: Run

Scenario: Can run with input
    When I run with input "echo 'test'"
    Then The output should contain:
        | Line |
        | test |
    And The exit code is 0

Scenario: Can run async with input
    When I run async with input "echo 'test'"
    Then The output should contain:
        | Line |
        | test |

Scenario: Can run headless with input
    When I run headless with input "echo 'test'"
    Then The output should contain:
        | Line |
        | test |

Scenario: Can run headless async with input
    When I run headless async with input "echo 'test'"
    Then The output should contain:
        | Line |
        | test |

Scenario: Can run with command line arguments
    Given I set the command line text to "echo 'test'"
    When I run with command line arguments
    Then The output should contain:
        | Line |
        | test |

Scenario: Can run with command line arguments async
    Given I set the command line text to "echo 'test'"
    When I run with command line arguments async
    Then The output should contain:
        | Line |
        | test |

Scenario: Can run headless with command line arguments
    Given I set the command line text to "echo 'test'"
    When I run headless with command line arguments
    Then The output should contain:
        | Line |
        | test |

Scenario: Can run headless with command line arguments async
    Given I set the command line text to "echo 'test'"
    When I run headless with command line arguments async
    Then The output should contain:
        | Line |
        | test |

Scenario: Can run interactive with commands
    Given I input the following lines:
        | Line        |
        | echo 'test' |
    And I set the EngineStartInteractive script to:
        | Line         |
        | echo "start" |
    When I run interactively
    Then The output should contain:
        | Line  |
        | start |
        | test  |