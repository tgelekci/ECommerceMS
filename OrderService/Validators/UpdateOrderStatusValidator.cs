using FluentValidation;
using OrderService.DataTransferObjects;

namespace OrderService.Validators;

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    private static readonly string[] ValidStatuses = ["Pending", "Shipped", "Delivered", "Cancelled"];

    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Durum bos olamaz.")
            .Must(s => ValidStatuses.Contains(s))
            .WithMessage("Gecerli durumlar: Pending, Shipped, Delivered, Cancelled");
    }
}