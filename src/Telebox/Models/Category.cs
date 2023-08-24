using Telebox.XStream.Entities;

namespace Telebox.Models;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;
    
    public Category() { }

    public Category(Live model)
    {
        Id = int.Parse(model.Category_id);
        Name = model.Category_name;
    }
}