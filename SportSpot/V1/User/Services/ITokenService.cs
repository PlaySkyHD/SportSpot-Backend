﻿using System.Security.Claims;

namespace SportSpot.V1.User
{
    public interface ITokenService
    {
        AccessTokenDto GenerateAccessToken(IEnumerable<Claim> claims);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshToken();
    }
}