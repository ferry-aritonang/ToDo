using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.Models;

namespace ToDo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly TodoContext _context;

        public ToDoController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems to get all ToDos
        // GET: api/TodoItems?today=true to get today ToDos
        // GET: api/TodoItems?nextday=true to get nextday ToDos
        // GET: api/TodoItems?currentweek=true to get currentweek ToDos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems(bool? today, bool? nextday,
            bool? currentweek)
        {
            var todoItems = _context.TodoItems.AsQueryable();

            var dtToday = DateTime.Today.Date;
            if (today != null)
            {
                todoItems = _context.TodoItems.Where(i => i.ExpireDateTime.Date == dtToday.Date);
            }

            if (nextday != null)
            {
                var dtNextDay = dtToday.AddDays(1);
                todoItems = _context.TodoItems.Where(i => i.ExpireDateTime.Date == dtNextDay.Date);
            }

            if (currentweek != null)
            {
                var dtFirstDay = dtToday.AddDays(-(int) dtToday.DayOfWeek);
                var dtLastDay = dtFirstDay.AddDays(6);
                todoItems = _context.TodoItems.Where(i =>
                    i.ExpireDateTime.Date >= dtFirstDay && i.ExpireDateTime.Date <= dtLastDay);
            }

            return await todoItems.ToListAsync();
        }

        // GET: api/TodoItems/5 to get specific ToDo
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5 to update, set percent complete, mark ToDo as done by set percent complete to 100
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems to create ToDo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5 to delete ToDo
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return todoItem;
        }

        private bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
