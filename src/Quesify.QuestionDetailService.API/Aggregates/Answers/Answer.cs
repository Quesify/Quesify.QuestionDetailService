using Quesify.SharedKernel.Core.Entities;

namespace Quesify.QuestionDetailService.API.Aggregates.Answers;

public class Answer : AggregateRoot
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public string Body { get; set; }

    public Guid UserId { get; set; }

    public int Score { get; set; }

    public DateTime CreationDate { get; set; }

    public Answer()
    {
        Body = null!;
        Score = 0;
    }
}
