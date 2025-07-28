using Microsoft.AspNetCore.Mvc;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using SmartDesk.Domain.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDesk.API.Controllers;

[ApiController]
[Route("api/todoitems")]
public class TodoItemController : ControllerBase
{
    private readonly ITodoItemRepository _repo;
    private readonly INaturalLanguageTaskParser _parser;

    public TodoItemController(ITodoItemRepository repo, INaturalLanguageTaskParser parser)
    {
        _repo = repo;
        _parser = parser;
    }

    [HttpGet] 
    public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetAll() => 
        Ok((await _repo.GetAllAsync()).Select(item => new TodoItemDto {
            Id = item.Id, Title = item.Title, Description = item.Description,
            DueDate = item.DueDate, Priority = item.Priority, IsCompleted = item.IsCompleted
        }));

    [HttpGet("{id}")] 
    public async Task<ActionResult<TodoItemDto>> GetById(Guid id) {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(new TodoItemDto {
            Id = item.Id, Title = item.Title, Description = item.Description,
            DueDate = item.DueDate, Priority = item.Priority, IsCompleted = item.IsCompleted
        });
    }

    [HttpPost] 
    public async Task<IActionResult> Create([FromBody] TodoItemDto dto) {
        var item = new TodoItem {
            Id = Guid.NewGuid(), Title = dto.Title, Description = dto.Description,
            DueDate = dto.DueDate, Priority = dto.Priority, IsCompleted = dto.IsCompleted
        };
        var created = await _repo.AddAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")] 
    public async Task<IActionResult> Update(Guid id, [FromBody] TodoItemDto dto) {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return NotFound();
        item.Title = dto.Title; item.Description = dto.Description;
        item.DueDate = dto.DueDate; item.Priority = dto.Priority;
        item.IsCompleted = dto.IsCompleted;
        await _repo.UpdateAsync(item);
        return NoContent();
    }

    [HttpDelete("{id}")] 
    public async Task<IActionResult> Delete(Guid id) {
        await _repo.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("parse")] 
    public async Task<ActionResult<TodoItemDto>> Parse([FromBody] string raw) {
        var parsed = await _parser.ParseAsync(raw);
        return Ok(parsed);
    }
}
