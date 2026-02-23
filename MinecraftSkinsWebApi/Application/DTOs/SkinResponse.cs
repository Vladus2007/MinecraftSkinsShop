public record SkinResponse
{
    // ��� AutoMapper
    public SkinResponse() { }

    
    public SkinResponse(int id, string name, decimal basePriceUsd, decimal finalPriceUsd)
    {
        Id = id;
        Name = name;
        BasePriceUsd = basePriceUsd;
        FinalPriceUsd = finalPriceUsd;
    }

    public int Id { get; init; }
    public string Name { get; init; }
    public decimal BasePriceUsd { get; init; }
    public decimal FinalPriceUsd { get; init; }
}