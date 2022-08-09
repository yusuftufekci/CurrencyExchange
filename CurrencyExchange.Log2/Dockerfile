FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_URLS=http://+:5030/
EXPOSE 5030


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src


COPY ["CurrencyExchange.Log.csproj", "CurrencyExchange.Log2/"]



RUN dotnet restore "CurrencyExchange.Log2/CurrencyExchange.Log.csproj"

COPY . .

WORKDIR "/src"
RUN dotnet build "CurrencyExchange.Log.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CurrencyExchange.Log.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CurrencyExchange.Log.dll"]
