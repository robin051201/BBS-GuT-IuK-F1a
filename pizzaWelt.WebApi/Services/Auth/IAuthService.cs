//using Budget.Plan.AppServices.Controllers.Accounts.Dto;
//using Budget.Plan.AppServices.Dto;
//using Budget.Plan.AppServices.JwtFeatures;
//using Budget.Plan.EmailService;
//using Budget.Plan.Entity.Data;
//using Budget.Plan.Entity.Entities;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.WebUtilities;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Net.Mail;
//using System.Security.Claims;
//using System.Text;
//using System.Text.Encodings.Web;
//using System.Threading.Tasks;
//using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

//namespace Budget.Plan.AppServices.Controllers
//{
//    [Authorize]
//    [Route("api/[controller]")]
//    [Consumes("application/json")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private ApplicationUser _user;
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly JwtHandler _jwtHandler;
//        private readonly EmailSender _emailSender;
//        private readonly UrlEncoder _urlEncoder;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

//        public AuthController(ApplicationDbContext context = null, UserManager<ApplicationUser> userManager = null,
//            SignInManager<ApplicationUser> signInManager = null, RoleManager<IdentityRole> roleManager = null,
//            JwtHandler jwtHandler = null,
//            EmailSender emailSender = null,
//            UrlEncoder urlEncoder = null,
//            IHttpContextAccessor httpContextAccessor = null)
//        {
//            _context = context;
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _roleManager = roleManager;
//            _jwtHandler = jwtHandler;
//            _emailSender = emailSender;
//            _urlEncoder = urlEncoder;
//            _httpContextAccessor = httpContextAccessor;
//        }

//        [AllowAnonymous]
//        [HttpPost("login")]
//        public async Task<IActionResult> LoginUser([FromBody] LoginDto loginCredentials)
//        {
//            _user = await _userManager.FindByEmailAsync(loginCredentials.Email);

//            if (_user == null)
//            {
//                return BadRequest(new AuthResponseDto { ErrorMessage = "Invalid Request. User not found", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//            }

//            UserAccounts user = await _context.UserAccounts
//                .Where(c => c.Email == loginCredentials.Email)
//                .SingleOrDefaultAsync();

//            if (user?.Email != loginCredentials.Email)
//            {
//                return BadRequest(new AuthResponseDto { ErrorMessage = "Login failed", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//            }

//            if (!_user.EmailConfirmed)
//            {
//                await SendEmailConformation(loginCredentials.ClientUrl);
//                return BadRequest(new AuthResponseDto { ErrorMessage = "Email is not confirmed", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//            }

//            if (!await _userManager.CheckPasswordAsync(_user, loginCredentials.Password))
//            {
//                _ = await _userManager.AccessFailedAsync(_user);

//                if (await _userManager.IsLockedOutAsync(_user))
//                {
//                    return Unauthorized(new AuthResponseDto { ErrorMessage = "The account is locked out", AuthSuccessful = false, TwoFactorVerificationEnabled = _user.TwoFactorEnabled, Provider = _user.TwoFactorEnabled ? "App" : null, RefreshToken = null, Token = null });
//                }

//                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication", AuthSuccessful = false, TwoFactorVerificationEnabled = _user.TwoFactorEnabled, Provider = _user.TwoFactorEnabled ? "App" : null, RefreshToken = null, Token = null });
//            }

//            _ = await _signInManager.PasswordSignInAsync(_user, loginCredentials.Password, false, true);

//            string token = _jwtHandler.GenerateToken(_user);

//            return _user.TwoFactorEnabled
//                ? Ok(new AuthResponseDto
//                { AuthSuccessful = true, TwoFactorVerificationEnabled = true, Provider = "App", Token = token, ErrorMessage = null, RefreshToken = null })
//                : Ok(new AuthResponseDto { AuthSuccessful = true, TwoFactorVerificationEnabled = false, Token = token, ErrorMessage = null, RefreshToken = null });
//        }

//        [HttpPost("signout")]
//        public async Task<IActionResult> SignOutUser([FromQuery] string token)
//        {
//            _user = await _userManager.FindByEmailAsync(Claims(token)
//                .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
//                ?.Value);

