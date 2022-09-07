using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.Requests;
using FluentValidation;

namespace CurrencyExchange.Service.Validations
{
    public class BuyCoinRequestValidator : AbstractValidator<BuyCoinRequest>
    {
        public BuyCoinRequestValidator()
        {
            RuleFor(x => x.BuyWIthThisCoin).Matches(@"\b[A-Z]+(?:\s+[A-Z]+)*\b").WithMessage("{PropertyName} " + ValidatorConstantsMessages.CoinNameConstant);
            RuleFor(x => x.CoinToBuy).Matches(@"\b[A-Z]+(?:\s+[A-Z]+)*\b").WithMessage("{PropertyName} " + ValidatorConstantsMessages.CoinNameConstant);
            RuleFor(x => x.Amount).GreaterThan(0.001).WithMessage("{PropertyName} " + ValidatorConstantsMessages.CoinBuyAmount);
        }
    }
}
