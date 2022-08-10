using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using CurrencyExchange.Core.HelperFunctions;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using CurrencyExchange.Core.RabbitMqLogger;

namespace CurrencyExchange.Service.Services
{
    public class AuthenticationService<T> : IAuthenticationService<T> where T : class
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordRepository _passwordRepository;
        private readonly ISenderLogger _sender;

        public AuthenticationService(IUserRepository repository, IUnitOfWork unitOfWork,
            IPasswordRepository passwordRepository, ITokenRepository tokenRepository, ISenderLogger senderLogger)
        {
            _userRepository = repository;
            _unitOfWork = unitOfWork;
            _passwordRepository = passwordRepository;
            _tokenRepository = tokenRepository;
            _sender = senderLogger;
        }


        public async Task<CustomResponseDto<TokenDto>> UserLogin(UserLoginRequest userLoginRequest, string ipAdress)
        {
            if (ipAdress is null)
            {
                _sender.SenderFunction("Log", "Can't get IpAdress from user");
                return CustomResponseDto<TokenDto>.Fail(404, "Can't get IpAdress from user");
            }
            var user = await _userRepository.Where(p => p.UserEmail == userLoginRequest.UserEmail).SingleOrDefaultAsync();
            var userParam = await _passwordRepository.Where(p => p.User == user).SingleOrDefaultAsync();
            if (userParam == null)
            {
                _sender.SenderFunction("Log", "UserLogin request failed. Username or Password is wrong!");
                return CustomResponseDto<TokenDto>.Fail(404, new List<string> { "Username or Password is wrong!" });
            }
            if (!PasswordHash.VerifyPasswordHash(userLoginRequest.Password, userParam.PasswordHash, userParam.PasswordSalt))
            {
                _sender.SenderFunction("Log", "UserLogin request failed. Username or Password is wrong!");
                return CustomResponseDto<TokenDto>.Fail(404, new List<string> { "Username or Password is wrong!" });
            }
            var token = CreateToken.GenerateToken(user);

            var controlToken = await _tokenRepository.Where(p => p.UserId == user.Id).SingleOrDefaultAsync();
            user.IpAddress = ipAdress;
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
                var userToken = new UserToken
                {
                    Token = token,
                    ExpDate = new JwtSecurityTokenHandler().ReadToken(token).ValidTo,
                    ModifiedDate = DateTime.UtcNow
                };
                await _unitOfWork.CommitAsync();
            }
            _sender.SenderFunction("Log", "UserLogin request successfully completed.");
            return CustomResponseDto<TokenDto>.Succes(201, new TokenDto { Token = token });
        }

        public async Task<CustomResponseDto<NoContentDto>> UserRegister(UserRegisterRequest userRegisterRequest, string ipAdress)
        {
            if (ipAdress is null)
            {
                _sender.SenderFunction("Log", "Can't get IpAdress from user");
                return CustomResponseDto<NoContentDto>.Fail(404, "Can't get IpAdress from user");
            }
            var userExist = await _userRepository.Where(p => p.UserEmail == userRegisterRequest.UserEmail).SingleOrDefaultAsync();
            if (userExist != null)
            {
                _sender.SenderFunction("Log", "UserRegister request failed. Email already in used");
                // throw new ClientSideException($"Email already used");
                return CustomResponseDto<NoContentDto>.Fail(404, "Email already in used");
            }
            var user = new User
            {
                Name = userRegisterRequest.Name,
                Surname = userRegisterRequest.Surname,
                UserEmail = userRegisterRequest.UserEmail,
                IpAddress = ipAdress
             };

            PasswordHash.CreatePasswordHash(userRegisterRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);

            Password password = new Password
            {
                User = user,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await _userRepository.AddAsync(user);
            await _passwordRepository.AddAsync(password);
            await _unitOfWork.CommitAsync();
            _sender.SenderFunction("Log", "UserRegister request successfully completed.");
            return CustomResponseDto<NoContentDto>.Succes(201);
        }
    }

}
