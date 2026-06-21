namespace Backend.Models;

public class Price
{
    public int Id { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
}