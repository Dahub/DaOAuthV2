﻿using Microsoft.IdentityModel.Tokens;
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

namespace DaOAuthV2.Service
{
    public class JwtService : ServiceBase, IJwtService
    {
        public JwtTokenDto GenerateToken(CreateTokenDto value)
        {
            Validate(value);

            var utcNow = DateTimeOffset.UtcNow;

            IList<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimName.ClientId, value.ClientId));
            claims.Add(new Claim(ClaimName.TokenName, value.TokenName));
            claims.Add(new Claim(ClaimName.Issued, utcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)));
            claims.Add(new Claim(ClaimName.UserPublicId, value.UserPublicId.HasValue ? value.UserPublicId.Value.ToString() : String.Empty));
            claims.Add(new Claim(ClaimName.Name, !String.IsNullOrEmpty(value.UserName) ? value.UserName : String.Empty));
            claims.Add(new Claim(ClaimName.Scope, !String.IsNullOrEmpty(value.Scope) ? value.Scope : String.Empty));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Configuration.Issuer,
                audience: Configuration.Audience,
                claims: claims,
                signingCredentials: creds,
                expires: utcNow.AddMinutes(value.MinutesLifeTime).DateTime);

            JwtTokenDto toReturn = new JwtTokenDto()
            {
                ClientId = value.ClientId,
                Expire = utcNow.AddMinutes(value.MinutesLifeTime).ToUnixTimeSeconds(),
                IsValid = true,
                Scope = value.Scope,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = value.UserName,
                UserPublicId = value.UserPublicId.HasValue?value.UserPublicId.ToString():String.Empty
            };

            return toReturn;
        }

        public JwtTokenDto ExtractToken(ExtractTokenDto tokenInfo)
        {
            Validate(tokenInfo);

            JwtTokenDto toReturn = new JwtTokenDto()
            {
                Token = tokenInfo.Token,
                IsValid = false
            };

            if (String.IsNullOrEmpty(tokenInfo.Token))
            {
                return toReturn;
            }

            var handler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidIssuer = Configuration.Issuer,
                ValidAudience = Configuration.Audience,
                IssuerSigningKeys = new List<SecurityKey>() { new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.SecurityKey)) }
            };

            ClaimsPrincipal pClaim = null;
            SecurityToken validatedToken;
            try
            {
                pClaim = handler.ValidateToken(tokenInfo.Token, validationParameters, out validatedToken);
            }
            catch(Exception ex)
            {
                toReturn.InvalidationCause = ex.Message;
                return toReturn;
            }

            long expire;
            if (!long.TryParse(GetValueFromClaim(pClaim.Claims, ClaimName.Expire), out expire))
                return toReturn;

            toReturn.Expire = expire;

            if (expire < DateTimeOffset.Now.ToUnixTimeSeconds() || GetValueFromClaim(pClaim.Claims, ClaimName.TokenName) != tokenInfo.TokenName)
                return toReturn;

            toReturn.Scope = GetValueFromClaim(pClaim.Claims, ClaimName.Scope);
            toReturn.ClientId = GetValueFromClaim(pClaim.Claims, ClaimName.ClientId);
            toReturn.UserName = GetValueFromClaim(pClaim.Claims, ClaimName.Name);
            toReturn.UserPublicId = GetValueFromClaim(pClaim.Claims, ClaimName.UserPublicId);
            toReturn.IsValid = true;

            return toReturn;
        }

        private string GetValueFromClaim(IEnumerable<Claim> claims, string claimType)
        {
            var claim = claims.Where(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (claim == null)
                return String.Empty;

            return claim.Value;
        }
    }
}