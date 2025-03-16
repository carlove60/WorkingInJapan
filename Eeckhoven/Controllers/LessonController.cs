using Eeckhoven.Models;
using Eeckhoven.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Eeckhoven.Controllers;

[ApiController]
[Route("api/lesson")]
public class LessonController : ControllerBase
{
    private readonly LessonRepository _lessonRepository;

    public LessonController(LessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    [HttpGet]
    public ActionResult<ResumeModel> GetLesson(string id)
    {
        if (!Guid.TryParse(id, out var userId))
        {
            return BadRequest("ID cannot be null or empty");
        }

        return Ok(_lessonRepository.GetLessonById(userId));
    }
    
    

    [HttpPost("api/lesson/id={id}")]
    public ActionResult<string> Post(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("ID cannot be null or empty");
        }

        return Ok(id);
    }
}