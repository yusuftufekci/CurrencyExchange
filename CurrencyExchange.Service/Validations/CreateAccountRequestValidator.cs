using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.Requests;
using FluentValidation;

namespace CurrencyExchange.Service.Validations
{
    public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountRequestValidator()
        {
            RuleFor(x => x.AccountName).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantEmpty).NotNull().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantNull);

        }
    }
}
