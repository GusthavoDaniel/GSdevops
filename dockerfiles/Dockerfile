# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restaura dependências (camadas mais estáveis primeiro)
COPY ["CareerMap.Recommendations.Api/CareerMap.Recommendations.Api.csproj", "CareerMap.Recommendations.Api/"]
COPY ["CareerMap.Recommendations.Domain/CareerMap.Recommendations.Domain.csproj", "CareerMap.Recommendations.Domain/"]
COPY ["CareerMap.Recommendations.Infrastructure/CareerMap.Recommendations.Infrastructure.csproj", "CareerMap.Recommendations.Infrastructure/"]
RUN dotnet restore "CareerMap.Recommendations.Api/CareerMap.Recommendations.Api.csproj"

# Copia o restante e compila
COPY . .
WORKDIR "/src/CareerMap.Recommendations.Api"
RUN dotnet build "CareerMap.Recommendations.Api.csproj" -c Release -o /app/build

# Publicação
FROM build AS publish
RUN dotnet publish "CareerMap.Recommendations.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# App Service (Linux/Containers) usa porta 8080
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CareerMap.Recommendations.Api.dll"]
