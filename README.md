# Azure Functions Cosmos DB To Do API

This is a sample To Do API written as .NET 6 Azure Function that uses the 
["v2" Cosmos DB bindings](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2?tabs=isolated-process%2Cextensionv4&pivots=programming-language-csharp).

## Deployment

Coming soon...

## Using identity based connections

### Create RBAC assignment in Cosmos DB

Run the following script to assign your own Entra ID user principal (i.e., the one currently logged 
on to the Azure CLI) read and write privileges for Cosmos DB.

```bash
cosmos_contributor='00000000-0000-0000-0000-000000000002'
resource_group='<your-rg>'
cosmos_account='<your-cosmos-account>'
principal_id=$(az ad signed-in-user show --query id -o tsv)
az cosmosdb sql role assignment create \
  --account-name "$cosmos_account" \
  --resource-group "$resource_group \
  --scope "/" \
  --principal-id "$principal_id" \
  --role-definition-id "$cosmos_contributor"
```
This script uses the predefined role `Cosmos DB Built-in Data Contributor`. 
See the [documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac)
for using a different or custom role to access Cosmos DB.

Coming soon...