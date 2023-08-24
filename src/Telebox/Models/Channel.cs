using Telebox.XStream.Entities;

namespace Telebox.Models;

public class Channel
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;
    
    public int StreamId { get; set; }

    public string EpgChannelId { get; set; } = default!;

    public int CategoryId { get; set; }
    
    public string CategoryName { get; set; } = default!;

    public string IconUrl { get; set; } = default!;
    
    public Channel() { }

    public Channel(Channels model)
    {
        Id = model.Num;
        Name = model.Name;
        StreamId = int.Parse(model.Stream_id);
        EpgChannelId = model.Epg_channel_id;
        CategoryId = int.Parse(model.Category_id);
        CategoryName = model.Category_name;
        IconUrl = model.Stream_icon;
    }
}