Feature: TCH - Transactional Handler

    Rule: In case if queries and commands

        Scenario: [UNT-TCH001]: Begin Transaction
            When Call 'CommandHandler.HandleAsync'
            Then New 'Transaction' should be created

        Scenario: [UNT-TCH002]: Running Transaction
            Given 'Transaction' has been started
            When Call 'CommandHandler.HandleAsnyc'
            Then New 'Transaction' should not been created

        Scenario: [UNT-TCH003]: Commit 'Transaction'
            Given 'Transaction' has been started
            When Call 'CommandHandler.HandleAsync'
            Then 'Transaction' should be commited

        Scenario: [UNT-TCH004]: Rollback 'Transaction'
            Given 'Transaction' has been started
            When Call 'CommandHandler.HandleAsync'
            And Next item in the 'CommandHandler' pipe throws an exception
            Then 'Transaction' should be rollbacked