//            if (_user == null || _user.AccessToken != token)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }
//            _user.AccessToken = null;
//            _user.RefreshToken = null;
//            _user.SecurityStamp = null;
//            _ = await _userManager.UpdateAsync(_user);
//            await _signInManager.SignOutAsync();

//            return Ok();
//        }

//        [HttpGet("signin-microsoft"), AllowAnonymous]
//        public IActionResult MicrosoftSignIn()
//        {
//            string returnUrl = null,
//                provider = MicrosoftAccountDefaults.AuthenticationScheme,
//                redirectUri = Url.Action(nameof(MicrosoftSignInCallback), "Accounts", new { returnUrl });

//            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUri);

//            return new ChallengeResult(provider, properties);
//        }

//        [HttpGet("microsoft-signin-callback"), AllowAnonymous]
//        public async Task<IActionResult> MicrosoftSignInCallback()
//        {
//            AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync("Microsoft");

//            if (!authenticateResult.Succeeded)
//            {
//                return BadRequest();
//            }

//            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
//            //string id = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
//            string email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
//            //string name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);

//            UserAccounts user = await _context.UserAccounts
//                .Where(c => c.Email == email)
//                .SingleOrDefaultAsync();

//            if (user?.Email != email)
//            {
//                return BadRequest(new AuthResponseDto { ErrorMessage = "Invalid Request. User not found", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//            }

//            _user = await _userManager.FindByEmailAsync(email);
//            string token;

//            if (user?.Email == email && _user != null)
//            {
//                if (!_user.EmailConfirmed)
//                {
//                    return Unauthorized(new AuthResponseDto { ErrorMessage = "Email is not confirmed", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//                }

//                if (await _userManager.IsLockedOutAsync(_user))
//                {
//                    return Unauthorized(new AuthResponseDto { ErrorMessage = "The account is locked out", AuthSuccessful = false, TwoFactorVerificationEnabled = _user.TwoFactorEnabled, Provider = _user.TwoFactorEnabled ? "App" : null, RefreshToken = null, Token = null });
//                }

//                SignInResult externalResult = await _signInManager.ExternalLoginSignInAsync("Microsoft",
//                    info.ProviderKey,
//                    false, false);

//                if (!externalResult.Succeeded || externalResult.IsLockedOut || externalResult.IsNotAllowed)
//                {
//                    return Unauthorized(new AuthResponseDto { ErrorMessage = "Login failed", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//                }

//                //token = _jwtHandler.GenerateToken(_user);
//                token = await _jwtHandler.GenerateAccessToken(_user);
//                string refreshToken = _jwtHandler.GenerateRefreshToken();

//                return Ok(new AuthResponseDto { AuthSuccessful = true, TwoFactorVerificationEnabled = true, Provider = "App", Token = token, ErrorMessage = null, RefreshToken = null });
//            }

//            _user = new ApplicationUser
//            {
//                UserAccountsId = user.Id,
//                Email = email,
//                UserName = email.Split('@')[0].ToLower(CultureInfo.InvariantCulture),
//                NormalizedUserName = email.Split('@')[0].ToUpper(CultureInfo.InvariantCulture),
//                DateCreated = DateTimeOffset.UtcNow,
//                DateModified = DateTimeOffset.UtcNow
//            };

//            IdentityResult result = await _userManager.CreateAsync(_user);

//            if (!result.Succeeded)
//            {
//                IEnumerable<string> errors = result.Errors.Select(e => e.Description);

//                return BadRequest(new RegistrationResponseDto { Errors = errors, IsSuccessfulRegistration = false });
//            }

//            IdentityRole role = await _roleManager.FindByIdAsync(user.RoleId);

//            if (role != null)
//            {
//                _ = await _userManager.AddToRoleAsync(_user, role.Name);
//            }

//            result = await _userManager.AddLoginAsync(_user, info);

//            if (!result.Succeeded)
//            {
//                IEnumerable<string> errors = result.Errors.Select(e => e.Description);
//                return BadRequest(new RegistrationResponseDto { Errors = errors, IsSuccessfulRegistration = false });
//            }

//            string clientUri = HttpContext.Request.Scheme + "://"
//                + HttpContext.Request.Host.Value
//                + Url.Action(nameof(ConfirmUserEmail), "accounts");

//            await SendEmailConformation(clientUri);

//            return Created(String.Empty, new RegistrationResponseDto { Errors = null, IsSuccessfulRegistration = true });
//        }

