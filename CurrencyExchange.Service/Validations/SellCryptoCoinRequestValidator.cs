using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CurrencyExchange.Service.Validations
{
    public class SellCryptoCoinRequestValidator : AbstractValidator<SellCryptoCoinRequest>
    {
        public SellCryptoCoinRequestValidator()
        {
            RuleFor(x => x.CoinToSell).Matches(@"\b[A-Z]+(?:\s+[A-Z]+)*\b").WithMessage("{PropertyName} " + ValidatorConstantsMessages.CoinNameConstant);
            RuleFor(x => x.Amount).GreaterThan(0.001).WithMessage("{PropertyName} " + ValidatorConstantsMessages.CoinBuyAmount);
        }

    }
}
