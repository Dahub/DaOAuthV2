using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Globalization;
using DaOAuthV2.Service.Interface;
using DaOAuthV2.Service.DTO;
using DaOAuthV2.Constants;
using Microsoft.Extensions.Logging;

namespace DaOAuthV2.Service
{
    public class JwtService : ServiceBase, IJwtService
    {
        private const int MAIL_TOKEN_LIFETIME_IN_SECONDS = 900;

        public JwtTokenDto GenerateToken(CreateTokenDto value)
        {
            Logger.LogInformation($"Try to generate token for user {value.UserName}");

            Validate(value);

            var utcNow = DateTimeOffset.UtcNow;

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimName.ClientId, value.ClientPublicId));
            claims.Add(new Claim(ClaimName.TokenName, value.TokenName));
            claims.Add(new Claim(ClaimName.Issued, utcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)));         
            claims.Add(new Claim(ClaimName.Name, !String.IsNullOrEmpty(value.UserName) ? value.UserName : String.Empty));
            claims.Add(new Claim(ClaimName.Scope, !String.IsNullOrEmpty(value.Scope) ? value.Scope : String.Empty));
           
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.SecurityKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Configuration.Issuer,
                audience: Configuration.Audience,
                claims: claims,
                signingCredentials: credentials,
                expires: utcNow.AddSeconds(value.SecondsLifeTime).UtcDateTime);

            var toReturn = new JwtTokenDto()
            {
                ClientId = value.ClientPublicId,
                Expire = utcNow.AddSeconds(value.SecondsLifeTime).ToUnixTimeSeconds(),
                IsValid = true,
                Scope = value.Scope,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = value.UserName
            };

            return toReturn;
        }

        public JwtTokenDto ExtractToken(ExtractTokenDto tokenInfo)
        {
            Logger.LogInformation("Try to extract token");

            Validate(tokenInfo);

            var toReturn = new JwtTokenDto()
            {
                Token = tokenInfo.Token,
                IsValid = false
            };

            if (string.IsNullOrEmpty(tokenInfo.Token))
            {
                return toReturn;
            }

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = Configuration.Issuer,
                ValidAudience = Configuration.Audience,
                IssuerSigningKeys = new List<SecurityKey>() { new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.SecurityKey)) }
            };

            ClaimsPrincipal pClaim;

            try
            {
                pClaim = handler.ValidateToken(tokenInfo.Token, validationParameters, out var validatedToken);
            }
            catch(Exception ex)
            {
                toReturn.InvalidationCause = ex.Message;
                return toReturn;
            }

            if (!long.TryParse(GetValueFromClaim(pClaim.Claims, ClaimName.Expire), out var expire))
            {
                return toReturn;
            }

            toReturn.Expire = expire;

            if (expire < DateTimeOffset.Now.ToUnixTimeSeconds() || GetValueFromClaim(pClaim.Claims, ClaimName.TokenName) != tokenInfo.TokenName)
            {
                return toReturn;
            }

            toReturn.Scope = GetValueFromClaim(pClaim.Claims, ClaimName.Scope);
            toReturn.ClientId = GetValueFromClaim(pClaim.Claims, ClaimName.ClientId);
            toReturn.UserName = GetValueFromClaim(pClaim.Claims, ClaimName.Name);
            toReturn.IsValid = true;

            return toReturn;
        }

        private string GetValueFromClaim(IEnumerable<Claim> claims, string claimType)
        {
            var claim = claims.FirstOrDefault(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase));

            return claim == null ? string.Empty : claim.Value;
        }

        public MailJwtTokenDto GenerateMailToken(string userName)
        {
            Logger.LogInformation($"Try to generate mail token for user {userName}");

            var utcNow = DateTimeOffset.UtcNow;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.SecurityKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimName.Issued, utcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)));
            claims.Add(new Claim(ClaimName.Name, !String.IsNullOrEmpty(userName) ? userName : String.Empty));

            var token = new JwtSecurityToken(
                issuer: Configuration.Issuer,
                audience: Configuration.Audience,
                signingCredentials: credentials,
                claims: claims,
                expires: utcNow.AddSeconds(MAIL_TOKEN_LIFETIME_IN_SECONDS).UtcDateTime);

            return new MailJwtTokenDto()
            {
                Expire = utcNow.AddSeconds(MAIL_TOKEN_LIFETIME_IN_SECONDS).ToUnixTimeSeconds(),
                IsValid = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        public MailJwtTokenDto ExtractMailToken(string token)
        {
            Logger.LogInformation("Try to extract mail token");

            var toReturn = new MailJwtTokenDto()
            {
                IsValid = false,
                Token = token
            };

            if (string.IsNullOrEmpty(token))
            {
                return toReturn;
            }

            ClaimsPrincipal pClaim = null;
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = Configuration.Issuer,
                ValidAudience = Configuration.Audience,
                IssuerSigningKeys = new List<SecurityKey>() { new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.SecurityKey)) }
            };

            SecurityToken validatedToken;
            try
            {
                pClaim = handler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (Exception ex)
            {
                toReturn.InvalidationCause = ex.Message;
                return toReturn;
            }

            long expire;
            if (!long.TryParse(GetValueFromClaim(pClaim.Claims, ClaimName.Expire), out expire))
            {
                return toReturn;
            }

            toReturn.Expire = expire;
            toReturn.UserName = GetValueFromClaim(pClaim.Claims, ClaimName.Name);

            if (expire < DateTimeOffset.Now.ToUnixTimeSeconds())
            {
                return toReturn;
            }

            toReturn.IsValid = true;

            return toReturn;
        }
    }
}
