# Playground


## How to start
be at project route directory "Playground"  
Run docker-compose up

## Expose Bits

- Catalog Api 
    - https://localhost:44323
    - https://localhost:44323/hc 
- Basket Api 
    - https://localhost:44376
    - https://localhost:44376/hc  
- Sql Server 
    - localhost,1434;user id=sa;password=Pass@word123;database=Microsoft.eShopOnContainers.Services.CatalogDb;  
- [Rabbit MQ](https://www.rabbitmq.com/) 
    - http://localhost:15672/
- WebMvc
    - https://localhost:44384/
- [Seq](https://datalust.co/seq)
    - http://localhost:5340/


## Helpful commands
##### composing containers
- docker-compose up
    - [more info](https://docs.docker.com/compose/reference/up/)
    - docker compose up --build identity-api
- docker-compose down
    - [more info](https://docs.docker.com/compose/reference/down/)
- docker-compose build [(optional)service-name -- ex: catalog-api]
    - [more info](https://docs.docker.com/compose/reference/build/)
    - docker-compose build identity-api
- docker-compose start
    - [more info](https://docs.docker.com/compose/reference/start/)
- docker-compose stop
    - [more info](https://docs.docker.com/compose/reference/stop/)
- docker-compose run --service-ports [service-name -- ex: basket-api]
    - [more info](https://docs.docker.com/compose/reference/run/)
    - docker-compose run --service-ports basket-api
- docker-compose up -d --no-deps --build <service_name>
    - [how-to-rebuild-docker-container-in-docker-compose-yml](https://stackoverflow.com/questions/36884991/how-to-rebuild-docker-container-in-docker-compose-yml)
    - docker-compose up -d --no-deps --build catalog-api



##### interacting / manipulating containers
- docker container inspect catalog-api-eshop
- docker exec -it catalog-api-eshop sh

#### Docker Hub Examples
docker push thegomesteam/playground:catalog-api-eshop 
docker push thegomesteam/playground:web-mvc-eshop

docker tag playground_catalog-api thegomesteam/playground:catalog-api-eshop  


## Documents to know

Playground/docker-compose -- root docker compose file  
Playground/Catalog.Api/Dockerfile -- dockerfile to create Catalog.Api image 
Playground/Basket.Api/Dockerfile -- dockerfile to create Catalog.Api image 
Playground/WebMvc/Dockerfile -- dockerfile to create WebMvc image

## Helpful Docs

[.NET Core 2.1 and Docker — How to get Docker to recognize a local SSL certificate](https://mikewilliams.io/net-core-2-1-and-docker-how-to-get-docker-to-recognize-a-local-ssl-certificate-6e637e1e8800)  

dotnet run --server.urls https://0.0.0.0:54010
dotnet run --server.urls https://0.0.0.0:44384

## Noteworthy Db Tables

Clients  
ClientRedirectUris  
AspNetUsers  


## SSL
Generate self signed cert

openssl.exe
req -x509 -nodes -new -sha256 -days 1024 -newkey rsa:2048 -keyout RootCA.key -out RootCA.pem -subj "/C=US/CN=localhost" -config openssl.cfg
x509 -outform pem -in RootCA.pem -out RootCA.crt 


req -x509 -nodes -new -sha256 -days 1024 -newkey rsa:2048 -keyout identity-api.key -out identity-api.pem -subj "/C=US/CN=identity-api" -config openssl.cfg

x509 -outform pem -in identity-api.pem -out identity-api.crt