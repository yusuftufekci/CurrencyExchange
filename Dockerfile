FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_URLS=http://+:5020/
EXPOSE 5020


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY ["CurrencyExchange.API/CurrencyExchange.API.csproj", "CurrencyExchange.API/"]
COPY ["CurrencyExchange.Service/CurrencyExchange.Service.csproj", "CurrencyExchange.Service/"]
COPY ["CurrencyExchange.Repository/CurrencyExchange.Repository.csproj", "CurrencyExchange.Repository/"]
COPY ["CurrencyExchange.Core/CurrencyExchange.Core.csproj", "CurrencyExchange.Core/"]
COPY ["CurrencyExchange.Log2/CurrencyExchange.Log.csproj", "CurrencyExchange.Log2/"]



RUN dotnet restore "CurrencyExchange.API/CurrencyExchange.API.csproj"

COPY . .

WORKDIR "/src/CurrencyExchange.API"
RUN dotnet build "CurrencyExchange.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CurrencyExchange.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CurrencyExchange.API.dll"]
