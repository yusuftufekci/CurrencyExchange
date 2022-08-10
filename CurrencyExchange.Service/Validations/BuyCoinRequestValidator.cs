using CurrencyExchange.Core.Requests;
using FluentValidation;

namespace CurrencyExchange.Service.Validations
{
    public class BuyCoinRequestValidator : AbstractValidator<BuyCoinRequest>
    {
        public BuyCoinRequestValidator()
        {
            RuleFor(x => x.BuyWIthThisCoin).NotEmpty().WithMessage("{PropertyName} cant be empty").NotNull().WithMessage("{PropertyName}} cant be null");
            RuleFor(x => x.CoinToBuy).NotEmpty().WithMessage("{PropertyName} cant be empty").NotNull().WithMessage("{PropertyName}} cant be null");
            RuleFor(x => x.Amount).NotEmpty().WithMessage("{PropertyName} cant be empty").NotNull().WithMessage("{PropertyName}} cant be null").GreaterThan(0.001);

        }
    }
}
