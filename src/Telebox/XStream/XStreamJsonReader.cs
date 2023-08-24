using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Telebox.XStream
{
    public class XStreamJsonReader : IXStreamReader
    {
        private readonly HttpClient _client;

        public XStreamJsonReader()
        {
            _client = new HttpClient();
        }
        
        public void Dispose()
        {
            _client?.Dispose();
        }

        public async Task<T> GetFromApi<T>(ConnectionInfo connectionInfo, XStreamApiEnum xStreamApiEnum, CancellationToken cancellationToken, params string[] extraParams)
        {
            var jsonContent = await _client.GetStringAsync(new Uri(GetApiUrl(connectionInfo, xStreamApiEnum, extraParams)), cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            jsonContent = TrimJsonAndConvertChannelsToArray(jsonContent);
            return JsonConvert.DeserializeObject<T>(jsonContent);
        }

        private static string TrimJsonAndConvertChannelsToArray(string jsonContent)
        {
            jsonContent = Regex.Replace(jsonContent, @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1");
            jsonContent = Regex.Replace(jsonContent, "\"\\d+\":{", "{", RegexOptions.Multiline);
            jsonContent = jsonContent.Replace("\"available_channels\":{", "\"available_channels\":[");
            jsonContent = jsonContent.Replace("}}}", "}]}");
            return jsonContent;
        }

        protected virtual string GetApiUrl(ConnectionInfo connectionInfo, XStreamApiEnum xStreamApiEnum, params string[] extraParams)
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.Append($"{connectionInfo.Server}");

            switch (xStreamApiEnum)
            {
                case XStreamApiEnum.Player_Api:
                    urlBuilder.Append($"/player_api.php?username={connectionInfo.UserName}&password={connectionInfo.Password}");
                    return urlBuilder.ToString();
                case XStreamApiEnum.LiveCategories:
                    urlBuilder.Append($"/player_api.php?username={connectionInfo.UserName}&password={connectionInfo.Password}&action=get_live_categories");
                    return urlBuilder.ToString();
                case XStreamApiEnum.LiveStreams:
                    urlBuilder.Append($"/player_api.php?username={connectionInfo.UserName}&password={connectionInfo.Password}&action=get_live_streams");
                    return urlBuilder.ToString();
                case XStreamApiEnum.LiveStreamsByCategories:
                    urlBuilder.Append($"/player_api.php?username={connectionInfo.UserName}&password={connectionInfo.Password}&action=get_live_streams&category_id={extraParams[0]}");
                    return urlBuilder.ToString();
                case XStreamApiEnum.Panel_Api:
                    urlBuilder.Append($"/panel_api.php?username={connectionInfo.UserName}&password={connectionInfo.Password}");
                    return urlBuilder.ToString();
                case XStreamApiEnum.VOD_Streams:
                    urlBuilder.Append($"/player_api.php?username={connectionInfo.UserName}&password={connectionInfo.Password}&action=get_vod_streams");
                    return urlBuilder.ToString();
                case XStreamApiEnum.ShortEpgForStream:
                    urlBuilder.Append($"/player_api.php?username={connectionInfo.UserName}&password={connectionInfo.Password}&action=get_short_epg&stream_id={extraParams[0]}");
                    return urlBuilder.ToString();
                case XStreamApiEnum.AllEpg:
                    urlBuilder.Append($"/player_api.php?username={connectionInfo.UserName}&password={connectionInfo.Password}&action=get_simple_data_table");
                    return urlBuilder.ToString();
                default:
                    return string.Empty;
            }
        }
    }
}
