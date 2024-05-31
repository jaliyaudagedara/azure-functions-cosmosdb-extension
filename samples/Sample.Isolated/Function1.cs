using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Sample.Isolated;

public class Function1
{
    private readonly ILogger _logger;

    public Function1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Function1>();
    }

    [Function("Function1")]
    public void Run(
        [CosmosDBMongoTrigger(databaseName: "formio-dev-001",
            collectionName: "submissions",
            ConnectionStringKey = "CosmosDB",
            LeaseCollectionName = "leases")] byte[] input)
    {
        var docs = BsonSerializer.Deserialize<BsonDocument>(input)["results"].AsBsonArray.ToList();
        _logger.LogInformation("Documents modified " + docs.Count);
        foreach (var item in docs)
        {
            _logger.LogInformation("Document " + item.ToJson());
        }
    }
}