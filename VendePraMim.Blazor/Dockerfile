# Dockerfile para VendePraMim.Blazor (.NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["VendePraMim.Blazor/VendePraMim.Blazor.csproj", "VendePraMim.Blazor/"]
COPY ["VendePraMim.Blazor/", "VendePraMim.Blazor/"]
RUN dotnet restore "VendePraMim.Blazor/VendePraMim.Blazor.csproj"
WORKDIR "/src/VendePraMim.Blazor"
RUN dotnet build "VendePraMim.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VendePraMim.Blazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VendePraMim.Blazor.dll"]
