# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto e restaura as dependências
COPY ["CareerMap.Recommendations.Api/CareerMap.Recommendations.Api.csproj", "CareerMap.Recommendations.Api/"]
COPY ["CareerMap.Recommendations.Domain/CareerMap.Recommendations.Domain.csproj", "CareerMap.Recommendations.Domain/"]
COPY ["CareerMap.Recommendations.Infrastructure/CareerMap.Recommendations.Infrastructure.csproj", "CareerMap.Recommendations.Infrastructure/"]
RUN dotnet restore "CareerMap.Recommendations.Api/CareerMap.Recommendations.Api.csproj"

# Copia o restante dos arquivos e faz o build
COPY . .
WORKDIR "/src/CareerMap.Recommendations.Api"
RUN dotnet build "CareerMap.Recommendations.Api.csproj" -c Release -o /app/build

# Estágio de Publicação
FROM build AS publish
RUN dotnet publish "CareerMap.Recommendations.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# App Service espera porta 80
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

# Copia os arquivos publicados
COPY --from=publish /app/publish .

# Define o ponto de entrada
ENTRYPOINT ["dotnet", "CareerMap.Recommendations.Api.dll"]
