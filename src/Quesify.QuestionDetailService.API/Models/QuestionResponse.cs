namespace Quesify.QuestionDetailService.API.Models;

public class QuestionResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public int Score { get; set; }

    public QuestionUserResponse User { get; set; }

    public List<QuestionAnswerResponse> Answers { get; set; }

    public DateTime CreationDate { get; set; }

    public QuestionResponse()
    {
        Title = null!;
        Body = null!;
        User = null!;
        Answers = [];
    }
}