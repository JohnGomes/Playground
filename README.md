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

[Install Protainer](https://www.portainer.io/installation/)

dotnet run --server.urls https://0.0.0.0:54010
dotnet run --server.urls https://0.0.0.0:44384


## SQL
> "server=localhost,1434;user id=sa;password=Pass@word123;database=Microsoft.eShopOnContainers.Services.CatalogDb;"
### Noteworthy Db Tables
Clients  
ClientRedirectUris  
AspNetUsers  


## SSL


[.NET Core 2.1 and Docker — How to get Docker to recognize a local SSL certificate](https://mikewilliams.io/net-core-2-1-and-docker-how-to-get-docker-to-recognize-a-local-ssl-certificate-6e637e1e8800)  


### Generate self signed cert /w OpenSSL
- install [openssl](https://www.openssl.org/)
- Run openssl.exe

## Current Instructions
```
https://stackoverflow.com/questions/7580508/getting-chrome-to-accept-self-signed-localhost-certificate
######################
# Become a Certificate Authority
######################

# Generate private key
openssl genrsa -des3 -out myCA.key 2048 -password pass:"idsrv3test"
# Generate root certificate
openssl req -x509 -new -nodes -key myCA.key -sha256 -days 1825 -out myCA.pem

######################
# Create CA-signed certs
######################

NAME=mydomain.com
# Generate private key
[[ -e $NAME.key ]] || openssl genrsa -out localhost.key 2048
# Create certificate-signing request
[[ -e $NAME.csr ]] || openssl req -new -key localhost.key -out localhost.csr
# Create a config file for the extensions
>$NAME.ext cat <<-EOF
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
subjectAltName = @alt_names
[alt_names]
DNS.1 = $NAME
DNS.2 = bar.$NAME
EOF
# Create the signed certificate
openssl x509 -req -in localhost.csr -CA myCA.pem -CAkey myCA.key -CAcreateserial -out localhost.crt -days 1825 -sha256 -extfile localhost.ext
```

## Old Instructions
```
- Generate self signed 'localhost' for dotnetcore
    - > req -x509 -nodes -days 1024 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -config localhost.conf -passin pass:Pass@word1
- Generate crt
    - > pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt

- Generate pem and key for Root CA 'localhost'
    - > req -x509 -nodes -new -sha256 -days 1024 -newkey rsa:2048 -keyout RootLocalhostCA.key -out RootLocalhostCA.pem -subj "/C=US/CN=localhost" -config openssl.cfg
- Generate crt from pem
    - > x509 -outform pem -in RootLocalhostCA.pem -out RootLocalhostCA.crt 

- Generate pem and key for 'identity-api' domain
    - > req -x509 -nodes -new -sha256 -days 1024 -newkey rsa:2048 -keyout identity-api.key -out identity-api.pem -subj "/C=US/CN=identity-api" -config openssl.cfg
- Generate crt from pem
    - > x509 -outform pem -in identity-api.pem -out identity-api.crt


- Create Private PEM
    - > touch private-idsrv3test.pem
    - > genrsa 2048 > private-idsrv3test.pem
    - Copy Generated text into file
- Generate public pem from private pem for 'idsrv3test' domain
    - > req -x509 -days 1024 -new -key private-idsrv3test.pem -out public-idsrv3test.pem -subj "/C=US/CN=idsrv3test" 
- Generate pfk from pem
    - > pkcs12 -export -in public-idsrv3test.pem -inkey private-idsrv3test.pem -out idsrv3test.pfx -passin pass:"idsrv3test"

```


## Sharing Localhost
### NGrok
 use ngrok to generate url https://dashboard.ngrok.com/get-started - ngrok http https://localhost:54010
### localhost.run
ssh -R 80:localhost:44375 basket-api@ssh.localhost.run