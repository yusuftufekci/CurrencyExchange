using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.Requests;
using FluentValidation;

namespace CurrencyExchange.Service.Validations
{
    internal class DepositFundRequestValidator : AbstractValidator<DepositFundRequest>
    {
        public DepositFundRequestValidator()
        {
            RuleFor(x => x.TotalBalance).NotNull().WithMessage("{PropertyName} "+ValidatorConstantsMessages.CantNull).NotEmpty().WithMessage("{PropertyName} "+ ValidatorConstantsMessages.CantEmpty).InclusiveBetween(1,double.MaxValue).WithMessage("{PropertyName} "+ValidatorConstantsMessages.GreaterThan);

        }
    }
}
