namespace Quesify.QuestionDetailService.API.Models;

public class QuestionAnswerUserResponse
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public int Score { get; set; }

    public string? ProfileImageUrl { get; set; }

    public QuestionAnswerUserResponse()
    {
        UserName = null!;
    }
}
