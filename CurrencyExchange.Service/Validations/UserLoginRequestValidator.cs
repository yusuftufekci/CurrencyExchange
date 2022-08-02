using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.Core.Requests;
using FluentValidation;
namespace CurrencyExchange.Service.Validations
{
    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(x => x.UserEmail).EmailAddress().WithMessage("Must be email address").NotNull().WithMessage("{PropertyName} is required").NotEmpty().WithMessage("{PropertyName} cant be none!");
            RuleFor(x => x.Password).NotEmpty().WithMessage("{PropertyName} can't be empty}").Length(9, 15).WithMessage("Password must be atleast 9 character").NotNull().WithMessage("{PropertyName} can't be null");

        }
    }
}
