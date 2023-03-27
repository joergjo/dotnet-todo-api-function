using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace TodoApi
{
    public static class TodoApi
    {
        [FunctionName(nameof(GetSingle))]
        public static IActionResult GetSingle(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos/{id}")] HttpRequest request,
            string id,
            [CosmosDB(
                databaseName: "todos",
                collectionName: "items",
                Id = "{id}",
                PartitionKey = "{id}",
                ConnectionStringSetting = "CosmosDBConnection")] TodoItem item,
            ILogger log)
        {
            log.LogInformation("Looking up todo '{id}'", id);
            if (item is null)
            {
                log.LogInformation("Cannot find todo '{id}'", id);
                return new NotFoundResult();
            }
            log.LogInformation("Found todo '{id}'", id);
            return new OkObjectResult(item);
        }

        [FunctionName(nameof(GetMany))]
        public static IActionResult GetMany(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos")] HttpRequest request,
            [CosmosDB(
                databaseName: "todos",
                collectionName: "items",
                SqlQuery = "SELECT TOP 100 * FROM c",
                ConnectionStringSetting = "CosmosDBConnection")] IEnumerable<TodoItem> items,
            ILogger log)
            {
                log.LogInformation("Looking up todos, found {count}", items.Count());
                return new OkObjectResult(items);
            }


        [FunctionName(nameof(Create))]
        public static async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todos")] TodoItem item,
            [CosmosDB(
                databaseName: "todos",
                collectionName: "items",
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<TodoItem> collector,
            ILogger log)
        {
            item.Id = Guid.NewGuid().ToString();
            log.LogInformation("Creating new todo");
            await collector.AddAsync(item);
            log.LogInformation("Created todo {id}", item.Id);
            return new CreatedResult($"/api/todos/{item.Id}", item);
        }

        [FunctionName(nameof(Update))]
        public static async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todos/{id}")] TodoItem item,
            [CosmosDB(
                databaseName: "todos",
                collectionName: "items",
                Id = "{id}",
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<TodoItem> collector,
            ILogger log)
        {
            log.LogInformation("Updating todo");
            await collector.AddAsync(item);
            log.LogInformation("Updated todo {id}", item.Id);
            return new OkObjectResult(item);
        }

        [FunctionName(nameof(Delete))]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todos/{id}")] TodoItem item,
            string id,
            [CosmosDB(
                databaseName: "todos",
                collectionName: "items",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("Deleting todo '{id}'", id);
            var uri = UriFactory.CreateDocumentUri("todos", "items", id);
            await client.DeleteDocumentAsync(uri, new RequestOptions { PartitionKey = new PartitionKey(id) });
            log.LogInformation("Deleted todo {id}", id);
            return new StatusCodeResult(StatusCodes.Status204NoContent);
        }
    }
}
