using CurrencyExchange.Core.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Service.Validations
{
    public class UserInformationRequestValidator : AbstractValidator<UserInformationRequest>
    {
        public UserInformationRequestValidator()
        {
            RuleFor(x => x.UserEmail).EmailAddress().WithMessage("Must be email address").NotNull().WithMessage("{PropertyName} is required").NotEmpty().WithMessage("{PropertyName} cant be none!");

        }
    }
}
