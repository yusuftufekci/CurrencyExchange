using CurrencyExchange.Core.Requests;
using FluentValidation;

namespace CurrencyExchange.Service.Validations
{
    public class SellCryptoCoinRequestValidator : AbstractValidator<SellCryptoCoinRequest>
    {
        public SellCryptoCoinRequestValidator()
        {
            RuleFor(x => x.CoinToSell).NotEmpty().WithMessage("{PropertyName} cant be empty").NotNull().WithMessage("{PropertyName}} cant be null");
            RuleFor(x => x.Amount).NotEmpty().WithMessage("{PropertyName} cant be empty").NotNull().WithMessage("{PropertyName}} cant be null").GreaterThan(0.001);

        }

    }
}
