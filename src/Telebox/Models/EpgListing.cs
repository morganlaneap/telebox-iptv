using Telebox.XStream.Entities;

namespace Telebox.Models;

public class EpgListing
{
    public string Id { get; set; } = default!;
    
    public string EpgId { get; set; } = default!;
    
    public string Title { get; set; } = default!;
    
    public string Language { get; set; } = default!;
    
    public DateTime Start { get; set; }
    
    public DateTime End { get; set; }
    
    public string Description { get; set; } = default!;
    
    public string ChannelId { get; set; } = default!;
    
    public EpgListing() { }

    public EpgListing(Epg_Listings model)
    {
        Id = model.Id;
        EpgId = model.Epg_id;
        Title = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(model.Title));
        Language = model.Lang;
        Start = DateTime.Parse(model.Start);
        End = DateTime.Parse(model.End);
        Description = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(model.Description));
        ChannelId = model.Channel_id;
    }
}