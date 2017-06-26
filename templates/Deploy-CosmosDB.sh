#!/bin/bash

# Set variables for the new account, database, and collection

echo '*****************************************************'
echo '             variables'
echo '*****************************************************'
echo ' '

resourceGroupName='WellToldStory'
location=westeurope
subscription=dc72a57c-74e5-475e-9f09-46f912070bf9
name='wtscosmosdb-dev'
databaseName='WTSSocialMedia'
smsCollName='SMSData'
facebookCollName='FacebookData'
usersCollName='UsersData'

echo '*****************************************************'
echo '             DB account'
echo '*****************************************************'
echo ' '

# Check if the DB Account name already exists, it must be unique for the domain "documents.azure.com"
exists=(az cosmosdb check-name-exists --name $name)
if [ $exists != 'true' ]
then
# Create a GlobalDocumentDB API Cosmos DB account
az cosmosdb create \
    --name $name \
    --kind GlobalDocumentDB \
    --resource-group $resourceGroupName \
    --max-interval 10 \
    --max-staleness-prefix 200
fi

echo '*****************************************************'
echo '             create DB'
echo '*****************************************************'
echo ' '

# Create a database
exists=$(az cosmosdb database exists --db-name $databaseName --resource-group-name $resourceGroupName --name $name)
if [ $exists != 'true' ]
then
az cosmosdb database create \
    --name $name \
    --db-name $databaseName \
    --resource-group $resourceGroupName
fi

echo '*****************************************************'
echo '             create collections'
echo '*****************************************************'
echo ' '

echo 'create collection profile'

# Create a collection

exists=$(az cosmosdb collection exists \
    --collection-name $smsCollName \
    --db-name $databaseName  \
    --resource-group-name $resourceGroupName \
    --name $name)

if [ $exists != 'true' ]
then
az cosmosdb collection create \
    --collection-name $smsCollName \
    --name $name \
    --db-name $databaseName \
    --resource-group $resourceGroupName
fi

# Create a collection

exists=$(az cosmosdb collection exists \
    --collection-name $facebookCollName \
    --db-name $databaseName  \
    --resource-group-name $resourceGroupName \
    --name $name)

if [ $exists != 'true' ]
then
az cosmosdb collection create \
    --collection-name $facebookCollName \
    --name $name \
    --db-name $databaseName \
    --resource-group $resourceGroupName
fi

# Create a collection

exists=$(az cosmosdb collection exists \
    --collection-name $usersCollName \
    --db-name $databaseName  \
    --resource-group-name $resourceGroupName \
    --name $name)

if [ $exists != 'true' ]
then
az cosmosdb collection create \
    --collection-name $usersCollName \
    --name $name \
    --db-name $databaseName \
    --resource-group $resourceGroupName
fi

