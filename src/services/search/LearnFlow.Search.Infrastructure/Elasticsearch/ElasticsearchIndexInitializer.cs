using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Logging;
using ElasticHttpMethod = Elastic.Transport.HttpMethod;


namespace LearnFlow.Search.Infrastructure.Elasticsearch;

public static class ElasticsearchIndexInitializer
{
    private const string CoursesIndex = "courses";
    private const string InstructorProfilesIndex = "instructor_profiles";

    public static async Task InitializeAsync(
        ElasticsearchClient client,
        ILogger logger)
    {
        await CreateIndexIfNotExistsAsync(client, logger, CoursesIndex, GetCoursesMappingJson());
        await CreateIndexIfNotExistsAsync(client, logger, InstructorProfilesIndex, GetInstructorProfilesMappingJson());
    }

    private static async Task CreateIndexIfNotExistsAsync(
        ElasticsearchClient client,
        ILogger logger,
        string indexName,
        string mappingJson)
    {
        var existsResponse = await client.Indices.ExistsAsync(indexName);

        if (existsResponse.IsValidResponse)
        {
            logger.LogInformation("Index '{Index}' already exists", indexName);
            return;
        }

        var response = await client.Transport.RequestAsync<StringResponse>(
            ElasticHttpMethod.PUT,
            $"/{indexName}",
            PostData.String(mappingJson));

        if (response.ApiCallDetails.HttpStatusCode == 200)
            logger.LogInformation("Index '{Index}' created successfully", indexName);
        else
            logger.LogError("Failed to create index '{Index}': {Body}",
                indexName, response.Body);
    }

    private static string GetCoursesMappingJson() => """
    {
      "mappings": {
        "properties": {
          "id":              { "type": "keyword" },
          "title":           { "type": "text" },
          "description":     { "type": "text" },
          "category":        { "type": "keyword" },
          "level":           { "type": "keyword" },
          "tags":            { "type": "keyword" },
          "instructorId":    { "type": "keyword" },
          "instructorName":  { "type": "text" },
          "price":           { "type": "float" },
          "enrollmentCount": { "type": "integer" },
          "isPublished":     { "type": "boolean" },
          "publishedAt":     { "type": "date" }
        }
      }
    }
    """;

    private static string GetInstructorProfilesMappingJson() => """
        {
          "mappings": {
            "properties": {
              "userId":   { "type": "keyword" },
              "fullName": { "type": "keyword" }
            }
          }
        }
        """;
}