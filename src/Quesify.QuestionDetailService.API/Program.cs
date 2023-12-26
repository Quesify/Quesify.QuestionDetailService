using Quesify.QuestionDetailService.API.Data.Contexts;
using Quesify.QuestionDetailService.API.IntegrationEvents.EventHandlers;
using Quesify.QuestionDetailService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebServices(builder.Configuration, builder.Host);

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var appStartedLogMessage = "{ApplicationName} started. Hosting environment: {Environment}";
logger.LogInformation(appStartedLogMessage);

var eventBus = app.Services.GetRequiredService<IEventBus>();
await eventBus.SubscribeAsync<QuestionCreatedIntegrationEvent, QuestionCreatedIntegrationEventHandler>();
await eventBus.SubscribeAsync<QuestionVotedIntegrationEvent, QuestionVotedIntegrationEventHandler>();
await eventBus.SubscribeAsync<AnswerCreatedIntegrationEvent, AnswerCreatedIntegrationEventHandler>();
await eventBus.SubscribeAsync<AnswerVotedIntegrationEvent, AnswerVotedIntegrationEventHandler>();
await eventBus.SubscribeAsync<UserCreatedIntegrationEvent, UserCreatedIntegrationEventHandler>();
await eventBus.SubscribeAsync<UserUpdatedIntegrationEvent, UserUpdatedIntegrationEventHandler>();

using (var serviceScope = app.Services.CreateScope())
{
    var mongoDb = serviceScope.ServiceProvider.GetRequiredService<QuestionDetailContext>();
    mongoDb.Configure();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseEnableRequestBuffering();

app.UseRequestTime();

app.UsePushSerilogPropertiesMiddleware();

app.UseCustomHttpLoggingMiddleware();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler(o => { });

app.Run();
