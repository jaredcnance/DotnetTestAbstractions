# DotnetTestAbstractions

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/DotnetTestAbstractions.svg)](https://www.nuget.org/packages/DotnetTestAbstractions)
[![Build status](https://ci.appveyor.com/api/projects/status/4wb49yoo3c5xrfuj?svg=true)](https://ci.appveyor.com/project/jaredcnance/dotnettestabstractions)



Provides a set of abstractions for improving the performance and DX when writing integration tests.

**Read the blog post [here](https://nance.io/a-better-story-for-asp-net-core-integration-testing/).**

## Usage

See the [Examples](https://github.com/jaredcnance/DotnetTestAbstractions/tree/master/examples/WebApp.Tests)
for usage details.

### Write Easy to Read Tests Without Worrying About Leaky State

```csharp
public class ArticlesController_Tests : JsonServerFixture<TestStartup, AppContext>
{
    [Fact]
    public async Task Can_Get_All_Articles()
    {
        // arrange
        var article = new Article();
        await DbContext.Articles.AddAsync(article);
        await DbContext.SaveChangesAsync();

        // act
        var articles = await GetAllAsync<Article>("articles");

        // assert
        Assert.Single(articles);
    }
}
```

## Features

- Management of `TestServer` instances
- Database calls get wrapped in a transaction that is shared between the test and server
- Easy to use APIs for executing RESTful JSON HTTP requests
- Reset the database before any tests run by setting the environment variable `DROP_DATABASE_ONSTART=true`
- Enable logging by setting the environment variable `DTA_LOG_LEVEL=Information` to the approproate log level (see Microsoft.Extensions.Logging.LogLevel for valid values.

# Development

## Pre-Requisites

Need a running SQL Server on localhost. 
You can do this by pulling the SQL Server docker container:

```
docker run \
    -e 'ACCEPT_EULA=Y' \
    -e 'SA_PASSWORD=P@ssword1' \
    -p 1433:1433 \
    --name DotnetTestAbstractions \
    -d microsoft/mssql-server-linux:2017-latest
```
