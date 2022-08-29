using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.Requests;
using FluentValidation;
namespace CurrencyExchange.Service.Validations
{
    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(x => x.UserEmail).EmailAddress().WithMessage("{PropertyName} "+ValidatorConstants.EmailValidatorConstant).NotNull().WithMessage("{PropertyName} "+ValidatorConstants.CantNull).NotEmpty().WithMessage("{PropertyName} "+ ValidatorConstants.CantEmpty);
            RuleFor(x => x.Password).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstants.CantEmpty).Length(9, 15).WithMessage("{PropertyName} "+ValidatorConstants.PasswordValidatorConstant).NotNull().WithMessage("{PropertyName} "+ValidatorConstants.CantNull);

        }
    }
}
