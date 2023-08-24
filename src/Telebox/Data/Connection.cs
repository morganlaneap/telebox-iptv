namespace Telebox.Data;

public class Connection
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string ServerUrl { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
}