using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace miauthcore.Model
{
    public sealed class MicrosoftAuthenticationConfig
    {

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Guid? TenantId { get; set; }

    }
}
