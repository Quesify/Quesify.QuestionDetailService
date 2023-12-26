using Quesify.SharedKernel.Core.Entities;

namespace Quesify.QuestionDetailService.API.Aggregates.Users;

public class User : AggregateRoot
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public int Score { get; set; }

    public string? ProfileImageUrl { get; set; }

    public User()
    {
        UserName = null!;
        Score = 0;
        ProfileImageUrl = null;
    }
}