//        [HttpPost("msal-signin"), AllowAnonymous]
//        public async Task<IActionResult> MsalSignIn(MsalMMsalAuthenticationResultDto authResult)
//        {
//            string email = authResult.Account.Username;

//            UserAccounts user = await _context.UserAccounts
//                .Where(c => c.Email == email)
//                .SingleOrDefaultAsync();

//            if (user?.Email != email)
//            {
//                return BadRequest(new AuthResponseDto { ErrorMessage = "Invalid Request. User not found", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//            }

//            _user = await _userManager.FindByEmailAsync(email);
//            string token;

//            if (user?.Email == email && _user != null)
//            {
//                if (!_user.EmailConfirmed)
//                {
//                    await SendEmailConformation(authResult.ClientUrl);

//                    return Unauthorized(new AuthResponseDto { ErrorMessage = "Email is not confirmed", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//                }

//                if (await _userManager.IsLockedOutAsync(_user))
//                {
//                    return Unauthorized(new AuthResponseDto { ErrorMessage = "The account is locked out", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = _user.TwoFactorEnabled ? "App" : null, RefreshToken = null, Token = null });
//                }

//                SignInResult externalResult = await _signInManager.ExternalLoginSignInAsync("Microsoft",
//                    authResult.UniqueId,
//                    false, false);

//                if (!externalResult.Succeeded || externalResult.IsLockedOut || externalResult.IsNotAllowed)
//                {
//                    return Unauthorized(new AuthResponseDto { ErrorMessage = "Login failed", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//                }

//                token = await _jwtHandler.GenerateAccessToken(_user);
//                string refreshToken = _jwtHandler.GenerateRefreshToken();

//                _user.AccessToken = token;
//                _user.RefreshToken = refreshToken;
//                _user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddMinutes(int.MaxValue);
//                _user.SecurityStamp = Guid.NewGuid().ToString();

//                _ = await _userManager.ResetAccessFailedCountAsync(_user);
//                _ = await _userManager.UpdateAsync(_user);

//                return Ok(new AuthResponseDto { AuthSuccessful = true, TwoFactorVerificationEnabled = false, Provider = null, Token = token, ErrorMessage = null, RefreshToken = refreshToken });
//            }

//            _user = new ApplicationUser
//            {
//                UserAccountsId = user.Id,
//                Email = email,
//                UserName = email.Split('@')[0].ToLower(CultureInfo.InvariantCulture),
//                NormalizedUserName = email.Split('@')[0].ToUpper(CultureInfo.InvariantCulture),
//                DateCreated = DateTimeOffset.UtcNow,
//                DateModified = DateTimeOffset.UtcNow
//            };

//            IdentityResult result = await _userManager.CreateAsync(_user);

//            if (!result.Succeeded)
//            {
//                IEnumerable<string> errors = result.Errors.Select(e => e.Description);

//                return BadRequest(new RegistrationResponseDto { Errors = errors, IsSuccessfulRegistration = false });
//            }

//            IdentityRole role = await _roleManager.FindByIdAsync(user.RoleId);

//            if (role != null)
//            {
//                _ = await _userManager.AddToRoleAsync(_user, role.Name);
//            }

//            UserLoginInfo info = new("Microsoft", authResult.UniqueId, "Microsoft");
//            result = await _userManager.AddLoginAsync(_user, info);

//            if (!result.Succeeded)
//            {
//                IEnumerable<string> errors = result.Errors.Select(e => e.Description);
//                return BadRequest(new RegistrationResponseDto { Errors = errors, IsSuccessfulRegistration = false });
//            }

//            await SendEmailConformation(authResult.ClientUrl);

//            return Created(String.Empty, new RegistrationResponseDto { Errors = null, IsSuccessfulRegistration = true });
//        }

//        [HttpPost("register"), AllowAnonymous]
//        public async Task<IActionResult> RegisterUser([FromBody] RegistrationDto registerCredentials)
//        {
//            UserAccounts user = await _context.UserAccounts
//                .Where(c => c.Email == registerCredentials.Email)
//                .SingleOrDefaultAsync();

//            if (user?.Email != registerCredentials.Email)
//            {
//                return BadRequest(new RegistrationResponseDto { Errors = new string[] { "User Registration Failed" }, IsSuccessfulRegistration = false });
//            }

