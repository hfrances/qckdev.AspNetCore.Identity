﻿using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Exceptions;
using qckdev.AspNetCore.Identity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using qckdev.Linq;
using qckdev.AspNetCore.Identity.Policies;

namespace qckdev.AspNetCore.Identity.Helpers
{
    static class CreateUserHelper
    {

        public static async Task<IdentityUser> CreateUser<TCreateUserCommand>(IServiceProvider services, TCreateUserCommand request)
            where TCreateUserCommand : CreateUserCommand
        {
            var currentSessionService = services.GetService<ICurrentSessionService>();
            var identityManager = services.GetService<IIdentityManager>();

            if (await identityManager.FindByNameAsync(request.UserName) != null)
            {
                throw new IdentityException("Error creating account: UserName already exists."); // TODO: Traducir.
            }
            else if (await identityManager.FindByEmailAsync(request.Email) != null)
            {
                throw new IdentityException("Error creating account: Email already exists."); // TODO: Traducir.
            }
            else
            {
                var userNameLogged = currentSessionService.GetUserNameIdentifier();
                IdentityUser userLogged, user;
                IdentityResult result;

                if (!string.IsNullOrEmpty(userNameLogged))
                {
                    userLogged = await identityManager.FindByNameAsync(userNameLogged);
                }

                user = identityManager.CreateInstanceUser();
                user.Email = request.Email;
                user.UserName = request.UserName;

                var userArgsType = typeof(CreateUserArgs<>).MakeGenericType(user.GetType());
                IEnumerable<string> roles = new string[] { };
                CustomizableActionHelper.GetCustomizers
                        <TCreateUserCommand, CreateUserCommand>(services, userArgsType)
                    .ForEach(x =>
                    {
                        var args = (ICreateUserArgs)qckdev.Reflection.ReflectionHelper.CreateInstance(userArgsType, user, roles);

                        x.Customize(request, args);
                        roles = args.Roles; // Keep for next iteration.
                    });

                result = await identityManager.CreateUserAsync(user, request.Password);
                if (result.Succeeded)
                {
                    result = await identityManager.AddToRolesAsync(user, roles);

                    if (result.Succeeded)
                    {
                        if (!user.EmailConfirmed)
                        {
                            await identityManager.AddToRoleAsync(user, GuestAuthorizationHandler.POLICY_NAME);
                            await GenerateAndSendEmailConfirmationAsync(services, user);
                        }
                        return user;
                    }
                    else
                    {
                        throw new IdentityException("Error creating account.", result.Errors); // TODO: Traducir.
                    }
                }
                else
                {
                    throw new IdentityException("Error creating account.", result.Errors); // TODO: Traducir.
                }
            }
        }

        public static async Task<string> GenerateAndSendEmailConfirmationAsync(IServiceProvider services, IdentityUser user)
        {
            var identityManager = services.GetService<IIdentityManager>();
            var currentSessionService = services.GetService<ICurrentSessionService>();
            var httpContextAccesor = services.GetService<IHttpContextAccessor>();


            string token;
            string confirmationLink;

            // https://code-maze.com/email-confirmation-aspnet-core-identity/
            token = await identityManager.GenerateEmailConfirmationTokenAsync(user);
            confirmationLink = currentSessionService.GetUriByAction(
                action: "confirmemail",
                controller: null,
                values: new { token, email = user.Email }
            );
            httpContextAccesor.HttpContext.Response.Redirect(confirmationLink); // TODO: Send mail.

            return token;
        }


        public static async Task<IdentityUser> CreateExternalUser(IServiceProvider services, IdentityUser user, IEnumerable<string> roles, UserLoginInfo userLoginInfo)
        {
            var identityManager = services.GetService<IIdentityManager>();
            IdentityResult result;

            user.EmailConfirmed = true;
            result = await identityManager.CreateUserAsync(user);
            if (result.Succeeded)
            {
                await identityManager.AddToRolesAsync(user, roles);
                result = await identityManager.AddLoginAsync(user, userLoginInfo);
                if (result.Succeeded)
                {
                    return user;
                }
                else
                {
                    throw new AggregateException(
                        $"Error adding {userLoginInfo.LoginProvider} login for {user.Email}. See inner exceptions.",
                        result.Errors.Select(err => new Exception(err.Description)));
                }
            }
            else
            {
                throw new AggregateException(
                    $"Error creating user for '{user.Email}'. See inner exceptions.",
                    result.Errors.Select(err => new Exception(err.Description)));
            }
        }
    }

}