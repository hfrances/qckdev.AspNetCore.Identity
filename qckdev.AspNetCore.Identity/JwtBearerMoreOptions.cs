using System;

namespace qckdev.AspNetCore.Identity
{
    public class JwtBearerMoreOptions
    {

        /// <summary>
        /// Gets or sets the amount of time a generated token remains valid. Defaults to 1 day.
        /// </summary>
        /// <value>
        /// The amount of time a generated token remains valid.
        /// </value>
        public TimeSpan TokenLifeTimespan { get; set; } = TimeSpan.FromDays(1);

        /// <summary>
        /// Gets or sets the amount of time a generated token remains valid. Defaults to 10 minutes.
        /// </summary>
        /// <value>
        /// The amount of time a generated token remains valid.
        /// </value>
        public TimeSpan ExternalConfirmationTokenLifeTimespan { get; set; } = TimeSpan.FromSeconds(600);

        public string ClientId { get; set; }

    }
}