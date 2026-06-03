using FluentValidation;
using OrderService.DataTransferObjects;
using OrderService.Models;

namespace OrderService.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Geçerli bir kullanıcı ID giriniz.");

        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Geçerli bir ürün ID giriniz.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Miktar 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Tek siparişte en fazla 100 adet olabilir.");
    }
}