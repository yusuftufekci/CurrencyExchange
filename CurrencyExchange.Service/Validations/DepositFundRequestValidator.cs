using CurrencyExchange.Core.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
