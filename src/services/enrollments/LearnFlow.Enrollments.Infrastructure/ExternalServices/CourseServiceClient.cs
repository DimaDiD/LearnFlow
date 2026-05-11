using LearnFlow.Enrollments.Application.Common.Interfaces;
using LearnFlow.Enrollments.Application.DTOs.Enrollments;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LearnFlow.Enrollments.Infrastructure.ExternalServices;

public class CourseServiceClient : ICourseServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CourseServiceClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CourseServiceClient(HttpClient httpClient, ILogger<CourseServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CourseDetailsDto?> GetCourseAsync(
        string courseId,
        CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/courses/{courseId}", ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Course {CourseId} not found. Status: {StatusCode}",
                    courseId, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var course = JsonSerializer.Deserialize<CourseDetailsDto>(content, JsonOptions);

            return course;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "Failed to reach Course Service for course {CourseId}", courseId);
            throw new InvalidOperationException(
                "Course Service is currently unavailable. Please try again later.");
        }
    }
}