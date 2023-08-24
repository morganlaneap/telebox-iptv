using Microsoft.AspNetCore.Mvc;
using Quartz;
using Telebox.Data;
using Telebox.Extensions;
using Telebox.Jobs;
using Telebox.Models;

namespace Telebox.Controllers;

[ApiController]
public class ConnectionRecordingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ILogger<ConnectionRecordingController> _logger;

    public ConnectionRecordingController(ApplicationDbContext context, ISchedulerFactory factory, ILogger<ConnectionRecordingController> logger)
    {
        _context = context;
        _schedulerFactory = factory;
        _logger = logger;
    }

    [HttpGet]
    [Route("connections/{connectionId}/recordings")]
    public IActionResult GetAll(int connectionId)
    {
        var connection = _context.Connections.FirstOrDefault(c => c.Id == connectionId || connectionId == 0);
        if (connection is null)
            return Ok(Array.Empty<Recording>());
        var recordings = _context.Recordings.Where(r => r.ConnectionId == connectionId || connectionId == 0).ToList();
        return Ok(recordings);
    }
    
    [HttpPost]
    [Route("connections/{connectionId}/recordings")]
    public async Task<IActionResult> Schedule(int connectionId, [FromBody] ScheduleRecordingRequest request)
    {
        var connection = _context.Connections.FirstOrDefault(c => c.Id == connectionId);
        if (connection is null)
            return NotFound();
        var recording = new Recording
        {
            ConnectionId = connectionId,
            Name = request.Name,
            StreamId = request.StreamId,
            EpgId = request.EpgId,
            ChannelName = request.ChannelName,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = RecordingStatus.Scheduled,
            FileName = $"{request.Name.ToSafeFileName()}-{request.ChannelName.ToSafeFileName()}-{request.StartTime:dd-MM-yyyy-HH-mm-ss}.ts",
            CreatedAt = DateTime.UtcNow
        };
        _context.Recordings.Add(recording);
        await _context.SaveChangesAsync();

        var scheduler = await _schedulerFactory.GetScheduler();
        var job = JobBuilder.Create<RecordJob>()
            .WithIdentity(recording.Id.ToString())
            .Build();
        var trigger = TriggerBuilder.Create();
        trigger = recording.StartTime <= DateTime.UtcNow ? 
            trigger.StartNow() : trigger.StartAt(recording.StartTime);
        await scheduler.ScheduleJob(job, trigger.Build());
        
        _logger.LogInformation("New recording created with ID {RecordingID}", recording.Id);
        _logger.LogInformation("Recording job scheduled for {StartTime} to {EndTime}", recording.StartTime, recording.EndTime);
        
        return Created($"/connections/{connectionId}/recordings/{recording.Id}", recording);
    }
    
    [HttpDelete]
    [Route("connections/{connectionId}/recordings/{recordingId}")]
    public async Task<IActionResult> Delete(int connectionId, int recordingId)
    {
        var recording = _context.Recordings.FirstOrDefault(r => r.Id == recordingId);
        if (recording is null) 
            return NoContent();
        _context.Recordings.Remove(recording);
        await _context.SaveChangesAsync();
        
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.DeleteJob(new JobKey(recording.Id.ToString()));
        
        _logger.LogInformation("Recording with ID {RecordingId} and associated job deleted", recording.Id);
        
        return NoContent();
    }
}