//            _user = new ApplicationUser
//            {
//                UserAccountsId = user.Id,
//                Email = registerCredentials.Email,
//                UserName = registerCredentials.Email.Split('@')[0].ToLower(CultureInfo.InvariantCulture),
//                NormalizedUserName = registerCredentials.Email.Split('@')[0].ToUpper(CultureInfo.InvariantCulture),
//                DateCreated = DateTimeOffset.UtcNow,
//                DateModified = DateTimeOffset.UtcNow
//            };

//            IdentityResult result = await _userManager.CreateAsync(_user, registerCredentials.Password);

//            if (!result.Succeeded)
//            {
//                IEnumerable<string> errors = result.Errors.Select(e => e.Description);

//                return BadRequest(new RegistrationResponseDto { Errors = errors, IsSuccessfulRegistration = false });
//            }

//            await SendEmailConformation(registerCredentials.ClientUrl);

//            IdentityRole role = await _roleManager.FindByIdAsync(user.RoleId);

//            if (role != null)
//            {
//                _ = await _userManager.AddToRoleAsync(_user, role.Name);
//            }

//            return Created(String.Empty, new RegistrationResponseDto { Errors = null, IsSuccessfulRegistration = true });
//        }

//        [HttpPost("sendemailconfirmation"), AllowAnonymous]
//        public async Task<IActionResult> SendEmailConformation([FromBody] RegistrationDto registerCredentials)
//        {
//            _user = await _userManager.FindByEmailAsync(registerCredentials.Email);
//            if (_user == null)
//            {
//                return BadRequest(new AuthResponseDto { ErrorMessage = "Invalid Request. User not found", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//            }
//            if (_user.EmailConfirmed)
//            {
//                return BadRequest(new AuthResponseDto { ErrorMessage = "Email already confirmed", AuthSuccessful = false, TwoFactorVerificationEnabled = _user.TwoFactorEnabled, Provider = _user.TwoFactorEnabled ? "App" : null, RefreshToken = null, Token = null });
//            }

//            await SendEmailConformation(registerCredentials.ClientUrl);

//            return Ok();
//        }

//        [HttpGet("confirmemail"), AllowAnonymous]
//        public async Task<IActionResult> ConfirmUserEmail([FromQuery] string email, [FromQuery] string token)
//        {
//            _user = await _userManager.FindByEmailAsync(email);
//            if (_user == null)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Email Confirmation Request" });
//            }

//            if (_user.EmailConfirmed)
//            {
//                return BadRequest(new { ErrorMessage = "Email already confirmed" });
//            }

//            IdentityResult confirmResult = await _userManager.ConfirmEmailAsync(_user, token);
//            return !confirmResult.Succeeded ? BadRequest(new { ErrorMessage = "Invalid Email Confirmation Request" }) : Ok();
//        }

//        [HttpPost("forgotpassword"), AllowAnonymous]
//        public async Task<IActionResult> ForgotUserPassword([FromBody] ForgotPasswordDto fortgotPasswordCredentials)
//        {
//            _user = await _userManager.FindByEmailAsync(fortgotPasswordCredentials.Email);

//            if (_user == null)
//            {
//                return BadRequest(new { ErrorMessage = "User not found" });
//            }

//            if (!_user.EmailConfirmed)
//            {
//                await SendEmailConformation(fortgotPasswordCredentials.ClientUrl);
//                return BadRequest(new { ErrorMessage = "Email not confirmed" });
//            }

//            List<UserLoginInfo> loginList = (List<UserLoginInfo>)await _userManager.GetLoginsAsync(_user);
//            if (loginList.Count > 0)
//            {
//                return BadRequest(new { ErrorMessage = "User not found" });
//            }

//            await SendPasswordReset(fortgotPasswordCredentials);

//            return Ok();
//        }

//        [HttpGet("verifypasswordrecovery"), AllowAnonymous]
//        public async Task<IActionResult> VerifyPasswordRecovery([FromQuery] string email, [FromQuery] string token)
//        {
//            _user = await _userManager.FindByEmailAsync(email);
//            if (_user == null)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Password Forgot Request" });
//            }

//            if (!_user.EmailConfirmed)
//            {
//                return BadRequest(new { ErrorMessage = "Email not confirmed" });
//            }

