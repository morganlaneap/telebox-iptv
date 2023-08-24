namespace Telebox.Data;

public class Setting
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string Value { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}