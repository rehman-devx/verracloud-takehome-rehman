namespace Backend.Services;

public interface IHoldingService
{
    Task<List<HoldingDto>> GetAllHoldingsAsync();
    Task<HoldingDto> AddHoldingAsync(CreateHoldingDto dto);
    Task DeleteHoldingAsync(int id);
}

public class HoldingDto
{
    public int Id { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal MarketValue { get; set; }
    public decimal UnrealizedPnL { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateHoldingDto
{
    public string Ticker { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
}