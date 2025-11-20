#!/bin/bash

# Script de Provisionamento de Infraestrutura Azure (Azure CLI)
# Requisitos: Resource Group, Azure Database (PaaS), Azure Web App (PaaS) ou Container Registry/Container Instance

# Variáveis de Configuração (Substitua pelos seus valores)
RESOURCE_GROUP_NAME="rg-devops-gs-net"
LOCATION="eastus" # Escolha uma região próxima
APP_SERVICE_PLAN_NAME="plan-devops-gs-net"
WEB_APP_NAME="webapp-devops-gs-net-$(openssl rand -hex 4)" # Nome único para o Web App
SQL_SERVER_NAME="sqlserver-devops-gs-net-$(openssl rand -hex 4)" # Nome único para o SQL Server
SQL_DATABASE_NAME="db-devops-gs-net"
SQL_ADMIN_USER="sqladmin"
SQL_ADMIN_PASSWORD="SuaSenhaForteAqui123!" # ATENÇÃO: Use uma senha forte e segura

# 1. Criar Resource Group
echo "Criando Resource Group: $RESOURCE_GROUP_NAME em $LOCATION"
az group create --name $RESOURCE_GROUP_NAME --location $LOCATION

# 2. Criar Azure SQL Server (PaaS)
echo "Criando Azure SQL Server: $SQL_SERVER_NAME"
az sql server create \
    --name $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP_NAME \
    --location $LOCATION \
    --admin-user $SQL_ADMIN_USER \
    --admin-password $SQL_ADMIN_PASSWORD

# 3. Configurar regra de firewall para permitir acesso de serviços Azure
echo "Configurando regra de firewall para serviços Azure"
az sql server firewall-rule create \
    --resource-group $RESOURCE_GROUP_NAME \
    --server $SQL_SERVER_NAME \
    --name AllowAzureServices \
    --start-ip-address 0.0.0.0 \
    --end-ip-address 0.0.0.0

# 4. Criar Azure SQL Database
echo "Criando Azure SQL Database: $SQL_DATABASE_NAME"
az sql db create \
    --resource-group $RESOURCE_GROUP_NAME \
    --server $SQL_SERVER_NAME \
    --name $SQL_DATABASE_NAME \
    --edition Basic \
    --family Gen5 \
    --capacity 1

# 5. Criar App Service Plan (Plano de Hospedagem)
echo "Criando App Service Plan: $APP_SERVICE_PLAN_NAME"
az appservice plan create \
    --name $APP_SERVICE_PLAN_NAME \
    --resource-group $RESOURCE_GROUP_NAME \
    --location $LOCATION \
    --sku B1 \
    --is-linux

# 6. Criar Web App (para deploy do .NET)
echo "Criando Web App: $WEB_APP_NAME"
az webapp create \
    --resource-group $RESOURCE_GROUP_NAME \
    --plan $APP_SERVICE_PLAN_NAME \
    --name $WEB_APP_NAME \
    --runtime "DOTNET|8.0" # Ajuste a versão do .NET conforme necessário

# 7. Configurar Variáveis de Ambiente (Connection String) no Web App
# ATENÇÃO: Em um cenário real, use Azure Key Vault ou Variáveis de Pipeline para a senha.
# Aqui estamos usando a configuração de Application Settings do App Service.
CONNECTION_STRING="Server=tcp:$SQL_SERVER_NAME.database.windows.net,1433;Initial Catalog=$SQL_DATABASE_NAME;Persist Security Info=False;User ID=$SQL_ADMIN_USER;Password=$SQL_ADMIN_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

echo "Configurando Connection String no Web App"
az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $WEB_APP_NAME \
    --settings "ConnectionStrings__DefaultConnection=$CONNECTION_STRING"

echo "Provisionamento concluído!"
echo "Resource Group: $RESOURCE_GROUP_NAME"
echo "Web App URL: https://$WEB_APP_NAME.azurewebsites.net"
echo "SQL Server: $SQL_SERVER_NAME"
echo "SQL Database: $SQL_DATABASE_NAME"

# Permissão de execução para o script
chmod +x /home/ubuntu/GS.NET2/scripts/script-infra.sh
