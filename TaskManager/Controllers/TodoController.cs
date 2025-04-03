using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskManager.Controllers
{
    [Route("api/todo")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        // ✅ GET: api/todo - Get all tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDo>>> GetTodos()
        {
            return await _context.ToDos.Include(t => t.Category).Include(t => t.Status).ToListAsync();
        }

        // ✅ GET: api/todo/{id} - Get a single task by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDo>> GetTodo(int id)
        {
            var todo = await _context.ToDos.FindAsync(id);
            if (todo == null)
            {
                return NotFound(new { message = "Task not found" });
            }
            return todo;
        }

        // ✅ POST: api/todo - Create a new task
        [HttpPost]
        public async Task<ActionResult<ToDo>> CreateTodo(ToDo todo)
        {
            if (todo == null)
            {
                return BadRequest(new { message = "Invalid task data" });
            }

            _context.ToDos.Add(todo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
        }

        // ✅ PUT: api/todo/{id} - Update a task
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, ToDo todo)
        {
            if (id != todo.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ToDos.Any(e => e.Id == id))
                {
                    return NotFound(new { message = "Task not found" });
                }
                throw;
            }

            return NoContent();
        }

        // ✅ DELETE: api/todo/{id} - Delete a task
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.ToDos.FindAsync(id);
            if (todo == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            _context.ToDos.Remove(todo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
