using Microsoft.AspNetCore.Mvc;
using Telebox.Data;
using Telebox.Models;

namespace Telebox.Controllers;

[ApiController]
public class ConnectionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ConnectionController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("connections")]
    public IActionResult GetAll()
    {
        return Ok(_context.Connections.ToList());
    }

    [HttpGet]
    [Route("connections/{connectionId}")]
    public IActionResult GetById(int connectionId)
    {
        var connection = _context.Connections.FirstOrDefault(c => c.Id == connectionId);
        if (connection is null)
            return NotFound();
        return Ok(connection);
    }

    [HttpPost]
    [Route("connections")]
    public IActionResult Create([FromBody] CreateConnectionRequest request)
    {
        var connection = new Connection
        {
            Name = request.Name,
            Username = request.Username,
            Password = request.Password,
            ServerUrl = request.ServerUrl,
            CreatedAt = DateTime.UtcNow
        };
        _context.Connections.Add(connection);
        _context.SaveChanges();
        return Created($"/connections/{connection.Id}", connection);
    }

    [HttpDelete]
    [Route("connections/{connectionId}")]
    public IActionResult Delete(int connectionId)
    {
        var connection = _context.Connections.FirstOrDefault(c => c.Id == connectionId);
        if (connection is null) 
            return NoContent();
        _context.Connections.Remove(connection);
        _context.Recordings.RemoveRange(_context.Recordings.Where(r => r.ConnectionId == connectionId).ToList());
        _context.SaveChanges();
        return NoContent();
    }
}