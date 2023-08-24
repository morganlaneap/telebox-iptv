using Telebox.XStream.Entities;

namespace Telebox.XStream
{
    public class XStreamClient : IXStreamClient, IDisposable
    {
        private readonly IXStreamReader _ixStreamReader;

        public XStreamClient(IXStreamReader ixStreamReader)
        {
            _ixStreamReader = ixStreamReader;
        }

        public Task<PlayerApi> GetUserAndServerInfoAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken) =>
            _ixStreamReader.GetFromApi<PlayerApi>(connectionInfo, XStreamApiEnum.Player_Api, cancellationToken);

        public Task<XtreamPanel> GetPanelAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken) =>
             _ixStreamReader.GetFromApi<XtreamPanel>(connectionInfo, XStreamApiEnum.Panel_Api, cancellationToken);

        public Task<List<Channels>> GetVodStreamsAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken) =>
             _ixStreamReader.GetFromApi<List<Channels>>(connectionInfo, XStreamApiEnum.VOD_Streams, cancellationToken);

        public Task<List<Channels>> GetLiveStreamsAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken) =>
             _ixStreamReader.GetFromApi<List<Channels>>(connectionInfo, XStreamApiEnum.LiveStreams, cancellationToken);

        public Task<List<Channels>> GetLiveStreamsByCategoriesAsync(ConnectionInfo connectionInfo, string categoryId, CancellationToken cancellationToken) =>
             _ixStreamReader.GetFromApi<List<Channels>>(connectionInfo, XStreamApiEnum.LiveStreamsByCategories, cancellationToken, categoryId);

        public Task<List<Live>> GetLiveCategoriesAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken) =>
             _ixStreamReader.GetFromApi<List<Live>>(connectionInfo, XStreamApiEnum.LiveCategories, cancellationToken);

        public Task<EpgResponse> GetAllEpgAsync(ConnectionInfo connectionInfo, CancellationToken cancellationToken) =>
             _ixStreamReader.GetFromApi<EpgResponse>(connectionInfo, XStreamApiEnum.AllEpg, cancellationToken);

        public Task<EpgResponse> GetShortEpgForStreamAsync(ConnectionInfo connectionInfo, string streamId, CancellationToken cancellationToken) =>
            _ixStreamReader.GetFromApi<EpgResponse>(connectionInfo, XStreamApiEnum.ShortEpgForStream, cancellationToken, streamId);

        public void Dispose()
        {
            _ixStreamReader?.Dispose();
        }

    }
}
