namespace PetStoreApi.Models;

public class ApiResponse
{
    public int? Code { get; set; }
    public string? Type { get; set; }
    public string? Message { get; set; }
}

public class Category
{
    public long? Id { get; set; }
    public string? Name { get; set; }
}

public class Pet
{
    public long? Id { get; set; }
    public Category? Category { get; set; }
    public string? Name { get; set; }
    public List<string> PhotoUrls { get; set; } = [];
    public List<Tag>? Tags { get; set; }
    public string? Status { get; set; }
}

public class Tag
{
    public long? Id { get; set; }
    public string? Name { get; set; }
}

public class Order
{
    public long? Id { get; set; }
    public long? PetId { get; set; }
    public int? Quantity { get; set; }
    public DateTime? ShipDate { get; set; }
    public string? Status { get; set; }
    public bool? Complete { get; set; }
}

public class User
{
    public long? Id { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }
    public int? UserStatus { get; set; }
}