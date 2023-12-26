using Quesify.EventBus.Events;

namespace Quesify.QuestionDetailService.API.IntegrationEvents.Events;

public class AnswerCreatedIntegrationEvent : IntegrationEvent
{
    public Guid QuestionId { get; set; }

    public Guid AnswerId { get; set; }

    public string Body { get; set; }

    public Guid UserId { get; set; }

    public DateTime AnswerCreationDate { get; set; }

    public AnswerCreatedIntegrationEvent(
        Guid questionId,
        Guid answerId,
        string body,
        Guid userId,
        DateTime answerCreationDate,
        Guid id,
        DateTime creationDate)
        : base(id, creationDate)
    {
        QuestionId = questionId;
        AnswerId = answerId;
        Body = body;
        UserId = userId;
        AnswerCreationDate = answerCreationDate;
    }
}
