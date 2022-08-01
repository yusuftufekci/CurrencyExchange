using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.Core.Requests;
using FluentValidation;


namespace CurrencyExchange.Service.Validations
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestValidator()
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage("Must be email address").NotNull().WithMessage("{PropertyName} is required").NotEmpty().WithMessage("{PropertyName} cant be none!");
            RuleFor(x => x.Password).NotEmpty().WithMessage("{PropertyName} can't be empty}").Length(9, 15).WithMessage("Password must be atleast 9 character").NotNull().WithMessage("{PropertyName} can't be null");
            RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} cant be empty").NotNull().WithMessage("{PropertyName}} cant be null");
            RuleFor(x => x.Surname).NotEmpty().WithMessage("{PropertyName} cant be empty").NotNull().WithMessage("{PropertyName}} cant be null");

        }

    }
}
