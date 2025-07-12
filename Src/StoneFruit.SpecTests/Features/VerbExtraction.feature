Feature: VerbExtraction

Scenario: CamelCase verb extractor
    Given I use the MultiWordVerb handler
    And I use the CamelCaseVerbExtractor
    When I run headless with input "multi word verb"
    Then The output should contain:
        | Line                    |
        | multi word verb invoked |

Scenario: CamelCaseToSpinalCase verb extractor
    Given I use the MultiWordVerb handler
    And I use the CamelCaseToSpinalCaseVerbExtractor
    When I run headless with input "multi-word-verb"
    Then The output should contain:
        | Line                    |
        | multi word verb invoked |

Scenario: ToLower verb extractor
    Given I use the MultiWordVerb handler
    And I use the ToLowerNameVerbExtractor
    When I run headless with input "multiwordverb"
    Then The output should contain:
        | Line                    |
        | multi word verb invoked |

