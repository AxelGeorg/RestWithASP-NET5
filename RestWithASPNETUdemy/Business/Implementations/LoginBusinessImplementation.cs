using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using RestWithASPNETUdemy.Configurations;
using RestWithASPNETUdemy.Data.VO;
using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Repository;
using RestWithASPNETUdemy.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace RestWithASPNETUdemy.Business.Implementations
{
    public class LoginBusinessImplementation : ILoginBusiness
    {
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private TokenConfiguration _configuration;

        private IUserRepository _repository;
        private readonly ITokenService _tokenService;

        public LoginBusinessImplementation(TokenConfiguration configuration, IUserRepository repository, ITokenService tokenService)
        {
            _configuration = configuration;
            _repository = repository;
            _tokenService = tokenService;
        }

        public TokenVO ValidateCredentials(UserVO userCredentials)
        {
            User user = _repository.ValidateCredentials(userCredentials);
            if (user == null)
                return null;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            string accessToken = _tokenService.GenerateAccessToken(claims);
            string refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configuration.DaysToExpiry);

            _repository.RefreshUserInfo(user);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenVO(
                true, 
                createDate.ToString(DATE_FORMAT), 
                expirationDate.ToString(DATE_FORMAT), 
                accessToken, 
                refreshToken
                );
        }

        public TokenVO ValidateCredentials(TokenVO token)
        {
            string accessToken = token.AccessToken;
            string refreshToken = token.RefreshToken;

            ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

            string userName = principal.Identity.Name;

            User user = _repository.ValidateCredentials(userName);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return null;

            accessToken = _tokenService.GenerateAccessToken(principal.Claims);
            refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configuration.DaysToExpiry);

            _repository.RefreshUserInfo(user);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenVO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
                );

        }

        public bool RevokeToken(string userName)
        {
            return _repository.RevokeToken(userName);
        }
    }
}
