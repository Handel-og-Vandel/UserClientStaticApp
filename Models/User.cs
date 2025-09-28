namespace Models;

public class User
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public short PostalCode { get; set; }
    public string? City { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
}