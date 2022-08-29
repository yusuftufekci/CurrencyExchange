using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.Requests;
using FluentValidation;

namespace CurrencyExchange.Service.Validations
{
    public class BuyCoinRequestValidator : AbstractValidator<BuyCoinRequest>
    {
        public BuyCoinRequestValidator()
        {
            RuleFor(x => x.BuyWIthThisCoin).NotEmpty().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantEmpty).NotNull().WithMessage("{PropertyName} "+ ValidatorConstantsMessages.CantNull);
            RuleFor(x => x.CoinToBuy).NotEmpty().WithMessage("{PropertyName} " + ValidatorConstantsMessages.CantEmpty).NotNull().WithMessage("{PropertyName}} cant be null "+ ValidatorConstantsMessages.CantNull);
            RuleFor(x => x.Amount).NotEmpty().WithMessage("{PropertyName} " + ValidatorConstantsMessages.CantEmpty).NotNull().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantNull).GreaterThan(0.001);

        }
    }
}
