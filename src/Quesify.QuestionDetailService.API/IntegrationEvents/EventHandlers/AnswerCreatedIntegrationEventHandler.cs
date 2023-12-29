using Quesify.QuestionDetailService.API.Aggregates.Answers;
using Quesify.QuestionDetailService.API.Aggregates.Questions;
using Quesify.QuestionDetailService.API.Data.Contexts;
using Quesify.QuestionDetailService.API.IntegrationEvents.Events;
using Quesify.QuestionDetailService.API.Models;
using Quesify.SharedKernel.Caching;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.QuestionDetailService.API.IntegrationEvents.EventHandlers;

public class AnswerCreatedIntegrationEventHandler : IIntegrationEventHandler<AnswerCreatedIntegrationEvent>
{
    private readonly QuestionDetailContext _context;
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyGenerator _cacheKeyGenerator;
    private readonly ILogger<AnswerCreatedIntegrationEventHandler> _logger;

    public AnswerCreatedIntegrationEventHandler(
        QuestionDetailContext context,
        ICacheService cacheService,
        ICacheKeyGenerator cacheKeyGenerator,
        ILogger<AnswerCreatedIntegrationEventHandler> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _cacheKeyGenerator = cacheKeyGenerator;
        _logger = logger;
    }

    public async Task HandleAsync(AnswerCreatedIntegrationEvent integrationEvent)
    {
        var answer = new Answer()
        {
            Id = integrationEvent.AnswerId,
            QuestionId = integrationEvent.QuestionId,
            Body = integrationEvent.Body,
            Score = 0,
            UserId = integrationEvent.UserId,
            CreationDate = integrationEvent.AnswerCreationDate,
        };

        await _context.Answers.InsertOneAsync(answer);

        var cacheKey = _cacheKeyGenerator.GenerateCacheKey("GetQuestionDetailById", integrationEvent.QuestionId);
        if (await _cacheService.AnyAsync(cacheKey))
        {
            await _cacheService.RemoveAsync(cacheKey);
        }
    }
}
