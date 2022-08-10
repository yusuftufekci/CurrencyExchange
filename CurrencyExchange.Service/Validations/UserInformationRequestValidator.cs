using CurrencyExchange.Core.Requests;
using FluentValidation;

namespace CurrencyExchange.Service.Validations
{
    public class UserInformationRequestValidator : AbstractValidator<UserInformationRequest>
    {
        public UserInformationRequestValidator()
        {

        }
    }
}
