using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Repositories;
using System.Security.Claims;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly ITodoRepository _repo;

    public TodoController(ITodoRepository repo)
    {
        _repo = repo;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private bool IsAdmin() =>
        User.IsInRole("Admin");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetAll()
    {
        if (IsAdmin())
            return Ok(await _repo.GetAllAsync());

        var userId = GetUserId();
        return Ok((await _repo.GetAllAsync()).Where(t => t.UserId == userId));
    }

    [HttpPost]
    public async Task<ActionResult<Todo>> Create(Todo todo)
    {
        todo.UserId = GetUserId();
        var created = await _repo.CreateAsync(todo);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var todo = await _repo.GetByIdAsync(id);
        if (todo == null) return NotFound();

        if (!IsAdmin() && todo.UserId != GetUserId())
            return Forbid();

        await _repo.DeleteAsync(id);
        return NoContent();
    }
}
