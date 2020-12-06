using System;

namespace qckdev.AspNetCore.Identity.ViewModels
{
    public sealed class TokenToConfirmExternalUserViewModel : ITokenViewModel
    {
        public string AccessToken { get; set; }
        public DateTime Expired { get; set; }
        public dynamic NewUserData { get; set; }

    }
}
