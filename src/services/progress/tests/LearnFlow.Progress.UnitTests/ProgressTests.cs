using FluentAssertions;
using LearnFlow.Progress.Domain.Enums;
using ProgressEntity = LearnFlow.Progress.Domain.Entities.Progress;

namespace LearnFlow.Progress.UnitTests;

public class ProgressTests
{
    private static ProgressEntity CreateProgressWithFourLessons()
    {
        var modules = new List<(string ModuleId, List<string> LessonIds)>
        {
            ("module-1", new List<string> { "lesson-1", "lesson-2", "lesson-3", "lesson-4" })
        };

        return ProgressEntity.Create(
            studentId: "student-1",
            courseId: "course-1",
            courseTitleSnapshot: "Test Course",
            modules: modules);
    }

    [Fact]
    public void Create_NewProgress_StartsAtZeroPercentAndNotStarted()
    {
        // Arrange & Act
        var progress = CreateProgressWithFourLessons();

        // Assert
        progress.CompletionPercentage.Should().Be(0);
        progress.CompletedLessonsCount.Should().Be(0);
        progress.TotalLessons.Should().Be(4);
        progress.Status.Should().Be(ProgressStatus.NotStarted);
        progress.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void CompleteLesson_OneOfFour_Sets25PercentAndInProgress()
    {
        // Arrange
        var progress = CreateProgressWithFourLessons();

        // Act
        var result = progress.CompleteLesson("module-1", "lesson-1");

        // Assert
        result.Should().BeTrue();
        progress.CompletedLessonsCount.Should().Be(1);
        progress.CompletionPercentage.Should().Be(25);
        progress.Status.Should().Be(ProgressStatus.InProgress);
    }

    [Fact]
    public void CompleteLesson_AllFour_Sets100PercentAndCompleted()
    {
        // Arrange
        var progress = CreateProgressWithFourLessons();

        // Act
        progress.CompleteLesson("module-1", "lesson-1");
        progress.CompleteLesson("module-1", "lesson-2");
        progress.CompleteLesson("module-1", "lesson-3");
        progress.CompleteLesson("module-1", "lesson-4");

        // Assert
        progress.CompletedLessonsCount.Should().Be(4);
        progress.CompletionPercentage.Should().Be(100);
        progress.Status.Should().Be(ProgressStatus.Completed);
        progress.CompletedAt.Should().NotBeNull();
        progress.IsFullyCompleted.Should().BeTrue();
    }

    [Fact]
    public void IsFullyCompleted_PartialProgress_ReturnsFalse()
    {
        // Arrange
        var progress = CreateProgressWithFourLessons();

        // Act
        progress.CompleteLesson("module-1", "lesson-1");

        // Assert
        progress.IsFullyCompleted.Should().BeFalse();
    }

    [Fact]
    public void CompleteLesson_SameLessonTwice_IsIdempotent()
    {
        // Arrange
        var progress = CreateProgressWithFourLessons();
        progress.CompleteLesson("module-1", "lesson-1");

        var secondResult = progress.CompleteLesson("module-1", "lesson-1");

        // Assert
        secondResult.Should().BeFalse();
        progress.CompletedLessonsCount.Should().Be(1);
        progress.CompletionPercentage.Should().Be(25);
    }

    [Fact]
    public void CompleteLesson_NonExistentModule_ThrowsInvalidOperationException()
    {
        // Arrange
        var progress = CreateProgressWithFourLessons();

        // Act
        var act = () => progress.CompleteLesson("non-existent-module", "lesson-1");

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(1, 25)]
    [InlineData(2, 50)]
    [InlineData(3, 75)]
    [InlineData(4, 100)]
    public void CompleteLesson_VariousCounts_CalculatesCorrectPercentage(
        int lessonsToComplete, double expectedPercentage)
    {
        // Arrange
        var progress = CreateProgressWithFourLessons();
        var lessons = new[] { "lesson-1", "lesson-2", "lesson-3", "lesson-4" };

        // Act
        for (int i = 0; i < lessonsToComplete; i++)
            progress.CompleteLesson("module-1", lessons[i]);

        // Assert
        progress.CompletionPercentage.Should().Be(expectedPercentage);
    }
}