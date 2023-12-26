using Quesify.SharedKernel.Core.Entities;

namespace Quesify.QuestionDetailService.API.Aggregates.Questions;

public class Question : AggregateRoot
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public Guid UserId { get; set; }

    public int Score { get; set; }

    public DateTime CreationDate { get; set; }

    public Question()
    {
        Title = null!;
        Body = null!;
        Score = 0;
    }
}
