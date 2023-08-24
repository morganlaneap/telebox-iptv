namespace Telebox.XStream
{
    public interface IXStreamReader: IDisposable
    {
        Task<T> GetFromApi<T>(ConnectionInfo connectionInfo, XStreamApiEnum xStreamApiEnum, CancellationToken cancellationToken, params string[] extraParams);
    }
}