//            bool confirmResult = await _userManager.VerifyUserTokenAsync(_user,
//                _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);

//            return !confirmResult ? BadRequest(new { ErrorMessage = "Invalid Password Reset Request" }) : Ok();
//        }

//        [HttpPost("passwordrecovery"), AllowAnonymous]
//        public async Task<IActionResult> ResetUserPassword([FromBody] PasswordRecoveryDto resetPasswordCredentials)
//        {
//            _user = await _userManager.FindByEmailAsync(resetPasswordCredentials.Email);

//            if (_user == null)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }

//            List<UserLoginInfo> loginList = (List<UserLoginInfo>)await _userManager.GetLoginsAsync(_user);
//            if (loginList.Count > 0)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }

//            IdentityResult resetPasswordResult = await _userManager.ResetPasswordAsync(_user,
//                resetPasswordCredentials.Token, resetPasswordCredentials.NewPassword);

//            if (!resetPasswordResult.Succeeded)
//            {
//                IEnumerable<string> errors = resetPasswordResult.Errors.Select(e => e.Description);
//                return BadRequest(new { Errors = errors });
//            }

//            _ = await _userManager.SetLockoutEndDateAsync(_user, DateTimeOffset.UtcNow);

//            return Ok();
//        }

//        [Authorize(Roles = "Administrator, Viewer")]
//        [HttpPost("changepassword")]
//        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordDto changePasswordDtoCredentials)
//        {
//            string token = changePasswordDtoCredentials.Token;
//            _user = await _userManager.FindByEmailAsync(Claims(token)
//                .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
//                ?.Value);

//            if (_user == null || _user.AccessToken != token)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }

//            List<UserLoginInfo> loginList = (List<UserLoginInfo>)await _userManager.GetLoginsAsync(_user);
//            if (loginList.Count > 0)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }

//            IdentityResult result = await _userManager.ChangePasswordAsync(_user,
//                changePasswordDtoCredentials.OldPassword,
//                changePasswordDtoCredentials.ConfirmNewPassword);

//            if (!result.Succeeded)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Password" });
//            }

//            return Ok();
//        }

//        [Authorize(Roles = "2FA")]
//        [HttpPost("twofactorauthentication")]
//        public async Task<IActionResult> LoginWithTwoFactorAuthentication(
//            [FromBody] TwoFactorAuthenticationDto twoFactorLoginCredentials)
//        {
//            string token = twoFactorLoginCredentials.Token;
//            _user = await _userManager.FindByEmailAsync(Claims(token)
//                .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
//                ?.Value);

//            ApplicationUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
//            if (_user == null)
//            {
//                return BadRequest(new AuthResponseDto { ErrorMessage = "Invalid Request. User not found", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//            }
//            if (!_user.TwoFactorEnabled)
//            {
//                return BadRequest(new AuthResponseDto { ErrorMessage = "Two Factor Authentication not enabled", AuthSuccessful = false, TwoFactorVerificationEnabled = false, Provider = null, RefreshToken = null, Token = null });
//            }

//            string authenticatorCode = twoFactorLoginCredentials.TwoFactorCode.Replace(" ", string.Empty)
//                .Replace("-", string.Empty);

//            //SignInResult result = authenticatorCode.Length == 6
//            //    ? await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, twoFactorLoginCredentials.RememberMe, false)
//            //    : await _signInManager.TwoFactorRecoveryCodeSignInAsync(authenticatorCode); //Backup Code Login

//            bool result = await _userManager.VerifyTwoFactorTokenAsync(_user, new IdentityOptions().Tokens.AuthenticatorTokenProvider, authenticatorCode);

//            if (!result)
//            {
//                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid authenticator code.", AuthSuccessful = false, TwoFactorVerificationEnabled = true, Provider = "App", RefreshToken = null, Token = null });
//            }

//            token = await _jwtHandler.GenerateAccessToken(_user);

//            _user.AccessToken = token;
//            _user.RefreshToken = _jwtHandler.GenerateRefreshToken();
//            _user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddMinutes(int.MaxValue);
//            _user.SecurityStamp = Guid.NewGuid().ToString();

//            _ = await _userManager.ResetAccessFailedCountAsync(_user);
//            _ = await _userManager.UpdateAsync(_user);

