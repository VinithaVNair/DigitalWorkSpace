#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV CardEdited="CardEdited"
ENV SwaggerName="Catalog Open Api"
ENV Producer="MYKAFKA:9092"
ENV SwaggerEndpoint="/swagger/CatalogOpenApiSpecification/swagger.json"
ENV CatalogDb="server=catalogdbcontainer;Port=3306;database=catalogdb;uid=root;pwd=root"
ENV ASPNETCORE_ENVIRONMENT="Development"
ENV CardLinked="CardLinked"
ENV GroupId="CatalogGroup"

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CatalogManaging/CatalogManaging.csproj", "CatalogManaging/"]
COPY ["Catalog.Infrastructure/CatalogManaging.Infrastructure.csproj", "Catalog.Infrastructure/"]
COPY ["Catalog.Core/CatalogManaging.Core.csproj", "Catalog.Core/"]
RUN dotnet restore "CatalogManaging/CatalogManaging.csproj"
COPY . .
WORKDIR "/src/CatalogManaging"
RUN dotnet build "CatalogManaging.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogManaging.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogManaging.dll"]