namespace Backend.Models;

public class Holding
{
    public int Id { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}