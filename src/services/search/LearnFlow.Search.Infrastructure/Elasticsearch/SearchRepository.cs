using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using LearnFlow.Search.Application.Common.Interfaces;
using LearnFlow.Search.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;

using ElasticHttpMethod = Elastic.Transport.HttpMethod;


namespace LearnFlow.Search.Infrastructure.Elasticsearch;

public class SearchRepository : ISearchRepository
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<SearchRepository> _logger;

    private const string CoursesIndex = "courses";
    private const string InstructorProfilesIndex = "instructor_profiles";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public SearchRepository(ElasticsearchClient client, ILogger<SearchRepository> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task IndexCourseAsync(CourseDocument course, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(course, JsonOptions);

        var response = await _client.Transport.RequestAsync<StringResponse>(
            ElasticHttpMethod.PUT,
            $"/{CoursesIndex}/_doc/{course.Id}",
            PostData.String(json),
            null, ct);

        if (response.ApiCallDetails.HttpStatusCode != 200 &&
            response.ApiCallDetails.HttpStatusCode != 201)
            _logger.LogError("Failed to index course {CourseId}: {Body}",
                course.Id, response.Body);
    }

    public async Task UpdateCourseAsync(CourseDocument course, CancellationToken ct = default)
    {
        await IndexCourseAsync(course, ct);
    }

    public async Task DeleteCourseAsync(string courseId, CancellationToken ct = default)
    {
        var response = await _client.Transport.RequestAsync<StringResponse>(
            ElasticHttpMethod.DELETE,
            $"/{CoursesIndex}/_doc/{courseId}",
            null, null, ct);

        if (response.ApiCallDetails.HttpStatusCode != 200)
            _logger.LogError("Failed to delete course {CourseId}: {Body}",
                courseId, response.Body);
    }

    public async Task<(List<CourseDocument> Items, long Total)> SearchAsync(
        string? query,
        string? category,
        string? level,
        int page,
        int pageSize,
        string? sortBy,
        CancellationToken ct = default)
    {
        var searchBody = BuildSearchBody(query, category, level, page, pageSize, sortBy);
        var json = JsonSerializer.Serialize(searchBody, JsonOptions);

        _logger.LogInformation("Elasticsearch search body: {Body}", json);

        var response = await _client.Transport.RequestAsync<StringResponse>(
            ElasticHttpMethod.POST,
            $"/{CoursesIndex}/_search",
            PostData.String(json),
            null, ct);

        if (response.ApiCallDetails.HttpStatusCode != 200)
        {
            _logger.LogError("Search failed: {Body}", response.Body);
            return (new List<CourseDocument>(), 0);
        }

        return ParseSearchResponse(response.Body);
    }

    public async Task UpsertInstructorProfileAsync(
        string userId,
        string fullName,
        CancellationToken ct = default)
    {
        var profile = new { userId, fullName };
        var json = JsonSerializer.Serialize(profile, JsonOptions);

        await _client.Transport.RequestAsync<StringResponse>(
            ElasticHttpMethod.PUT,
            $"/{InstructorProfilesIndex}/_doc/{userId}",
            PostData.String(json),
            null, ct);
    }

    public async Task<string?> GetInstructorNameAsync(
        string userId,
        CancellationToken ct = default)
    {
        var response = await _client.Transport.RequestAsync<StringResponse>(
            ElasticHttpMethod.GET,
            $"/{InstructorProfilesIndex}/_doc/{userId}",
            null, null, ct);

        if (response.ApiCallDetails.HttpStatusCode != 200)
            return null;

        using var doc = JsonDocument.Parse(response.Body);
        if (!doc.RootElement.TryGetProperty("_source", out var source))
            return null;

        return source.TryGetProperty("fullName", out var fullName)
            ? fullName.GetString()
            : null;
    }

    private static object BuildSearchBody(
        string? query,
        string? category,
        string? level,
        int page,
        int pageSize,
        string? sortBy)
    {
        var mustClauses = new List<object>();
        var filterClauses = new List<object>
        {
            new { term = new { isPublished = true } }
        };

        if (!string.IsNullOrWhiteSpace(query))
        {
            mustClauses.Add(new
            {
                multi_match = new
                {
                    query,
                    fields = new[] { "title^2", "description", "instructorName", "tags" },
                    type = "best_fields",
                    fuzziness = "AUTO"
                }
            });
        }

        if (!string.IsNullOrWhiteSpace(category))
            filterClauses.Add(new { term = new { category } });

        if (!string.IsNullOrWhiteSpace(level))
            filterClauses.Add(new { term = new { level } });

        var boolQuery = mustClauses.Any()
            ? new { must = mustClauses, filter = filterClauses }
            : (object)new { filter = filterClauses };

        var sort = sortBy switch
        {
            "newest" => new object[] { new { publishedAt = new { order = "desc" } } },
            "popular" => new object[] { new { enrollmentCount = new { order = "desc" } } },
            _ => new object[] { "_score" }
        };

        return new
        {
            from = (page - 1) * pageSize,
            size = pageSize,
            query = new { @bool = boolQuery },
            sort
        };
    }

    private static (List<CourseDocument> Items, long Total) ParseSearchResponse(string body)
    {
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        var total = root
            .GetProperty("hits")
            .GetProperty("total")
            .GetProperty("value")
            .GetInt64();

        var items = root
            .GetProperty("hits")
            .GetProperty("hits")
            .EnumerateArray()
            .Select(hit =>
            {
                var source = hit.GetProperty("_source");
                return new CourseDocument
                {
                    Id = source.TryGetProperty("id", out var id) ? id.GetString()! : string.Empty,
                    Title = source.TryGetProperty("title", out var title) ? title.GetString()! : string.Empty,
                    Description = source.TryGetProperty("description", out var desc) ? desc.GetString()! : string.Empty,
                    Category = source.TryGetProperty("category", out var cat) ? cat.GetString()! : string.Empty,
                    Level = source.TryGetProperty("level", out var lvl) ? lvl.GetString()! : string.Empty,
                    Tags = source.TryGetProperty("tags", out var tags)
                        ? tags.EnumerateArray().Select(t => t.GetString()!).ToList()
                        : new List<string>(),
                    InstructorId = source.TryGetProperty("instructorId", out var instrId) ? instrId.GetString()! : string.Empty,
                    InstructorName = source.TryGetProperty("instructorName", out var instrName) ? instrName.GetString()! : string.Empty,
                    Price = source.TryGetProperty("price", out var price) ? price.GetDecimal() : 0,
                    EnrollmentCount = source.TryGetProperty("enrollmentCount", out var count) ? count.GetInt32() : 0,
                    IsPublished = source.TryGetProperty("isPublished", out var published) && published.GetBoolean(),
                    PublishedAt = source.TryGetProperty("publishedAt", out var pubAt) ? pubAt.GetDateTime() : DateTime.MinValue
                };
            })
            .ToList();

        return (items, total);
    }
}