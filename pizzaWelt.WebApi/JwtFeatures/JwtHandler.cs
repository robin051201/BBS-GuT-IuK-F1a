namespace PizzaWelt.JwtFeatures
{
    public class JwtHandler
    {
        private readonly IConfigurationSection _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtHandler(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _jwtSettings = configuration.GetSection("JwtSettings");
            _userManager = userManager;
        }

        public async Task<string> GenerateAccessToken(ApplicationUser user)
        {
            SigningCredentials signingCredentials = GetSigningCredentials();
            List<Claim> claims = await GetClaims(user);
            JwtSecurityToken tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            string token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return token;
        }

        public string GenerateToken(ApplicationUser user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, "2FA") //Role for 2FA SignIn
            };
            JwtSecurityToken tokenOptions = GenerateTokenOptions(GetSigningCredentials(), claims);
            string token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return token;
        }

        public string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidAudience = _jwtSettings.GetSection("validAudience").Value,
                ValidIssuer = _jwtSettings.GetSection("validIssuer").Value,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value)),
                ValidateLifetime = true //here we are saying that we care about the token's expiration date
            };

            JwtSecurityTokenHandler tokenHandler = new();
            ClaimsPrincipal principal;
            SecurityToken securityToken;
            try
            {
                principal = tokenHandler
                    .ValidateToken(token, tokenValidationParameters, out securityToken);
            }
            catch (Exception)
            {
                principal = new();
                return principal;
            }

            return securityToken is not JwtSecurityToken jwtSecurityToken ||
                   !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase)
                ? throw new SecurityTokenException("Invalid token")
                : principal;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidAudience = _jwtSettings.GetSection("validAudience").Value,
                ValidIssuer = _jwtSettings.GetSection("validIssuer").Value,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            JwtSecurityTokenHandler tokenHandler = new();
            ClaimsPrincipal principal =
                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            return securityToken is not JwtSecurityToken jwtSecurityToken ||
                   !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase)
                ? throw new SecurityTokenException("Invalid token")
                : principal;
        }

        private SigningCredentials GetSigningCredentials()
        {
            byte[] key = Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value);
            SymmetricSecurityKey secret = new(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            IList<string> roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            JwtSecurityToken tokenOptions = new(
                _jwtSettings.GetSection("validIssuer").Value,
                _jwtSettings.GetSection("validAudience").Value,
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings.GetSection("expiryInMinutes").Value)),
                signingCredentials: signingCredentials);
            return tokenOptions;
        }
    }
}