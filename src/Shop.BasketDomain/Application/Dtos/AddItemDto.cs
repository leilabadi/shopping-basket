namespace Shop.BasketDomain.Application.Dtos;

public record AddItemDto(Guid ProductId, int Quantity = 1);