using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Quesify.QuestionDetailService.API.Aggregates.Answers;

namespace Quesify.QuestionDetailService.API.Data.Configurations;

public class AnswerConfiguration
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Answer>(config =>
        {
            config.AutoMap();
            config.MapIdMember(o => o.Id);
            config.MapMember(o => o.Id).SetElementName("_id");
            config.MapMember(o => o.Body).SetElementName("body");
            config.MapMember(o => o.UserId).SetElementName("user_id");
            config.MapMember(o => o.Score).SetElementName("score");
            config.MapMember(o => o.CreationDate).SetElementName("creation_date");
        });
    }
}
