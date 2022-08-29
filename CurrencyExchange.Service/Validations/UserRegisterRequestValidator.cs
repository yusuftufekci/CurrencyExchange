using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.Requests;
using FluentValidation;


namespace CurrencyExchange.Service.Validations
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestValidator()
        {
            RuleFor(x => x.UserEmail).EmailAddress().WithMessage("{PropertyName} "+ValidatorConstantsMessages.EmailValidatorConstant).NotNull().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantNull).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantEmpty);
            RuleFor(x => x.Password).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantEmpty).Length(9, 15).WithMessage("{PropertyName} "+ValidatorConstantsMessages.PasswordValidatorConstant).NotNull().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantNull);
            RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantEmpty).NotNull().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantNull);
            RuleFor(x => x.Surname).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantEmpty).NotNull().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantNull);

        }

    }
}
