using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.Requests;
using FluentValidation;

namespace CurrencyExchange.Service.Validations
{
    public class BuyCoinRequestValidator : AbstractValidator<BuyCoinRequest>
    {
        public BuyCoinRequestValidator()
        {
            RuleFor(x => x.BuyWIthThisCoin).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstants.CantEmpty).NotNull().WithMessage("{PropertyName} "+ ValidatorConstants.CantNull);
            RuleFor(x => x.CoinToBuy).NotEmpty().WithMessage("{PropertyName} " + ValidatorConstants.CantEmpty).NotNull().WithMessage("{PropertyName}} cant be null "+ ValidatorConstants.CantNull);
            RuleFor(x => x.Amount).NotEmpty().WithMessage("{PropertyName} " + ValidatorConstants.CantEmpty).NotNull().WithMessage("{PropertyName} "+ValidatorConstants.CantNull).GreaterThan(0.001);

        }
    }
}
