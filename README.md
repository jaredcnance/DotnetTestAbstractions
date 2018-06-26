# DotnetTestAbstractions

Provides a set of abstractions for improving the performance and DX when writing integration tests.

## Pre-Requisites

Need a running SQL Server on localhost. You can do this 

```
docker run \
    -e 'ACCEPT_EULA=Y' \
    -e 'SA_PASSWORD=P@ssword1' \
    -p 1433:1433 \
    --name sql1 \
    -d microsoft/mssql-server-linux:2017-latest
```
