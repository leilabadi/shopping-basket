namespace Shop.BasketDomain.Domain.Model;

public class Basket
{
    public Guid Id { get; }
    public BasketShipping Shipping { get; private set; }
    private readonly List<BasketItem> _items = [];
    private readonly List<Discount> _discounts;

    internal Basket(Guid id, BasketShipping shipping, List<Discount>? discounts = null)
    {
        Id = id;
        Shipping = shipping;
        _discounts = discounts ?? [];
    }

    // A read-only view of the items and discounts
    public IReadOnlyList<BasketItem> Items => _items;
    public IReadOnlyList<Discount> Discounts => _discounts;

    public void AddItem(BasketProduct product)
    {
        _items.Add(new BasketItem(product));
    }

    public void AddItem(BasketProduct product, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            AddItem(product);
        }
    }

    public bool RemoveItem(Guid productId)
    {
        var itemToRemove = _items.FirstOrDefault(item => item.Product.Id == productId);
        if (itemToRemove != null)
        {
            _items.Remove(itemToRemove);
            return true;
        }
        return false;
    }

    public void AddDiscount(Discount discount)
    {
        _discounts.Add(discount);
    }

    public void SetShipping(BasketShipping shipping)
    {
        Shipping = shipping;
    }
}