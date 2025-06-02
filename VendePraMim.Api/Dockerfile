# Dockerfile para VendePraMim.Api (.NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["VendePraMim.Api/VendePraMim.Api.csproj", "VendePraMim.Api/"]
COPY ["VendePraMim.Api/", "VendePraMim.Api/"]
RUN dotnet restore "VendePraMim.Api/VendePraMim.Api.csproj"
WORKDIR "/src/VendePraMim.Api"
RUN dotnet build "VendePraMim.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VendePraMim.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VendePraMim.Api.dll"]
