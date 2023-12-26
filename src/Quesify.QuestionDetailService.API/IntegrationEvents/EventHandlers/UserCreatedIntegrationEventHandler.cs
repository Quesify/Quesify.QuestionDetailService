using Quesify.QuestionDetailService.API.Aggregates.Users;
using Quesify.QuestionDetailService.API.Data.Contexts;
using Quesify.QuestionDetailService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.QuestionDetailService.API.IntegrationEvents.EventHandlers;

public class UserCreatedIntegrationEventHandler : IIntegrationEventHandler<UserCreatedIntegrationEvent>
{
    private readonly QuestionDetailContext _context;
    private readonly ILogger<UserCreatedIntegrationEventHandler> _logger;

    public UserCreatedIntegrationEventHandler(
        QuestionDetailContext context,
        ILogger<UserCreatedIntegrationEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task HandleAsync(UserCreatedIntegrationEvent integrationEvent)
    {
        var user = new User()
        {
            Id = integrationEvent.UserId,
            UserName = integrationEvent.UserName,
            Score = 0,
            ProfileImageUrl = null
        };

        await _context.Users.InsertOneAsync(user);
    }
}
