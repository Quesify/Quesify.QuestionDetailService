using Quesify.QuestionDetailService.API.Aggregates.Questions;
using Quesify.QuestionDetailService.API.Data.Contexts;
using Quesify.QuestionDetailService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.QuestionDetailService.API.IntegrationEvents.EventHandlers;

public class QuestionCreatedIntegrationEventHandler : IIntegrationEventHandler<QuestionCreatedIntegrationEvent>
{
    private readonly QuestionDetailContext _context;
    private readonly ILogger<QuestionCreatedIntegrationEventHandler> _logger;

    public QuestionCreatedIntegrationEventHandler(
        QuestionDetailContext context,
        ILogger<QuestionCreatedIntegrationEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task HandleAsync(QuestionCreatedIntegrationEvent integrationEvent)
    {
        var question = new Question()
        {
            Id = integrationEvent.QuestionId,
            Title = integrationEvent.Title,
            Body = integrationEvent.Body,
            UserId = integrationEvent.UserId,
            Score = 0,
            CreationDate = integrationEvent.QuestionCreationDate
        };

        await _context.Questions.InsertOneAsync(question);
    }
}
