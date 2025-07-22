namespace Shop.BasketDomain.Domain.Services;

public class TaxCalculationService : ITaxCalculationService
{
    private const decimal VAT_RATE = 0.20m;

    public decimal CalculateVAT(decimal amount)
    {
        return amount * VAT_RATE;
    }

    public decimal AddVAT(decimal amount)
    {
        return amount * (1 + VAT_RATE);
    }

    public decimal RemoveVAT(decimal amount)
    {
        return amount / (1 + VAT_RATE);
    }
}
