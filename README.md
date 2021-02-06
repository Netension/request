![nuget-template](https://github.com/Netension/nuget-template/blob/develop/banner.png)
__Description__

![Publish](https://github.com/Netension/nuget-template/workflows/Publish/badge.svg)<br/>
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Netension_nuget-template&metric=alert_status)](https://sonarcloud.io/dashboard?id=Netension_nuget-template)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Netension_nuget-template&metric=coverage)](https://sonarcloud.io/dashboard?id=Netension_nuget-template)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Netension_nuget-template&metric=bugs)](https://sonarcloud.io/dashboard?id=Netension_nuget-template)

# Command-Query Separation (CQS) principle

The ***CQS*** principle classifies an object's methods two sharply separeted categories:
- ***Commands:*** Change the state of the system, but do not have return value.
- ***Queries:*** Do not change the state of the system, but it has return value (free of side effects).
