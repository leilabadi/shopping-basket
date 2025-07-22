using Shop.BasketDomain.Domain.Model;

namespace Shop.BasketDomain.Domain.Factories;

public class BasketFactory
{
    public Basket CreateBasket(BasketShipping shipping, List<Discount>? discounts = null)
    {
        return new Basket(Guid.NewGuid(), shipping, discounts);
    }
}