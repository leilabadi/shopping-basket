namespace Shop.BasketDomain.Application.Dtos;

public record GetBasketResponse(bool IsSuccessful, BasketDto? Dto)
{
    public static GetBasketResponse Success(BasketDto dto) => new(true, dto);
    public static GetBasketResponse Failure() => new(false, null);
}
