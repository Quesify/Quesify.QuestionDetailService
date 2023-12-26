using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Quesify.QuestionDetailService.API.Aggregates.Answers;
using Quesify.QuestionDetailService.API.Aggregates.Questions;
using Quesify.QuestionDetailService.API.Aggregates.Users;
using Quesify.QuestionDetailService.API.Data.Configurations;
using Quesify.SharedKernel.MongoDB.Contexts;
using Quesify.SharedKernel.MongoDB.Models;

namespace Quesify.QuestionDetailService.API.Data.Contexts;

public class QuestionDetailContext : MongoDbContext
{
    public IMongoCollection<User> Users => Set<User>();

    public IMongoCollection<Question> Questions => Set<Question>();

    public IMongoCollection<Answer> Answers => Set<Answer>();

    public QuestionDetailContext(MongoDbOptions mongoDbOptions) : base(mongoDbOptions) { }

    public void Configure()
    {
        //BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        AnswerConfiguration.Configure();
        QuestionConfiguration.Configure();
        UserConfiguration.Configure();
    }
}
