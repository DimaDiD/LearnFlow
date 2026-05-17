using LearnFlow.Search.Application.Common.Interfaces;
using LearnFlow.Shared.Contracts.Events.Identity;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Search.Application.Features.Search.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly ISearchRepository _searchRepository;
    private readonly ILogger<UserRegisteredConsumer> _logger;

    public UserRegisteredConsumer(
        ISearchRepository searchRepository,
        ILogger<UserRegisteredConsumer> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var message = context.Message;

        // Only cache instructors — students don't need to be indexed
        if (message.Role != "Instructor")
            return;

        var fullName = $"{message.FirstName} {message.LastName}";

        await _searchRepository.UpsertInstructorProfileAsync(
            message.UserId,
            fullName,
            context.CancellationToken);

        _logger.LogInformation(
            "Instructor profile cached: {UserId} {FullName}",
            message.UserId, fullName);
    }
}