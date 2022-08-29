using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using CurrencyExchange.Core.HelperFunctions;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.ConstantsMessages;
using CurrencyExchange.Core.Entities.Log;
using CurrencyExchange.Core.Entities.LogMessages;
using CurrencyExchange.Core.RabbitMqLogger;

namespace CurrencyExchange.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordRepository _passwordRepository;
        private readonly ISenderLogger _logSender;
        private readonly ICommonFunctions _commonFunctions;

        public AuthenticationService(IUserRepository repository, IUnitOfWork unitOfWork,
            IPasswordRepository passwordRepository, ITokenRepository tokenRepository, ISenderLogger logSenderLogger,
            ICommonFunctions commonFunctions)
        {
            _userRepository = repository;
            _unitOfWork = unitOfWork;
            _passwordRepository = passwordRepository;
            _tokenRepository = tokenRepository;
            _logSender = logSenderLogger;
            _commonFunctions = commonFunctions;
        }


        public async Task<CustomResponseDto<TokenDto>> UserLogin(UserLoginRequest userLoginRequest, string ipAddress)
        {
            ResponseMessages responseMessage;
            LogMessages logMessages;
            if (ipAddress is null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("LoginIpAddressNotFound", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage(ConstantResponseMessage.IpAddressNotFound, language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<TokenDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var user = await _userRepository.Where(p => p.UserEmail == userLoginRequest.UserEmail).SingleOrDefaultAsync();
            var userParam = await _passwordRepository.Where(p => p.User == user).SingleOrDefaultAsync();
            if (userParam == null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("LoginUsernameOrPasswordWrong", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage(ConstantResponseMessage.UsernameOrPasswordWrong, language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<TokenDto>.Fail((int)HttpStatusCode.NotFound, new List<string> { responseMessage.Value });
            }
            if (!PasswordHash.VerifyPasswordHash(userLoginRequest.Password, userParam.PasswordHash, userParam.PasswordSalt))
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("LoginUsernameOrPasswordWrong", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage(ConstantResponseMessage.UsernameOrPasswordWrong, language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<TokenDto>.Fail((int)HttpStatusCode.Unauthorized, new List<string> { responseMessage.Value });
            }
            var token = _commonFunctions.GenerateToken(user);

            var controlToken = await _tokenRepository.Where(p => p.UserId == user.Id).SingleOrDefaultAsync();
            user.IpAddress = ipAddress;
            user.ModifiedDate = DateTime.UtcNow;

            if (controlToken == null)
            {
                var userToken = new UserToken
                {
                    Token = token,
                    UserId = user.Id,
                    ExpDate = new JwtSecurityTokenHandler().ReadToken(token).ValidTo
                };
                await _tokenRepository.AddAsync(userToken);
                await _unitOfWork.CommitAsync();
            }
            else
            {
                controlToken.ExpDate = new JwtSecurityTokenHandler().ReadToken(token).ValidTo;
                controlToken.Token = token;
                controlToken.ModifiedDate = DateTime.UtcNow;
               
                await _unitOfWork.CommitAsync();
            }
            logMessages = await _commonFunctions.GetLogResponseMessage("LoginSuccess", language: "en");
            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<TokenDto>.Success(new TokenDto { Token = token });
        }

        public async Task<CustomResponseDto<NoContentDto>> UserRegister(UserRegisterRequest userRegisterRequest, string ipAdress)
        {
            ResponseMessages responseMessage;
            LogMessages logMessages;
            if (ipAdress is null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("RegisterIpAddressNotFound", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage(ConstantResponseMessage.IpAddressNotFound, language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var userExist = await _userRepository.Where(p => p.UserEmail == userRegisterRequest.UserEmail).SingleOrDefaultAsync();
            if (userExist != null)
            {
                logMessages = await _commonFunctions.GetLogResponseMessage("RegisterUserAlreadyExist", language: "en");
                responseMessage = await _commonFunctions.GetApiResponseMessage(ConstantResponseMessage.UserAlreadyExist, language: "en");
                _logSender.SenderFunction("Log", logMessages.Value);
                return CustomResponseDto<NoContentDto>.Fail((int)HttpStatusCode.NotFound, responseMessage.Value);
            }
            var user = new User
            {
                Name = userRegisterRequest.Name,
                Surname = userRegisterRequest.Surname,
                UserEmail = userRegisterRequest.UserEmail,
                IpAddress = ipAdress
             };

            PasswordHash.CreatePasswordHash(userRegisterRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var password = new Password
            {
                User = user,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await _userRepository.AddAsync(user);
            await _passwordRepository.AddAsync(password);
            await _unitOfWork.CommitAsync();
            logMessages = await _commonFunctions.GetLogResponseMessage("RegisterSuccess", language: "en");
            _logSender.SenderFunction("Log", logMessages.Value);
            return CustomResponseDto<NoContentDto>.Success();
        }
    }
}
