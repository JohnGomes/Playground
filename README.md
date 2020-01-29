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


##### interacting / manipulating containers
- docker container inspect catalog-api-eshop
- docker exec -it catalog-api-eshop sh



## Documents to know

Playground/docker-compose -- root docker compose file  
Playground/Catalog.Api/Dockerfile -- dockerfile to create Catalog.Api image 
Playground/WebMvc/Dockerfile -- dockerfile to create WebMvc image  