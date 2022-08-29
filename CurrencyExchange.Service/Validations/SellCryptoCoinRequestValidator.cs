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
            RuleFor(x => x.CoinToSell).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstants.CantEmpty).NotNull().WithMessage("{PropertyName} "+ValidatorConstants.CantNull);
            RuleFor(x => x.Amount).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstants.CantEmpty).NotNull().WithMessage("{PropertyName} "+ValidatorConstants.CantEmpty).GreaterThan(0.001);

        }

    }
}