//            return Ok(new AuthResponseDto
//            { AuthSuccessful = true, Token = token, RefreshToken = _user.RefreshToken, TwoFactorVerificationEnabled = true, Provider = "App", ErrorMessage = null });
//        }

//        [Authorize(Roles = "2FA, Administrator, Viewer")]
//        [HttpGet("twofactorauthenticationsetup")]
//        public async Task<IActionResult> SetupTwoFactorAuthentication([FromQuery] string token)

//        {
//            _user = await _userManager.FindByEmailAsync(Claims(token)
//                .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
//                ?.Value);

//            if (_user == null)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }

//            if (_user.TwoFactorEnabled)
//            {
//                return BadRequest(new { ErrorMessage = "Two-Factor-Authentication already enabled" });
//            }

//            List<UserLoginInfo> loginList = (List<UserLoginInfo>)await _userManager.GetLoginsAsync(_user);
//            if (loginList.Count > 0)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }

//            string unformattedKey = await _userManager.GetAuthenticatorKeyAsync(_user);
//            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(_user, 6);

//            if (string.IsNullOrEmpty(unformattedKey))
//            {
//                _ = await _userManager.ResetAuthenticatorKeyAsync(_user);
//                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(_user);
//            }

//            TwoFactorAuhenticationSetupDto model = new()
//            {
//                SharedKey = FormatKey(unformattedKey),
//                AuthenticatorUri = GenerateQrCodeUri(_user.Email, unformattedKey),
//                RecoveryCodes = recoveryCodes.ToArray()
//            };

//            return Ok(model);
//        }

//        [Authorize(Roles = "2FA, Administrator, Viewer")]
//        [HttpPost("validatetwofactorauthentication")]
//        public async Task<IActionResult> ValidateTwoFactorAuthentication(
//            [FromBody] TwoFactorAuhenticationValidateDto twoFactorCredentials)
//        {
//            string token = twoFactorCredentials.Token;
//            _user = await _userManager.FindByEmailAsync(Claims(token)
//                .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
//                ?.Value);

//            if (_user == null)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }

//            if (_user.TwoFactorEnabled)
//            {
//                return BadRequest(new { ErrorMessage = "Two-Factor-Authentication already enabled" });
//            }

//            string verificationCode = twoFactorCredentials.AuthenticatorCode.Replace(" ", string.Empty)
//                .Replace("-", string.Empty);

//            bool twoFactorCodeValid = await _userManager.VerifyTwoFactorTokenAsync(
//                _user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

//            if (!twoFactorCodeValid)
//            {
//                return Unauthorized(new { ErrorMessage = "Verification code is invalid" });
//            }

//            _ = await _userManager.SetTwoFactorEnabledAsync(_user, true);

//            token = await _jwtHandler.GenerateAccessToken(_user);

//            _user.AccessToken = token;
//            _user.RefreshToken = _jwtHandler.GenerateRefreshToken();
//            _user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddMinutes(int.MaxValue);
//            _user.SecurityStamp = Guid.NewGuid().ToString();

//            _ = await _userManager.ResetAccessFailedCountAsync(_user);
//            _ = await _userManager.UpdateAsync(_user);

//            return Ok(new AuthResponseDto
//            {
//                AuthSuccessful = true,
//                TwoFactorVerificationEnabled = true,
//                Provider = "App",
//                Token = token,
//                RefreshToken = _user.RefreshToken
//            });
//        }

//        [Authorize(Roles = "Administrator, Viewer")]
//        [HttpPost("resettwofactorauthentication")]
//        public async Task<IActionResult> ResetTwoFactorAuthentication([FromBody] TokenDto tokenModel)
//        {
//            string token = tokenModel.Token;
//            _user = await _userManager.FindByEmailAsync(Claims(token)
//                .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
//                ?.Value);

//            if (_user == null || _user.AccessToken != token)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }

//            if (!_user.TwoFactorEnabled)
//            {
//                return BadRequest(new { ErrorMessage = "Two-Factor-Authentication already disabled" });
//            }

//            _ = await _userManager.SetTwoFactorEnabledAsync(_user, false);
//            _ = await _userManager.ResetAuthenticatorKeyAsync(_user);

//            return Ok();
//        }

