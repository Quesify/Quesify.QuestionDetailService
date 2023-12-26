namespace Quesify.QuestionDetailService.API.Models;

public class QuestionAnswerResponse
{
    public Guid Id { get; set; }

    public string Body { get; set; }

    public int Score { get; set; }

    public QuestionAnswerUserResponse User { get; set; }

    public DateTime CreationDate { get; set; }

    public QuestionAnswerResponse()
    {
        Body = null!;
        User = null!;
    }
}
