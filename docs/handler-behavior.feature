Feature: PRHB - Pre Handler Behavior

    Rule: All scenarios should work in case of 'QueryHandler' and 'CommandHandler'

        Scenario: [UNT-PRHB001]: Call 'PreHandler' behavior
            Given I have a <handler>
            And I have a <behavior>
            When Disposing the <handler>
            Then <behavior> should be called before <handler>
            Examples:
                | handler                             | behavior                               |
                | CommandHandler<Command>             | PreCommandHandler<Command>             |
                | QueryHandler<Query<object>, object> | PreQueryHandler<Query<object>, object> |

        Scenario: [UNT-PRHB002]: 'PreHandler' not configured
            Given I have a <handler>
            When Disposing the <handler>
            Then <handler> should be called without error
            Examples:
                | handler                             |
                | CommandHandler<Command>             |
                | QueryHandler<Query<object>, object> |

Feature: POHB - Post Handler Behavior

    Rule: All scenarios should work in case of 'QueryHandler' and 'CommandHandler'

        Scenario: [UNT-POHB001]: Call 'PostHandler' behavior
            Given I have a <handler>
            And I have a <behavior>
            When Disposing the <handler>
            Then <behavior> should be called after <handler>
            Examples:
                | handler                             | behavior                                |
                | CommandHandler<Command>             | PostCommandHandler<Command>             |
                | QueryHandler<Query<object>, object> | PostQueryHandler<Query<object>, object> |

        Scenario: [UNT-POHB002]: 'PostHandler' not configured
            Given I have a <handler>
            When Disposing the <handler>
            Then <handler> should be called without error
            Examples:
                | handler                             |
                | CommandHandler<Command>             |
                | QueryHandler<Query<object>, object> |

Feature: FLHB - Failure Handler Behavior

    Rule: All scenarios should work in case of 'QueryHandler' and 'CommandHandler'

        Scenario: [UNT-FLHB001]: Call 'FailureHandler' behavior
            Given I have a <handler>
            And I have a <behavior>
            When <handler> throwns an exception
            Then <behavior> should be called
            Examples:
                | handler                             | behavior                                |
                | CommandHandler<Command>             | FailureCommandHandler<Command>             |
                | QueryHandler<Query<object>, object> | FailureQueryHandler<Query<object>, object> |

        Scenario: [UNT-FLHB002]: 'FailureHandler' not configured
            Given I have a <handler>
            When <handler> throwns an exception
            Then <handler> should be called without error
            Examples:
                | handler                             |
                | CommandHandler<Command>             |
                | QueryHandler<Query<object>, object> |