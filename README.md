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
- docker-compose down
    - [more info](https://docs.docker.com/compose/reference/down/)
- docker-compose build [(optional)service-name -- ex: catalog-api]
    - [more info](https://docs.docker.com/compose/reference/build/)
- docker-compose start
    - [more info](https://docs.docker.com/compose/reference/start/)
- docker-compose stop
    - [more info](https://docs.docker.com/compose/reference/stop/)
- docker-compose run --service-ports [service-name -- ex: basket-api]
    - [more info](https://docs.docker.com/compose/reference/run/)



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