using Confluent.Kafka;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Quesify.QuestionDetailService.API.Data.Contexts;
using Quesify.QuestionDetailService.API.IntegrationEvents.EventHandlers;
using Quesify.SharedKernel.AspNetCore.Swagger.Filters;
using Quesify.SharedKernel.Caching;
using Quesify.SharedKernel.Caching.Redis;
using Quesify.SharedKernel.EventBus;
using Quesify.SharedKernel.Guids;
using Quesify.SharedKernel.Json;
using Quesify.SharedKernel.MongoDB.Models;
using Quesify.SharedKernel.TimeProviders;
using Quesify.SharedKernel.Utilities.Guards;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(
        this IServiceCollection services,
        IConfiguration configuration,
        ConfigureHostBuilder hostBuilder)
    {
        Guard.Against.Null(services, nameof(services));
        Guard.Against.Null(configuration, nameof(configuration));
        Guard.Against.Null(hostBuilder, nameof(hostBuilder));

        services
           .AddControllers()
           .AddJsonOptions(options =>
           {
               var enumConverter = new JsonStringEnumConverter();
               options.JsonSerializerOptions.Converters.Add(enumConverter);
           })
           .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

        services.AddFluentValidationAutoValidation(options => { options.DisableDataAnnotationsValidation = true; });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Quesify.QuestionDetailService.API", Version = "v1" });
            options.AddLowercaseDocumentFilter();
        });

        services.AddEnableRequestBuffering();

        services.AddRequestTime();

        hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
            .WriteTo.Seq(context.Configuration.GetValue<string>("Seq:ServerUrl")!)
            .WriteTo.Console()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
            .AddCustomEnriches(services);
        });

        services.AddPushSerilogProperties();

        services.AddCustomHttpLogging(o => { });

        services.AddCustomAuthentication(
            configuration.GetValue<string>("AccessTokenOptions:Audience")!,
            configuration.GetValue<string>("AccessTokenOptions:Issuer")!,
            configuration.GetValue<string>("AccessTokenOptions:SecurityKey")!);

        services.AddHttpContextAccessor();
        services.AddHttpContextCurrentPrincipalAccessor();

        services.AddCustomExceptionHandler();

        services.AddGuid(options => { options.UseSequentialGuidGenerator(services); });

        services.AddTimeProvider(options => { options.UseUtcDateTime(services); });

        services.AddJsonSerializer(options => { options.UseNewtonsoft(services, settings => { }); });

        services.AddSecurity(options => { });

        services.AddEventBus(options =>
        {
            options.ProjectName = "Quesify";
            options.ServiceName = "QuestionDetailService";
            options.UseKafka(services, kafkaOptions =>
            {
                kafkaOptions.ProducerConfig.BootstrapServers = "localhost:9092"; //TODO: Read from appsettings.json
                kafkaOptions.ConsumerConfig.BootstrapServers = "localhost:9092";
                kafkaOptions.ConsumerConfig.GroupId = "Quesify.QuestionDetailService.Group";
                kafkaOptions.ConsumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                kafkaOptions.ConsumerConfig.ClientId = "Quesify.QuestionDetailService";
                kafkaOptions.ConsumerConfig.AllowAutoCreateTopics = true;
                kafkaOptions.AdminClientConfig.AllowAutoCreateTopics = true;
                kafkaOptions.AdminClientConfig.BootstrapServers = "localhost:9092";
            });
        });

        services.AddMongoDB<QuestionDetailContext>(options =>
        {
            options.Database = "quesify_question_detail_service";
            options.ConnectionString = "mongodb://localhost:27017"; //TODO: Read from appsettings.json
        });
        
        services.AddCaching(options =>
        {
            options.KeyPrefix = "Quesify.QuestionDetailService";
            options.UseRedis(services, redisOptions =>
            {
                redisOptions.Port = 6379;
                redisOptions.Host = "localhost";
                redisOptions.Database = 0;
            });
        });

        services.AddBinarySerializer();

        services.AddTransient<QuestionCreatedIntegrationEventHandler>();
        services.AddTransient<QuestionVotedIntegrationEventHandler>();
        services.AddTransient<AnswerCreatedIntegrationEventHandler>();
        services.AddTransient<AnswerVotedIntegrationEventHandler>();
        services.AddTransient<UserCreatedIntegrationEventHandler>();
        services.AddTransient<UserUpdatedIntegrationEventHandler>();

        return services;
    }
}
