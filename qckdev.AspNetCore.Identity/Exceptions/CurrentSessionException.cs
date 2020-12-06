using System;

namespace qckdev.AspNetCore.Identity.Exceptions
{

    [Serializable]
    public sealed class CurrentSessionException : Exception
    {

        public CurrentSessionException(string message) : base(message)
        {
        }

    }
}
