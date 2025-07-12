Feature: Environments

Rule: Can change environments
    
    Background: 
        Given I clear the EngineStartInteractive script
        And I use the environments:
            | Environment |
            | A           |
            | B           |

    Scenario: I can change environment
        Given I input the following lines:
            | Line  |
            | env B |
            | env A |
        When I run interactively in environment "A"
        Then The output should contain:
            | Line                     |
            | Environment changed to A |
            | Environment changed to B |
            | Environment changed to A |

    Scenario: I do not change environment with notset
        Given I input the following lines:
            | Line  |
            | env -notset B |
        When I run interactively in environment "A"
        Then The output should contain:
            | Line                     |
            | Environment changed to A |

    Scenario: I can change environment by index
        Given I input the following lines:
            | Line  |
            | env 2 |
            | env 1 |
        When I run interactively in environment "A"
        Then The output should contain:
            | Line                     |
            | Environment changed to A |
            | Environment changed to B |
            | Environment changed to A |

     Scenario: I cannot change to an invalid environment
        Given I clear the EnvironmentChanged script
        Given I input the following lines:
            | Line  |
            | env X |
        When I run interactively in environment "A"
        Then The output should contain at least:
            | Line                    |
            | Unknown environment 'X' |

Rule: Can list environments
    
    Background: 
        Given I clear the EngineStartInteractive script
        And I clear the EngineStartInteractive script
        And I clear the EnvironmentChanged script
        And I use the environments:
            | Environment |
            | A           |
            | B           |
            | C           |

    Scenario: I can list environments
        Given I input the following lines:
            | Line      |
            | env -list |
        When I run interactively in environment "A"
        Then The output should contain:
            | Line |
            | 1) A |
            | 2) B |
            | 3) C |

Rule: Can change environments by prompt
    
    Background: 
        Given I clear the EngineStartInteractive script
        And I use the environments:
            | Environment |
            | A           |
            | B           |
            | C           |

    Scenario: I can change environment by prompt name
        Given I input the following lines:
            | Line  |
            | env   |
            | B     |
            | env   |
            | C     |
        When I run interactively
        Then The output should contain:
            | Line                          |
            | Please select an environment: |
            | 1) A                          |
            | 2) B                          |
            | 3) C                          |
            | Environment changed to B      |
            | Please select an environment: |
            | 1) A                          |
            | 2) B                          |
            | 3) C                          |
            | Environment changed to C      |

    Scenario: I can change environment by prompt index
        Given I input the following lines:
            | Line  |
            | env   |
            | 2     |
            | env   |
            | 3     |
        When I run interactively
        Then The output should contain:
            | Line                          |
            | Please select an environment: |
            | 1) A                          |
            | 2) B                          |
            | 3) C                          |
            | Environment changed to B      |
            | Please select an environment: |
            | 1) A                          |
            | 2) B                          |
            | 3) C                          |
            | Environment changed to C      |

Rule: PerEnvironment objects maintain state until env changed

    Background: 
        Given I use the IncrementEnvStateCount handler
        And I clear the EngineStartInteractive script
        And I use the environments:
            | Environment |
            | A           |
            | B           |


    Scenario: PerEnvironment object maintains state
        Given I input the following lines:
            | Line                      |
            | increment-env-state-count |
            | increment-env-state-count |
            | increment-env-state-count |
            | env B                     |
            | increment-env-state-count |
            | increment-env-state-count |
        When I run interactively in environment "A"
        Then The output should contain:
            | Line                               |
            | Environment changed to A           |
            | A: 1                               |
            | A: 2                               |
            | A: 3                               |
            | Environment changed to B           |
            | B: 1                               |
            | B: 2                               |

    Scenario: PerEnvironment object maintains state no env changed script
        Given I clear the EnvironmentChanged script
        And I input the following lines:
            | Line                      |
            | increment-env-state-count |
            | increment-env-state-count |
            | increment-env-state-count |
            | env B                     |
            | increment-env-state-count |
            | increment-env-state-count |
        When I run interactively in environment "A"
        Then The output should contain:
            | Line                               |
            | A: 1                               |
            | A: 2                               |
            | A: 3                               |
            | B: 1                               |
            | B: 2                               |
