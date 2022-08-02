using CurrencyExchange.Core.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Service.Validations
{
    public class SellCryptoCoinRequestValidator : AbstractValidator<SellCryptoCoinRequest>
    {
        public SellCryptoCoinRequestValidator()
        {
            RuleFor(x => x.UserEmail).EmailAddress().WithMessage("Must be email address").NotNull().WithMessage("{PropertyName} is required").NotEmpty().WithMessage("{PropertyName} cant be none!");
            RuleFor(x => x.CoinToSell).NotEmpty().WithMessage("{PropertyName} cant be empty").NotNull().WithMessage("{PropertyName}} cant be null");
            RuleFor(x => x.Amount).NotEmpty().WithMessage("{PropertyName} cant be empty").NotNull().WithMessage("{PropertyName}} cant be null").GreaterThan(0.001);

        }

    }
}
