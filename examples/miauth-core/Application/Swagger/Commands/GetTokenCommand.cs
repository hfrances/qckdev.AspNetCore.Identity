using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace miauthcore.Application.Swagger.Commands
{
    public class GetTokenCommand : IRequest<Unit> 
    {

        public string Provider { get; set; }
        public string Code { get; set;  }
        public string Redirect_uri { get; set; }
        public string State { get; set; }

    }
}
