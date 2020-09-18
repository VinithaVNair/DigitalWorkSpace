FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV CardEdited="CardEdited"
ENV UrlGroupId="CardUrlGroup"
ENV UrlDeleted="UrlDeleted"
ENV SwaggerName="Card Open Api"
ENV Producer="MYKAFKA:9092"
ENV SwaggerEndpoint="/swagger/CardOpenApiSpecification/swagger.json"
ENV UrlLinked="UrlLinked"
ENV CardDb="server=carddbcontainer;Port=3306;database=cardsdb;uid=root;pwd=root"
ENV ASPNETCORE_ENVIRONMENT="Development"
ENV CardLinked="CardLinked"
ENV GroupId="CardGroup"

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CardManaging/CardManaging.csproj", "CardManaging/"]
RUN dotnet restore "CardManaging/CardManaging.csproj"
COPY . .
WORKDIR "/src/CardManaging"
RUN dotnet build "CardManaging.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CardManaging.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CardManaging.dll"]