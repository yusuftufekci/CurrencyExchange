using CurrencyExchange.Core.DTOs;
using CurrencyExchange.Core.Entities.Authentication;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Requests;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CurrencyExchange.Core.HelperFunctions;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using CurrencyExchange.Service.Exceptions;

namespace CurrencyExchange.Service.Services
{
    public class AuthenticationService<T> : IAuthenticationService<T> where T : class
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IPasswordRepository _passwordRepository;
        public AuthenticationService(IUserRepository repository, IUnitOfWork unitOfWork, IPasswordRepository passwordRepository, ITokenRepository tokenRepository)
        {
            _userRepository = repository;
            _UnitOfWork = unitOfWork;
            _passwordRepository = passwordRepository;
            _tokenRepository = tokenRepository;
        }


        public async Task<CustomResponseDto<TokenDto>> UserLogin(UserLoginRequest userLoginRequest, string IpAdress)
        {
            var user = await _userRepository.Where(p => p.UserEmail == userLoginRequest.UserEmail).SingleOrDefaultAsync();

            var user_param = await _passwordRepository.Where(p => p.User == user).SingleOrDefaultAsync();

            if (user_param == null)
            {
                return CustomResponseDto<TokenDto>.Fail(404, new List<string> { "Username or Password is wrong!" });
            }


            if (!PasswordHash.VerifyPasswordHash(userLoginRequest.Password, user_param.PasswordHash, user_param.PasswordSalt))
            {
                return CustomResponseDto<TokenDto>.Fail(404, new List<string> { "Username or Password is wrong!" });
            }


            string token = CreateToken.GenerateToken(user);

            var controlToken = await _tokenRepository.Where(p => p.UserId == user.Id).SingleOrDefaultAsync();
            user.IpAddress = IpAdress;
            user.ModifiedDate = DateTime.UtcNow;

            if (controlToken == null)
            {
                UserToken userToken = new UserToken();
                userToken.Token = token;
                userToken.UserId = user.Id;
                userToken.ExpDate = new JwtSecurityTokenHandler().ReadToken(token).ValidTo;
                await _tokenRepository.AddAsync(userToken);
                await _UnitOfWork.CommitAsync();
            }
            else
            {
                controlToken.Token = token;
                controlToken.ExpDate = new JwtSecurityTokenHandler().ReadToken(token).ValidTo;
                controlToken.ModifiedDate = DateTime.UtcNow;
                await _UnitOfWork.CommitAsync();
            }

            return CustomResponseDto<TokenDto>.Succes(201, new TokenDto { Token = token });
        }

        public async Task<CustomResponseDto<NoContentDto>> UserRegister(UserRegisterRequest userRegisterRequest, string IpAdress)
        {

            var userExist = await _userRepository.Where(p => p.UserEmail == userRegisterRequest.UserEmail).SingleOrDefaultAsync();
            if (userExist != null)
            {
                throw new ClientSideException($"Email already used");
                // return CustomResponseDto<NoContentDto>.Fail(404,"Email address not found");
            }

            User user = new User();
            user.Name = userRegisterRequest.Name;
            user.Surname = userRegisterRequest.Surname;
            user.UserEmail = userRegisterRequest.UserEmail;
            user.IpAddress = IpAdress;

            Password password = new Password();

            PasswordHash.CreatePasswordHash(userRegisterRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);
            password.User = user;
            password.PasswordHash = passwordHash;
            password.PasswordSalt = passwordSalt;
            await _userRepository.AddAsync(user);
            await _passwordRepository.AddAsync(password);
            await _UnitOfWork.CommitAsync();
            return CustomResponseDto<NoContentDto>.Succes(201);
        }
    }

}
