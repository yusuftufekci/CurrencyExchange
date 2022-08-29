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
            RuleFor(x => x.CoinToSell).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantEmpty).NotNull().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantNull);
            RuleFor(x => x.Amount).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantEmpty).NotNull().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantEmpty).GreaterThan(0.001);

        }

    }
}
