# Playground


## How to start
be at project route directory "Playground"  
Run docker-compose up

## Expose Bits

#### Catalog Api 
    http://localhost:44322/swagger/index.html  
    https://localhost:44323/swagger/index.html  

#### Sql Server 
    localhost,1434;user id=sa;password=Pass@word123;database=Microsoft.eShopOnContainers.Services.CatalogDb;  

#### Rabbit MQ 
    http://localhost:15672/

#### WebMvc
    https://localhost:44384/


## Helpful commands
##### composing containers
- docker-compose up
- docker-compose down
- docker-compose build
- docker-compose start
- docker-compose stop
- docker-compose run --service-ports [service-name -- ex: basket-api]



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