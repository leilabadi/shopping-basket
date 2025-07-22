namespace Shop.BasketDomain.Domain.Services;

public interface ITaxCalculationService
{
    decimal CalculateVAT(decimal amount);
    decimal AddVAT(decimal amount);
    decimal RemoveVAT(decimal amount);
}