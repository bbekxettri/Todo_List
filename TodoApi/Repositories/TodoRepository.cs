using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly TodoDb _db;

    public TodoRepository(TodoDb db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Todo>> GetAllAsync() =>
        await _db.Todos.ToListAsync();

    public async Task<Todo?> GetByIdAsync(int id) =>
        await _db.Todos.FindAsync(id);

    public async Task<IEnumerable<Todo>> GetCompletedAsync() =>
        await _db.Todos.Where(t => t.IsComplete).ToListAsync();

    public async Task<Todo> CreateAsync(Todo todo)
    {
        _db.Todos.Add(todo);
        await _db.SaveChangesAsync();
        return todo;
    }

    public async Task UpdateAsync(Todo todo)
    {
        _db.Todos.Update(todo);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo != null)
        {
            _db.Todos.Remove(todo);
            await _db.SaveChangesAsync();
        }
    }
}
