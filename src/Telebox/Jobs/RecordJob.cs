using Quartz;
using Telebox.Data;

namespace Telebox.Jobs;

public class RecordJob : IJob
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RecordJob> _logger;

    public RecordJob(ApplicationDbContext context, ILogger<RecordJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var outputPath = _context.Settings.FirstOrDefault(x => x.Name == "outputPath")?.Value;
        if (outputPath is null || !Directory.Exists(outputPath))
        {
            outputPath = Environment.CurrentDirectory;
        }
        
        var recordingId = int.Parse(context.JobDetail.Key.Name);
        var recording = _context.Recordings.FirstOrDefault(r => r.Id == recordingId);
        
        LogInfo(recordingId, "Started recording job");
        
        if (recording is null)
        {
            LogInfo(recordingId, "Recording does not exist - skipping job");
            return;
        }

        var connection = _context.Connections.FirstOrDefault(c => c.Id == recording.ConnectionId);

        if (connection is null)
        {
            LogInfo(recordingId, "Connection does not exist - skipping job");
            return;
        }
        
        // Setup stream URL
        var streamUrl = $"{connection.ServerUrl}/{connection.Username}/{connection.Password}/{recording.StreamId}";

        // Update recording status
        recording.Status = RecordingStatus.Recording;
        _context.Recordings.Update(recording);
        await _context.SaveChangesAsync();
        
        LogInfo(recordingId, "Changed status to 'Recording'");
        
        // Calculate recording duration
        var duration = recording.EndTime - DateTime.Now;
        LogInfo(recordingId, $"Will record for {duration.TotalMinutes} minutes");
        
        // Trigger recording
        var success = await DownloadAndSaveVideoAsync(recordingId, streamUrl, outputPath, recording.FileName, duration, 0);
        
        // Update recording status
        recording.Status = success ? RecordingStatus.Recorded : RecordingStatus.Errored;
        _context.Recordings.Update(recording);
        await _context.SaveChangesAsync();
        
        LogInfo(recordingId, "Updated recording status");

        LogInfo(recordingId, $"Recording completed - {(success ? "success" : "fail")}");
    }
    
    private async Task<bool> DownloadAndSaveVideoAsync(int recordingId, string videoUrl, string outputFilePath, string fileName, TimeSpan downloadDuration, int retryNumber)
    {
        LogInfo(recordingId, $"Starting attempt #{retryNumber}");
        
        if (retryNumber == 20)
        {
            LogError(recordingId, new TimeoutException("Max retries reached"), "Failed to download after 20 retries. Abandoning recording");
            return false;
        }

        var timeStampOnStart = DateTime.Now;
        
        try
        {
            using var httpClient = new HttpClient();
            await using var videoStream = await httpClient.GetStreamAsync(videoUrl);
            await using var fileStream = File.Create($"{outputFilePath}/{retryNumber}-{fileName}");
            LogInfo(recordingId, $"Saving to output file {outputFilePath}/{retryNumber}-{fileName}");

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Start the timer to cancel the download after the specified duration
            await using var timer = new Timer(_ => cancellationTokenSource.Cancel(), null, downloadDuration, Timeout.InfiniteTimeSpan);
            
            LogInfo(recordingId, "Starting stream copy with buffer size 81920");
            await videoStream.CopyToAsync(fileStream, 81920, cancellationToken);
            LogInfo(recordingId, "Completed stream copy");
        }
        catch (OperationCanceledException)
        {
            LogInfo(recordingId, "Recording cancelled after the specified duration");
            return true;
        }
        catch (Exception ex)
        {
            var nextRetryNumber = retryNumber + 1;
            LogError(recordingId, ex, $"An error occurred while recording. Attempting retry number {nextRetryNumber}");

            // Update duration to record based off what we have so far
            var timeSinceStarted = DateTime.Now - timeStampOnStart;
            return await DownloadAndSaveVideoAsync(recordingId, videoUrl, outputFilePath, fileName, downloadDuration - timeSinceStarted, nextRetryNumber);
        }

        LogInfo(recordingId, $"Recording failed");
        return false;
    }

    private void LogInfo(int recordingId, string message) =>
        _logger.LogInformation("[Recording #{RecordingId}] {Message}", recordingId, message);
    
    private void LogError(int recordingId, Exception ex, string message) =>
        _logger.LogError(ex, "[Recording #{RecordingId}] {Message}", recordingId, message);
}