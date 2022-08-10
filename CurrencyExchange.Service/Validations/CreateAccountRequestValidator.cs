using CurrencyExchange.Core.Requests;
using FluentValidation;

namespace CurrencyExchange.Service.Validations
{
    public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountRequestValidator()
        {
            RuleFor(x => x.AccountName).NotEmpty().WithMessage("{PropertyName} cant be empty").NotNull().WithMessage("{PropertyName}} cant be null");

        }
    }
}
