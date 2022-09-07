﻿using CurrencyExchange.Core.Constants;
using CurrencyExchange.Core.Requests;
using FluentValidation;
namespace CurrencyExchange.Service.Validations
{
    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(x => x.UserEmail).EmailAddress().WithMessage("{PropertyName} " + ValidatorConstantsMessages.EmailValidatorConstant);
            RuleFor(x => x.Password).Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$").WithMessage("{PropertyName} " + ValidatorConstantsMessages.PasswordValidatorConstant);
        }
    }
}
