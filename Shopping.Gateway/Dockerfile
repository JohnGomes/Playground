FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

COPY "Playground.sln" "Playground.sln"

COPY "Shopping.Gateway/Shopping.Gateway.csproj" "Shopping.Gateway/Shopping.Gateway.csproj"
COPY "EventBusRabbitMQ/EventBusRabbitMQ.csproj" "EventBusRabbitMQ/EventBusRabbitMQ.csproj"
COPY "EventBusServiceBus/EventBusServiceBus.csproj" "EventBusServiceBus/EventBusServiceBus.csproj"
COPY "EventBus/EventBus.csproj" "EventBus/EventBus.csproj"
COPY "WebHost.Customization/WebHost.Customization.csproj" "WebHost.Customization/WebHost.Customization.csproj"
COPY "IntegrationEventLogEF/IntegrationEventLogEF.csproj" "IntegrationEventLogEF/IntegrationEventLogEF.csproj"
COPY "Basket.Api/Basket.Api.csproj" "Basket.Api/Basket.Api.csproj"
COPY "GrpcGreeter/GrpcGreeter.csproj" "GrpcGreeter/GrpcGreeter.csproj"
COPY "GrpcGreeterClient/GrpcGreeterClient.csproj" "GrpcGreeterClient/GrpcGreeterClient.csproj"
COPY "Catalog.Api/Catalog.Api.csproj" "Catalog.Api/Catalog.Api.csproj"
COPY "WebMvc/WebMvc.csproj" "WebMvc/WebMvc.csproj"
COPY "Identity.Api/Identity.Api.csproj" "Identity.Api/Identity.Api.csproj"
COPY "Ordering.Api/Ordering.Api.csproj" "Ordering.Api/Ordering.Api.csproj"
COPY "Ordering.Domain/Ordering.Domain.csproj" "Ordering.Domain/Ordering.Domain.csproj"
COPY "Ordering.Infrastructure/Ordering.Infrastructure.csproj" "Ordering.Infrastructure/Ordering.Infrastructure.csproj"

COPY "NuGet.config" "NuGet.config"


RUN dotnet restore "Playground.sln"

COPY . .
WORKDIR /Shopping.Gateway
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build /Cert /app/Cert
COPY --from=build /Cert /usr/local/share/ca-certificates/
RUN chmod 644 /usr/local/share/ca-certificates/RootLocalhostCA.crt 
RUN update-ca-certificates
ENTRYPOINT ["dotnet", "Shopping.Gateway.dll"]
