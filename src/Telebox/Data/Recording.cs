namespace Telebox.Data;

public enum RecordingStatus
{
    Scheduled = 0,
    Recording = 1,
    Recorded = 2,
    Errored = 3
}

public class Recording
{
    public int Id { get; set; }
    
    public int ConnectionId { get; set; }

    public string Name { get; set; } = default!;
    
    public int StreamId { get; set; }
    
    public int EpgId { get; set; }

    public string ChannelName { get; set; } = default!;
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public RecordingStatus Status { get; set; }

    public string FileName { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
}