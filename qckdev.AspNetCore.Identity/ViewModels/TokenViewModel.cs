using System;

namespace qckdev.AspNetCore.Identity.ViewModels
{
    public sealed class TokenViewModel : ITokenViewModel
    {

        public string AccessToken { get; set; }
        public DateTime Expired { get; set; }
        public string RefreshToken { get; set; }

    }
}
