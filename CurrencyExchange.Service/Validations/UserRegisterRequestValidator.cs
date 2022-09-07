using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.Requests;
using FluentValidation;


namespace CurrencyExchange.Service.Validations
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestValidator()
        {
            RuleFor(x => x.UserEmail).EmailAddress().WithMessage("{PropertyName} " + ValidatorConstantsMessages.EmailValidatorConstant);
            RuleFor(x => x.Password).Length(9, 15).WithMessage("{PropertyName} " + ValidatorConstantsMessages.PasswordValidatorConstant);
        }

    }
}
