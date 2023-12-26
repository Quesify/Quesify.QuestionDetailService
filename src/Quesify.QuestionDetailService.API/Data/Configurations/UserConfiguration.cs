using MongoDB.Bson.Serialization;
using Quesify.QuestionDetailService.API.Aggregates.Users;

namespace Quesify.QuestionDetailService.API.Data.Configurations;

public class UserConfiguration
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<User>(config =>
        {
            config.AutoMap();
            config.MapIdMember(o => o.Id);
            config.MapMember(o => o.Id).SetElementName("_id");
            config.MapMember(o => o.UserName).SetElementName("user_name");
            config.MapMember(o => o.Score).SetElementName("score");
            config.MapMember(o => o.ProfileImageUrl).SetElementName("profile_image_url");
        });
    }
}
