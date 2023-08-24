using Microsoft.AspNetCore.Mvc;
using Telebox.Data;
using Telebox.Models;
using Telebox.XStream;
using ConnectionInfo = Telebox.XStream.ConnectionInfo;

namespace Telebox.Controllers;

[ApiController]
public class ConnectionXStreamController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ConnectionXStreamController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("connections/{connectionId}/xstream/panel")]
    public async Task<IActionResult> GetPanelInfo(int connectionId, CancellationToken cancellationToken)
    {
        var connection = _context.Connections.FirstOrDefault(c => c.Id == connectionId);
        if (connection is null)
            return NotFound();
        var connectionInfo = new ConnectionInfo(connection.ServerUrl, connection.Username, connection.Password);
        using var client = new XStreamClient(new XStreamJsonReader());
        var panelInfo = await client.GetPanelAsync(connectionInfo, cancellationToken);
        return Ok(panelInfo);
    }
    
    [HttpGet]
    [Route("connections/{connectionId}/xstream/epg")]
    public async Task<IActionResult> GetAllEpg(int connectionId, CancellationToken cancellationToken)
    {
        var connection = _context.Connections.FirstOrDefault(c => c.Id == connectionId);
        if (connection is null)
            return NotFound();
        var connectionInfo = new ConnectionInfo(connection.ServerUrl, connection.Username, connection.Password);
        using var client = new XStreamClient(new XStreamJsonReader());
        var epg = await client.GetAllEpgAsync(connectionInfo, cancellationToken);
        return Ok(epg.Epg_listings.Select(e => new EpgListing(e)).ToList());
    }
    
    [HttpGet]
    [Route("connections/{connectionId}/xstream/categories")]
    public async Task<IActionResult> GetCategories(int connectionId, CancellationToken cancellationToken)
    {
        var connection = _context.Connections.FirstOrDefault(c => c.Id == connectionId);
        if (connection is null)
            return NotFound();
        var connectionInfo = new ConnectionInfo(connection.ServerUrl, connection.Username, connection.Password);
        using var client = new XStreamClient(new XStreamJsonReader());
        var categories = await client.GetLiveCategoriesAsync(connectionInfo, cancellationToken);
        return Ok(categories.Select(c => new Category(c)).ToList());
    }
    
    [HttpGet]
    [Route("connections/{connectionId}/xstream/categories/{categoryId}/channels")]
    public async Task<IActionResult> GetChannels(int connectionId, string categoryId, CancellationToken cancellationToken)
    {
        var connection = _context.Connections.FirstOrDefault(c => c.Id == connectionId);
        if (connection is null)
            return NotFound();
        var connectionInfo = new ConnectionInfo(connection.ServerUrl, connection.Username, connection.Password);
        using var client = new XStreamClient(new XStreamJsonReader());
        var channels = await client.GetLiveStreamsByCategoriesAsync(connectionInfo, categoryId, cancellationToken);
        return Ok(channels.Select(c => new Channel(c)).ToList());
    }
    
    [HttpGet]
    [Route("connections/{connectionId}/xstream/categories/{categoryId}/channels/{channelId}/epg")]
    public async Task<IActionResult> GetChannelEpg(int connectionId, string categoryId, string channelId, CancellationToken cancellationToken)
    {
        var connection = _context.Connections.FirstOrDefault(c => c.Id == connectionId);
        if (connection is null)
            return NotFound();
        var connectionInfo = new ConnectionInfo(connection.ServerUrl, connection.Username, connection.Password);
        using var client = new XStreamClient(new XStreamJsonReader());
        var epg = await client.GetShortEpgForStreamAsync(connectionInfo, channelId, cancellationToken);
        return Ok(epg.Epg_listings.Select(e => new EpgListing(e)).ToList());
    }
}