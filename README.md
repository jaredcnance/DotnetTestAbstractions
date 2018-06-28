# DotnetTestAbstractions

Provides a set of abstractions for improving the performance and DX when writing integration tests.

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

# Development

## Pre-Requisites

Need a running SQL Server on localhost. 
You can do this by pulling the SQL Server docker container:

```
docker run \
    -e 'ACCEPT_EULA=Y' \
    -e 'SA_PASSWORD=P@ssword1' \
    -p 1433:1433 \
    --name sql1 \
    -d microsoft/mssql-server-linux:2017-latest
```
