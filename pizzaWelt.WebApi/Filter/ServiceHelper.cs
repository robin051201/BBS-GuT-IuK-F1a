namespace PizzaWelt.Filter
{
    public class ServiceHelper
    {
        private ApplicationUser _user;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtHandler _jwtHandler;

        public ServiceHelper(UserManager<ApplicationUser> userManager, JwtHandler jwtHandler)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
        }

        public async Task<bool> IsAccessTokenValid(string token)
        {
            IEnumerable<Claim> claims = Claims(token);
            string email = claims?
                .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
                .Value;

            _user = email != null ? await _userManager.FindByEmailAsync(email) : null;

            return _user?.AccessToken == token;
        }

        private IEnumerable<Claim> Claims(string token) => _jwtHandler.GetPrincipalFromToken(token).Claims;

        public async Task<ApplicationUser> GetUser(string token) =>
            await _userManager.FindByEmailAsync(Claims(token)
            .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
            .Value);
    }
}
