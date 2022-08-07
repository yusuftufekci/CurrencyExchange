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

        }
    }
}
