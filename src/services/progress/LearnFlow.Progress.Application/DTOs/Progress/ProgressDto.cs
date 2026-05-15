namespace LearnFlow.Progress.Application.DTOs.Progress;

public record ProgressDto(
    string Id,
    string StudentId,
    string CourseId,
    string Status,
    double CompletionPercentage,
    int TotalLessons,
    int CompletedLessonsCount,
    List<ModuleProgressDto> ModuleProgresses,
    DateTime StartedAt,
    DateTime? CompletedAt,
    DateTime LastUpdatedAt
);

public record ModuleProgressDto(
    string ModuleId,
    int TotalLessons,
    int CompletedLessonsCount,
    bool IsCompleted,
    List<LessonProgressDto> LessonProgresses
);

public record LessonProgressDto(
    string LessonId,
    bool IsCompleted,
    DateTime? CompletedAt
);