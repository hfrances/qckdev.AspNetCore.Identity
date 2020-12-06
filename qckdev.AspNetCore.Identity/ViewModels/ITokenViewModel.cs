using System;

namespace qckdev.AspNetCore.Identity.ViewModels
{
    public interface ITokenViewModel
    {

        string AccessToken { get; set; }
        DateTime Expired { get; set; }

    }
}
