using FluentValidation;
using UserService.DataTransferObjects;

namespace UserService.Validators;

public class UserValidator : AbstractValidator<UserRequest>
{
    public UserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kullanici adi bos olamaz.")
            .MaximumLength(100).WithMessage("Kullanici adi 100 karakterden uzun olamaz.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email bos olamaz.")
            .EmailAddress().WithMessage("Gecerli bir email adresi giriniz.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon bos olamaz.")
            .MaximumLength(20).WithMessage("Telefon 20 karakterden uzun olamaz.");
    }
}