//        [Authorize(Roles = "Administrator, Viewer")]
//        [HttpGet("userismicrosoftaccount")]
//        public async Task<ActionResult<Boolean>> UserIsMicrosoftAccount([FromQuery] string token)
//        {
//            _user = await _userManager.FindByEmailAsync(Claims(token)
//                .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
//                ?.Value);

//            if (_user == null || _user.AccessToken != token)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }

//            List<UserLoginInfo> userLogins = (List<UserLoginInfo>)await _userManager.GetLoginsAsync(_user);

//            bool isMicrosoftAccount = userLogins.First(e => e.LoginProvider == "Microsoft").LoginProvider == "Microsoft";

//            return Ok(isMicrosoftAccount);
//        }

//        [Authorize(Roles = "Administrator, Viewer")]
//        [HttpGet("userhastwofactorauthentication")]
//        public async Task<ActionResult<Boolean>> UserHasTwoFactorAuthentication([FromQuery] string token)
//        {
//            _user = await _userManager.FindByEmailAsync(Claims(token)
//                .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
//                ?.Value);

//            if (_user == null || _user.AccessToken != token)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }

//            bool hasTwoFactorAuthentication = _user.TwoFactorEnabled;

//            return Ok(hasTwoFactorAuthentication);
//        }

//        [Authorize(Roles = "Administrator")]
//        [HttpPost("deleteuser")]
//        public async Task<IActionResult> DeleteUser([FromBody] TokenDto tokenModel)
//        {
//            string token = tokenModel.Token;
//            _user = await _userManager.FindByEmailAsync(Claims(token)
//                .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
//                ?.Value);

//            if (_user == null || _user.AccessToken != tokenModel.Token)
//            {
//                return BadRequest(new { ErrorMessage = "Invalid Request" });
//            }

//            _ = await _userManager.DeleteAsync(_user);

//            return Ok();
//        }

//        private IEnumerable<Claim> Claims(string token) => _jwtHandler.GetPrincipalFromToken(token).Claims;

//        public async Task<bool> IsAccessTokenValid(string token)
//        {
//            _user = await _userManager.FindByEmailAsync(Claims(token)
//                .FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
//                ?.Value);

//            return _user.AccessToken == token;
//        }

//        private static string FormatKey(string unformattedKey)
//        {
//            StringBuilder result = new();
//            int currentPosition = 0;
//            while (currentPosition + 4 < unformattedKey.Length)
//            {
//                _ = result.Append(unformattedKey.Substring(currentPosition, 4)).Append(' ');
//                currentPosition += 4;
//            }

//            if (currentPosition < unformattedKey.Length)
//            {
//                _ = result.Append(unformattedKey[currentPosition..]);
//            }

//            return result.ToString().ToLowerInvariant();
//        }

//        private string GenerateQrCodeUri(string email, string unformattedKey) => string.Format(
//            AuthenicatorUriFormat,
//            _urlEncoder.Encode("WebApplication"),
//            _urlEncoder.Encode(email),
//            unformattedKey);

//        private async Task SendEmailConformation(string clientUrl)
//        {
//            string token = await _userManager.GenerateEmailConfirmationTokenAsync(_user);

//            Dictionary<string, string> param = new()
//            {
//                { "token", token },
//                { "email", _user.Email }
//            };

//            string callback = QueryHelpers.AddQueryString(clientUrl, param);

//            MailMessage message = new()
//            {
//                Subject = "Email Confirmation Budget-Plan",
//                Body = callback,
//                IsBodyHtml = true
//            };
//            message.To.Add(_user.Email);
//            await _emailSender.SendEmailAsync(message);
//        }

//        private async Task SendPasswordReset(ForgotPasswordDto fortgotPasswordCredentials)
//        {
//            string token = await _userManager.GeneratePasswordResetTokenAsync(_user);

//            Dictionary<string, string> param = new()
//            {
//                { "token", token },
//                { "email", fortgotPasswordCredentials.Email }
//            };


//            string clientUri = fortgotPasswordCredentials.ClientUrl;

//            string callback = QueryHelpers.AddQueryString(clientUri, param);

//            MailMessage message = new()
//            {
//                Subject = "Reset password Budget-Plan",
//                Body = callback,
//                IsBodyHtml = true
//            };
//            message.To.Add(_user.Email);

//            await _emailSender.SendEmailAsync(message);
//        }
//    }
//}