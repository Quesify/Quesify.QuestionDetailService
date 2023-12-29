using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Quesify.QuestionDetailService.API.Aggregates.Questions;
using Quesify.QuestionDetailService.API.Aggregates.Users;
using Quesify.QuestionDetailService.API.Data.Contexts;
using Quesify.QuestionDetailService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.Caching;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.QuestionDetailService.API.IntegrationEvents.EventHandlers;

public class QuestionVotedIntegrationEventHandler : IIntegrationEventHandler<QuestionVotedIntegrationEvent>
{
    private readonly QuestionDetailContext _context; 
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyGenerator _cacheKeyGenerator;
    private readonly ILogger<QuestionVotedIntegrationEventHandler> _logger;


    public QuestionVotedIntegrationEventHandler(
        QuestionDetailContext context,
        ICacheService cacheService,
        ICacheKeyGenerator cacheKeyGenerator,
        ILogger<QuestionVotedIntegrationEventHandler> logger)
    {
        _context = context;
        _logger = logger;
        _cacheService = cacheService;
        _cacheKeyGenerator = cacheKeyGenerator;
    }

    public async Task HandleAsync(QuestionVotedIntegrationEvent integrationEvent)
    {
        await UpdateNewQuestionScoreAsync(integrationEvent);
        await UpdateUserScoreAsync(integrationEvent);
    }

    private async Task UpdateNewQuestionScoreAsync(QuestionVotedIntegrationEvent integrationEvent)
    {
        var question = await _context.Questions.AsQueryable().Where(o => o.Id == integrationEvent.QuestionId).FirstOrDefaultAsync();
        if (question == null)
        {
            _logger.LogError("The question {QuestionId} was not found.", integrationEvent.QuestionId);
            return;
        }

        question.Score = integrationEvent.NewQuestionScore;

        var filter = Builders<Question>.Filter.Eq(field => field.Id, question.Id);
        await _context.Questions.ReplaceOneAsync(filter, question);


        var cacheKey = _cacheKeyGenerator.GenerateCacheKey("GetQuestionDetailById", integrationEvent.QuestionId);
        if (await _cacheService.AnyAsync(cacheKey))
        {
            await _cacheService.RemoveAsync(cacheKey);
        }
    }

    private async Task UpdateUserScoreAsync(QuestionVotedIntegrationEvent integrationEvent)
    {
        var user = await _context.Users.AsQueryable().Where(o => o.Id == integrationEvent.QuestionOwnerUserId).FirstOrDefaultAsync();
        if (user == null)
        {
            _logger.LogError("The user {UserId} was not found.", integrationEvent.QuestionOwnerUserId);
            return;
        }

        user.Score += integrationEvent.UserScoreAction;

        var filter = Builders<User>.Filter.Eq(field => field.Id, user.Id);
        await _context.Users.ReplaceOneAsync(filter, user);
    }
}
