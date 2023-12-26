using Quesify.EventBus.Events;

namespace Quesify.QuestionDetailService.API.IntegrationEvents.Events;

public class AnswerVotedIntegrationEvent : IntegrationEvent
{
    public Guid QuestionId { get; set; }

    public Guid AnswerId { get; set; }

    public Guid AnswerOwnerUserId { get; set; }

    public int NewAnswerScore { get; set; }

    public int UserScoreAction { get; set; }

    public AnswerVotedIntegrationEvent(
        Guid questionId,
        Guid answerId,
        Guid answerOwnerUserId,
        int newAnswerScore,
        int userScoreAction,
        Guid id,
        DateTime creationDate)
        : base(id, creationDate)
    {
        QuestionId = questionId;
        AnswerId = answerId;
        AnswerOwnerUserId = answerOwnerUserId;
        NewAnswerScore = newAnswerScore;
        UserScoreAction = userScoreAction;
    }
}
