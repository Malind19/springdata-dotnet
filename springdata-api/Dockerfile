FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["springdata-api/springdata-api.csproj", "springdata-api/"]
RUN dotnet restore "springdata-api/springdata-api.csproj"
COPY . .
WORKDIR "/src/springdata-api"
RUN dotnet build "springdata-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "springdata-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "springdata-api.dll"]