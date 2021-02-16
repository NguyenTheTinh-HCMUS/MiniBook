using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniBook.Identity.Validations
{
    public class PhoneNumberPasswordGrantValidator : IExtensionGrantValidator
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEventService _events;
        private readonly ILogger<PhoneNumberTokenSmsGrantValidator> _logger;

        public PhoneNumberPasswordGrantValidator(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEventService events,
            ILogger<PhoneNumberTokenSmsGrantValidator> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _events = events;
            _logger = logger;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var raw = context.Request.Raw;
            var credential = raw.Get(OidcConstants.TokenRequest.GrantType);
            if (credential == null || credential != AuthConstants.PhoneNumberPassword.GrantType)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant,
                    "invalid verify_phone_number_password credential");
                return;
            }

            var phoneNumber = raw.Get(AuthConstants.PhoneNumberPassword.PhoneNumber);
            var password = raw.Get(AuthConstants.PhoneNumberPassword.Password);

            var user = await _userManager.Users.SingleOrDefaultAsync(x =>
                x.PhoneNumber == _userManager.NormalizeName(phoneNumber) && !string.IsNullOrEmpty(phoneNumber));
            if (user == null)
            {
                _logger.LogInformation("Authentication failed for PhoneNumber: {phoneNumber}, reason: not exists phonenumber",
                  phoneNumber);
                await _events.RaiseAsync(new UserLoginFailureEvent(phoneNumber,
                    "invalid PhoneNumber", false));
                return;
            }
            if (!user.PhoneNumberConfirmed)
            {
                _logger.LogInformation("Authentication failed for PhoneNumber: {phoneNumber}, reason: not confirm",
                  phoneNumber);
                await _events.RaiseAsync(new UserLoginFailureEvent(phoneNumber,
                    "invalid PhoneNumber", false));
                return;

            }
            if (! await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogInformation("Authentication failed for Password: {password}, reason: invalid",
                 password);
                await _events.RaiseAsync(new UserLoginFailureEvent(password,
                    "invalid password", false));
                return;
            }
            _logger.LogInformation("Credentials validated for username: {phoneNumber}", phoneNumber);
            await _events.RaiseAsync(new UserLoginSuccessEvent(phoneNumber, user.Id, phoneNumber, false));
            await _signInManager.SignInAsync(user, true);
            context.Result = new GrantValidationResult(user.Id, OidcConstants.AuthenticationMethods.Password);
        }
        public string GrantType => AuthConstants.PhoneNumberPassword.GrantType;


      
    }
}
