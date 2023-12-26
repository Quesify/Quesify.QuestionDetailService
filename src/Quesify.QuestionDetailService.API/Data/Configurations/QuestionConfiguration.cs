using MongoDB.Bson.Serialization;
using Quesify.QuestionDetailService.API.Aggregates.Questions;

namespace Quesify.QuestionDetailService.API.Data.Configurations;

public static class QuestionConfiguration
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Question>(config =>
        {
            config.AutoMap();
            config.MapIdMember(o => o.Id);
            config.MapMember(o => o.Id).SetElementName("_id");
            config.MapMember(o => o.Title).SetElementName("title");
            config.MapMember(o => o.Body).SetElementName("body");
            config.MapMember(o => o.UserId).SetElementName("user_id");
            config.MapMember(o => o.Score).SetElementName("score");
            config.MapMember(o => o.CreationDate).SetElementName("creation_date");
        });
    }
}
