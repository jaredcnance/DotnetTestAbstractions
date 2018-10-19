docker stop $(docker ps -aq) && docker rm $(docker ps -aq)

docker run \
  -e 'ACCEPT_EULA=Y' \
  -e 'SA_PASSWORD=P@ssword1' \
  -p 1433:1433 \
  --name DotnetTestAbstractions \
  -d microsoft/mssql-server-linux:2017-latest