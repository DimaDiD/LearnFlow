using FluentAssertions;
using LearnFlow.Progress.Domain.Enums;
using LearnFlow.Progress.Infrastructure.Persistence.Repositories;
using ProgressEntity = LearnFlow.Progress.Domain.Entities.Progress;

namespace LearnFlow.Progress.IntegrationTests;

public class ProgressRepositoryTests : IClassFixture<MongoDbFixture>
{
    private readonly ProgressRepository _repository;

    public ProgressRepositoryTests(MongoDbFixture fixture)
    {
        _repository = new ProgressRepository(fixture.Database);
    }

    private static ProgressEntity CreateTestProgress(
        string studentId = "507f1f77bcf86cd799439011",
        string courseId = "507f1f77bcf86cd799439012")
    {
        var modules = new List<(string ModuleId, List<string> LessonIds)>
        {
            ("module-1", new List<string> { "lesson-1", "lesson-2", "lesson-3", "lesson-4" })
        };

        return ProgressEntity.Create(studentId, courseId, "Test Course", modules);
    }

    [Fact]
    public async Task InsertAsync_ThenRead_PersistsProgressCorrectly()
    {
        // Arrange
        var progress = CreateTestProgress(
            studentId: "507f1f77bcf86cd799439011",
            courseId: "507f1f77bcf86cd799439012");

        // Act
        await _repository.InsertAsync(progress);
        var retrieved = await _repository.GetByStudentAndCourseAsync(
            "507f1f77bcf86cd799439011",
            "507f1f77bcf86cd799439012");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.TotalLessons.Should().Be(4);
        retrieved.CompletionPercentage.Should().Be(0);
        retrieved.Status.Should().Be(ProgressStatus.NotStarted);
        retrieved.ModuleProgresses.Should().HaveCount(1);
        retrieved.ModuleProgresses[0].LessonProgresses.Should().HaveCount(4);
    }

    [Fact]
    public async Task UpdateAsync_AfterCompletingLesson_PersistsUpdatedPercentage()
    {
        // Arrange
        var progress = CreateTestProgress(
            studentId: "607f1f77bcf86cd799439011",
            courseId: "607f1f77bcf86cd799439012");
        await _repository.InsertAsync(progress);

        // Act
        progress.CompleteLesson("module-1", "lesson-1");
        await _repository.UpdateAsync(progress);

        // Читаємо назад із бази
        var retrieved = await _repository.GetByStudentAndCourseAsync(
            "607f1f77bcf86cd799439011",
            "607f1f77bcf86cd799439012");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.CompletedLessonsCount.Should().Be(1);
        retrieved.CompletionPercentage.Should().Be(25);
        retrieved.Status.Should().Be(ProgressStatus.InProgress);
    }
}