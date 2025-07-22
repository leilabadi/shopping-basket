using Shop.BasketDomain.Domain.Model;

namespace Shop.BasketDomain.Domain.Services;

public interface IPriceCalculationService
{
    decimal CalculateTotalCost(Basket basket);
}
