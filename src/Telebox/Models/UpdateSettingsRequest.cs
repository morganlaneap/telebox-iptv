using Telebox.Data;

namespace Telebox.Models;

public class UpdateSettingsRequest
{
    public List<Setting> Settings { get; set; } = default!;
}