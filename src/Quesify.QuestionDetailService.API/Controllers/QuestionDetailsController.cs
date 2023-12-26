using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Quesify.QuestionDetailService.API.Data.Contexts;
using Quesify.QuestionDetailService.API.Models;
using Quesify.SharedKernel.AspNetCore.Controllers;
using Quesify.SharedKernel.Caching;
using Quesify.SharedKernel.Utilities.Exceptions;

namespace Quesify.QuestionDetailService.API.Controllers;

[Route("api/v1/question-details")]
[ApiController]
public class QuestionDetailsController : BaseController
{
    private readonly QuestionDetailContext _context;
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyGenerator _cacheKeyGenerator;

    public QuestionDetailsController(
        QuestionDetailContext context,
        ICacheService cacheService,
        ICacheKeyGenerator cacheKeyGenerator)
    {
        _context = context;
        _cacheService = cacheService;
        _cacheKeyGenerator = cacheKeyGenerator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var cacheKey = _cacheKeyGenerator.GenerateCacheKey("GetQuestionDetailById", id);
        if (await _cacheService.AnyAsync(cacheKey))
        {
            var cacheValue = await _cacheService.GetAsync<QuestionResponse>(cacheKey);
            return OkResponse(data: cacheValue);
        }

        var question = await _context.Questions.AsQueryable().Where(o => o.Id == id).FirstOrDefaultAsync();
        if (question == null)
        {
            throw new NotFoundException($"The question {id} was not found");
        }

        var answers = await _context.Answers
            .AsQueryable()
            .Where(o => o.QuestionId == id)
            .OrderByDescending(o => o.Score)
            .ThenBy(o => o.CreationDate)
            .ToListAsync();

        var userIds = answers.Select(o => o.UserId).Distinct().ToList();
        if (!userIds.Any(o => o == question.UserId))
        {
            userIds.Add(question.UserId);
        }

        var users = await _context.Users.AsQueryable().Where(o => userIds.Contains(o.Id)).ToListAsync();
        var questionUser = users.Where(o => o.Id == question.UserId).FirstOrDefault()!;

        var response = new QuestionResponse()
        {
            Id = question.Id,
            Title = question.Title,
            Body = question.Body,
            Score = question.Score,
            CreationDate = question.CreationDate,
            User = new QuestionUserResponse()
            {
                Id = questionUser.Id,
                UserName = questionUser.UserName,
                Score = questionUser.Score,
                ProfileImageUrl = questionUser.ProfileImageUrl
            }
        };

        foreach (var answer in answers)
        {
            var answerUser = users.FirstOrDefault(o => o.Id == answer.UserId)!;
            response.Answers.Add(new QuestionAnswerResponse()
            {
                Id = answer.Id,
                Body = answer.Body,
                Score = answer.Score,
                CreationDate = answer.CreationDate,
                User = new QuestionAnswerUserResponse()
                {
                    Id = answerUser.Id,
                    UserName =answerUser.UserName,
                    Score = answerUser.Score,
                    ProfileImageUrl = answerUser.ProfileImageUrl
                }
            });
        }

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(60));

        return OkResponse(data: response);
    }
}
