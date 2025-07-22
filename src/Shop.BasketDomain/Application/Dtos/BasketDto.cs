namespace Shop.BasketDomain.Application.Dtos;

public record BasketDto
(
    Guid BasketId,
    List<BasketItemDto> Items,
    BasketShippingDto Shipping,
    List<string> AppliedDiscounts,
    decimal SubtotalWithoutVat = 0,
    decimal VAT = 0,
    decimal TotalWithVat = 0
);