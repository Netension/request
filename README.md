![nuget-template](https://github.com/Netension/request/blob/develop/banner.png)
__Description__

![Publish](https://github.com/Netension/request/workflows/Release/badge.svg)<br/>
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Netension_request&metric=alert_status)](https://sonarcloud.io/dashboard?id=Netension_request)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Netension_request&metric=coverage)](https://sonarcloud.io/dashboard?id=Netension_request)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Netension_request&metric=bugs)](https://sonarcloud.io/dashboard?id=Netension_request)

# Command-Query Separation (CQS) principle

The ***CQS*** principle classifies an object's methods two sharply separeted categories:
- **Commands:** Change the state of the system, but do not have return value.
- **Queries:** Do not change the state of the system, but it has return value (free of side effects).
