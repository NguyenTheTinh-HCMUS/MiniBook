using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniBook.Identity
{
    public static class AuthConstants
    {
        public static class PhoneNumberTokenSms
        {
            public const string PhoneNumber = "phone_number";
            public const string Token = "verification_token";
            public const string GrantType = "phone_number_token_sms";
        }
        public static class PhoneNumberPassword
        {
            public const string PhoneNumber = "phone_number";
            public const string Password = "password";
            public const string GrantType = "phone_number_password";

        }


    }
}
