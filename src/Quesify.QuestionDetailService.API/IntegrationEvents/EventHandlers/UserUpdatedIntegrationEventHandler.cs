using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Quesify.QuestionDetailService.API.Aggregates.Users;
using Quesify.QuestionDetailService.API.Data.Contexts;
using Quesify.QuestionDetailService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.QuestionDetailService.API.IntegrationEvents.EventHandlers;

public class UserUpdatedIntegrationEventHandler : IIntegrationEventHandler<UserUpdatedIntegrationEvent>
{
    private readonly QuestionDetailContext _context;
    private readonly ILogger<UserCreatedIntegrationEventHandler> _logger;

    public UserUpdatedIntegrationEventHandler(
        QuestionDetailContext context,
        ILogger<UserCreatedIntegrationEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task HandleAsync(UserUpdatedIntegrationEvent integrationEvent)
    {
        var user = await _context.Users.AsQueryable().Where(o => o.Id == integrationEvent.UserId).FirstOrDefaultAsync();
        if (user == null)
        {
            _logger.LogError("The user {UserId} was not found.", integrationEvent.UserId);
            return;
        }

        user.ProfileImageUrl = integrationEvent.ProfileImageUrl;

        var filter = Builders<User>.Filter.Eq(field => field.Id, user.Id);
        await _context.Users.ReplaceOneAsync(filter, user);
    }
}
