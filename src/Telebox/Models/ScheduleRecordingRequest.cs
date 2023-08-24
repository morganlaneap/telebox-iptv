namespace Telebox.Models;

public class ScheduleRecordingRequest
{
    public string Name { get; set; } = default!;
    
    public int StreamId { get; set; }
    
    public int EpgId { get; set; }

    public string ChannelName { get; set; } = default!;
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
}