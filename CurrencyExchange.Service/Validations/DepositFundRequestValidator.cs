using CurrencyExchange.Core.Requests;
using FluentValidation;

namespace CurrencyExchange.Service.Validations
{
    internal class DepositFundRequestValidator : AbstractValidator<DepositFundRequest>
    {
        public DepositFundRequestValidator()
        {
            RuleFor(x => x.TotalBalance).NotNull().WithMessage("{PropertyName} is required").NotEmpty().WithMessage("{PropertyName} cant be none!").InclusiveBetween(1,double.MaxValue).WithMessage("{PropertyName} must be greater than 0");

        }
    }
}
