#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV UrlDeleted="UrlDeleted"
ENV SwaggerName="TinyUrl Api"
ENV Producer="kafka:9092"
ENV SwaggerEndpoint="/swagger/TinyUrlOpenApiSpecification/swagger.json"
ENV UrlDb="server=urldbcontainer;Port=3306;database=urldb;uid=root;pwd=root"
ENV UrlLinked="UrlLinked"
ENV ASPNETCORE_ENVIRONMENT="Development"
ENV GroupId="UrlGroup"


FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TinyUrl/UrlManaging.csproj", "TinyUrl/"]
RUN dotnet restore "TinyUrl/UrlManaging.csproj"
COPY . .
WORKDIR "/src/TinyUrl"
RUN dotnet build "UrlManaging.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UrlManaging.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrlManaging.dll"]