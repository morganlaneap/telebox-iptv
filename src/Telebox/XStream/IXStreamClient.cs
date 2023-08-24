using Telebox.XStream.Entities;

namespace Telebox.XStream
{
    public interface IXStreamClient
    {
        Task<EpgResponse> GetAllEpgAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken = default);
        Task<List<Live>> GetLiveCategoriesAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken = default);
        Task<List<Channels>> GetLiveStreamsAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken = default);
        Task<List<Channels>> GetLiveStreamsByCategoriesAsync(ConnectionInfo connectionInfo, string categoryId, CancellationToken cancellationToken = default);
        Task<XtreamPanel> GetPanelAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken = default);
        Task<EpgResponse> GetShortEpgForStreamAsync(ConnectionInfo connectionInfo, string streamId, CancellationToken cancellationToken = default);
        Task<PlayerApi> GetUserAndServerInfoAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken = default);
        Task<List<Channels>> GetVodStreamsAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken = default);
    }
}
