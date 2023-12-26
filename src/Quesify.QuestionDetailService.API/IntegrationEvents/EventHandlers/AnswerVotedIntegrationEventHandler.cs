using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Quesify.QuestionDetailService.API.Aggregates.Answers;
using Quesify.QuestionDetailService.API.Aggregates.Users;
using Quesify.QuestionDetailService.API.Data.Contexts;
using Quesify.QuestionDetailService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.Caching;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.QuestionDetailService.API.IntegrationEvents.EventHandlers;

public class AnswerVotedIntegrationEventHandler : IIntegrationEventHandler<AnswerVotedIntegrationEvent>
{
    private readonly QuestionDetailContext _context;
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyGenerator _cacheKeyGenerator;
    private readonly ILogger<AnswerVotedIntegrationEventHandler> _logger;

    public AnswerVotedIntegrationEventHandler(
        QuestionDetailContext context,
        ICacheService cacheService,
        ICacheKeyGenerator cacheKeyGenerator,
        ILogger<AnswerVotedIntegrationEventHandler> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _cacheKeyGenerator = cacheKeyGenerator;
        _logger = logger;
    }

    public async Task HandleAsync(AnswerVotedIntegrationEvent integrationEvent)
    {
        var cacheKey = _cacheKeyGenerator.GenerateCacheKey("GetQuestionDetailById", integrationEvent.QuestionId);
        if (await _cacheService.AnyAsync(cacheKey))
        {
            await _cacheService.RemoveAsync(cacheKey);
        }

        await UpdateNewAnswerScoreAsync(integrationEvent);
        await UpdateUserScoreAsync(integrationEvent);
    }

    private async Task UpdateNewAnswerScoreAsync(AnswerVotedIntegrationEvent integrationEvent)
    {
        var answer = await _context.Answers.AsQueryable().Where(o => o.Id == integrationEvent.AnswerId).FirstOrDefaultAsync();
        if (answer == null)
        {
            _logger.LogError("The answer {AnswerId} was not found.", integrationEvent.AnswerId);
            return;
        }

        answer.Score = integrationEvent.NewAnswerScore;

        var filter = Builders<Answer>.Filter.Eq(field => field.Id, answer.Id);
        await _context.Answers.ReplaceOneAsync(filter, answer);
    }

    private async Task UpdateUserScoreAsync(AnswerVotedIntegrationEvent integrationEvent)
    {
        var user = await _context.Users.AsQueryable().Where(o => o.Id == integrationEvent.AnswerOwnerUserId).FirstOrDefaultAsync();
        if (user == null)
        {
            _logger.LogError("The user {UserId} was not found.", integrationEvent.AnswerOwnerUserId);
            return;
        }

        user.Score += integrationEvent.UserScoreAction;

        var filter = Builders<User>.Filter.Eq(field => field.Id, user.Id);
        await _context.Users.ReplaceOneAsync(filter, user);
    }
}
