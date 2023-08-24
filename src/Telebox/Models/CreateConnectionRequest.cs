namespace Telebox.Models;

public class CreateConnectionRequest
{
    public string Name { get; set; } = default!;

    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string ServerUrl { get; set; } = default!;
}