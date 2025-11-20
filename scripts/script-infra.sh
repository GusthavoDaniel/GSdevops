#!/bin/bash

# ============================================================
#  Script de Provisionamento de Infraestrutura - GS DevOps
#  Projeto: CareerMap (.NET)
#  Requisitos atendidos:
#    - Resource Group
#    - Azure SQL Server + Database (PaaS)
#    - Azure Container Registry (ACR)
#    - App Service Plan (Linux)
#    - Web App (Linux) rodando CONTAINER do ACR
#    - Connection String configurada como App Setting
# ============================================================

# ========== CONFIGURAÇÕES INICIAIS ==========
# 👉 TROQUE o que estiver entre aspas SOMENTE se precisar

SUBSCRIPTION_ID="0250b066-0137-40d1-bd64-59945bbf9b82"   # SEU Subscription ID
LOCATION="brazilsouth"                                  # região
RM_ID="RM554681"                                        # seu RM

# Nomes dos recursos (padrão FIAP)
RESOURCE_GROUP_NAME="rg-gs-careermap-$RM_ID"
APP_SERVICE_PLAN_NAME="asp-gs-careermap-$RM_ID"

# nomes do ACR e SQL têm que ser ÚNICOS e minúsculos, sem caracteres especiais
ACR_NAME="acrcareermap${RM_ID,,}"             # ex: acrcareermaprm554681

# usa um sufixo aleatório pra garantir que não quebre por nome duplicado
RAND_SUFFIX=$(openssl rand -hex 3)

WEB_APP_NAME="app-careermap-${RM_ID,,}-${RAND_SUFFIX}"
SQL_SERVER_NAME="sqlcareermap${RM_ID,,}-${RAND_SUFFIX}"
SQL_DATABASE_NAME="db-careermap-${RM_ID,,}"

SQL_ADMIN_USER="sqladmin"
SQL_ADMIN_PASSWORD="Fiap@2025_DevOps"  # senha forte (guarda isso!)

# Nome e tag padrão da imagem Docker
IMAGE_NAME="careermap-api"
IMAGE_TAG="v1"

# ========== LOGIN NA SUBSCRIPTION ==========
if [ -n "$SUBSCRIPTION_ID" ]; then
  echo "➡️ Definindo subscription: $SUBSCRIPTION_ID"
  az account set --subscription "$SUBSCRIPTION_ID"
else
  echo "⚠️ SUBSCRIPTION_ID vazio. Usando subscription padrão logada no Cloud Shell."
fi

# ========== RESOURCE GROUP ==========
echo "➡️ Criando Resource Group: $RESOURCE_GROUP_NAME em $LOCATION"
az group create \
  --name "$RESOURCE_GROUP_NAME" \
  --location "$LOCATION"

# ========== AZURE CONTAINER REGISTRY (ACR) ==========
echo "➡️ Criando Azure Container Registry: $ACR_NAME"
az acr create \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --name "$ACR_NAME" \
  --sku Basic \
  --admin-enabled true

ACR_LOGIN_SERVER=$(az acr show \
  --name "$ACR_NAME" \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --query "loginServer" -o tsv)

echo "   ACR_LOGIN_SERVER: $ACR_LOGIN_SERVER"

# ========== AZURE SQL SERVER + DATABASE ==========
echo "➡️ Criando Azure SQL Server: $SQL_SERVER_NAME"
az sql server create \
  --name "$SQL_SERVER_NAME" \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --location "$LOCATION" \
  --admin-user "$SQL_ADMIN_USER" \
  --admin-password "$SQL_ADMIN_PASSWORD"

echo "➡️ Configurando firewall do SQL para permitir serviços Azure"
az sql server firewall-rule create \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --server "$SQL_SERVER_NAME" \
  --name "AllowAzureServices" \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

echo "➡️ Criando Azure SQL Database: $SQL_DATABASE_NAME"
az sql db create \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --server "$SQL_SERVER_NAME" \
  --name "$SQL_DATABASE_NAME" \
  --edition Basic \
  --family Gen5 \
  --capacity 1

# Montar a connection string padrão do SQL
CONNECTION_STRING="Server=tcp:$SQL_SERVER_NAME.database.windows.net,1433;Initial Catalog=$SQL_DATABASE_NAME;Persist Security Info=False;User ID=$SQL_ADMIN_USER;Password=$SQL_ADMIN_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

echo "   Connection String gerada:"
echo "   $CONNECTION_STRING"

# ========== APP SERVICE PLAN (LINUX) ==========
echo "➡️ Criando App Service Plan (Linux): $APP_SERVICE_PLAN_NAME"
az appservice plan create \
  --name "$APP_SERVICE_PLAN_NAME" \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --location "$LOCATION" \
  --sku B1 \
  --is-linux

# ========== WEB APP (CONTAINER) ==========
echo "➡️ Criando Web App (Linux/Container): $WEB_APP_NAME"
az webapp create \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --plan "$APP_SERVICE_PLAN_NAME" \
  --name "$WEB_APP_NAME" \
  --deployment-container-image-name "$ACR_LOGIN_SERVER/$IMAGE_NAME:$IMAGE_TAG"

# Pegar credenciais do ACR
ACR_USERNAME=$(az acr credential show --name "$ACR_NAME" --query "username" -o tsv)
ACR_PASSWORD=$(az acr credential show --name "$ACR_NAME" --query "passwords[0].value" -o tsv)

echo "➡️ Configurando Web App para usar o ACR como fonte de imagens"
az webapp config container set \
  --name "$WEB_APP_NAME" \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --docker-custom-image-name "$ACR_LOGIN_SERVER/$IMAGE_NAME:$IMAGE_TAG" \
  --docker-registry-server-url "https://$ACR_LOGIN_SERVER" \
  --docker-registry-server-user "$ACR_USERNAME" \
  --docker-registry-server-password "$ACR_PASSWORD"

# ========== VARIÁVEIS DE AMBIENTE DO APP ==========
echo "➡️ Configurando variáveis de ambiente do Web App"
az webapp config appsettings set \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --name "$WEB_APP_NAME" \
  --settings \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "ConnectionStrings__DefaultConnection=$CONNECTION_STRING"

# ========== RESUMO FINAL ==========
WEBAPP_URL="https://$WEB_APP_NAME.azurewebsites.net"

echo ""
echo "✅ PROVISIONAMENTO CONCLUÍDO!"
echo "----------------------------------------------------"
echo "Resource Group ..........: $RESOURCE_GROUP_NAME"
echo "ACR Name ................: $ACR_NAME"
echo "ACR Login Server ........: $ACR_LOGIN_SERVER"
echo "SQL Server ..............: $SQL_SERVER_NAME"
echo "SQL Database ............: $SQL_DATABASE_NAME"
echo "App Service Plan ........: $APP_SERVICE_PLAN_NAME"
echo "Web App (Container) .....: $WEB_APP_NAME"
echo "URL da aplicação ........: $WEBAPP_URL"
echo "----------------------------------------------------"
echo ""
echo "⚠️ LEMBRETE IMPORTANTE:"
echo " - Use estes nomes no azure-pipelines.yml:"
echo "     acrName: $ACR_NAME"
echo "     webAppName: $WEB_APP_NAME"
echo " - E o mesmo Resource Group: $RESOURCE_GROUP_NAME"
echo ""
