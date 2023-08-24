using Microsoft.AspNetCore.Mvc;
using Telebox.Data;
using Telebox.Models;

namespace Telebox.Controllers;

[ApiController]
public class SettingController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SettingController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("settings")]
    public IActionResult GetAll()
    {
        return Ok(_context.Settings.ToList());
    }

    [HttpPut]
    [Route("settings")]
    public IActionResult Update([FromBody] UpdateSettingsRequest request)
    {
        foreach (var setting in request.Settings)
        {
            var existing = _context.Settings.FirstOrDefault(x => x.Name == setting.Name);
            if (existing is not null)
            {
                existing.Value = setting.Value;
                existing.UpdatedAt = DateTime.Now;
                _context.Settings.Update(existing);
            }
            else
            {
                existing = new Setting
                {
                    Name = setting.Name,
                    Value = setting.Value,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Settings.Add(existing);
            }
        }
        _context.SaveChanges();
        return GetAll();
    }
}