namespace Quesify.QuestionDetailService.API.Models;

public class QuestionUserResponse
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public int Score { get; set; }

    public string? ProfileImageUrl { get; set; }

    public QuestionUserResponse()
    {
        UserName = null!;
    }
}
