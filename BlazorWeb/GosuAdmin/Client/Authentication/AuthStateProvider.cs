﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Gosu.GrpcClient.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationState anonymous;

        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
            this.anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(GetClaimsPrincipal()));
        }

        private ClaimsPrincipal GetClaimsPrincipal() {
            //Anonymuos
            if (String.IsNullOrWhiteSpace(WebUserCredential.Username))
            {
                WebUserCredential.IsAuthenticated = false;
                return new ClaimsPrincipal(new ClaimsIdentity());
            }

            //Authenticated user
            WebUserCredential.IsAuthenticated = true;
            //Claim
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, WebUserCredential.Username));
            claims.Add(new Claim(ClaimTypes.Role, WebUserCredential.RoleID.ToString()));

            claims.Add(new Claim("Fullname", WebUserCredential.Fullname));

            //ClaimsPrincipal
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwtAuthType"));
        }

        public void NotifyUserAuthentication()
        {
            //ClaimsPrincipal
            var authenticationUser = GetClaimsPrincipal();

            //AuthenticationState
            var authState = Task.FromResult(new AuthenticationState(authenticationUser));

            //Notify
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            //Clear WebUserCredential
            WebUserCredential.Username = "";
            WebUserCredential.Fullname = "";
            WebUserCredential.RoleID = "";
            WebUserCredential.ApproveLevel = 0;
            WebUserCredential.DocumentLevel = 0;
            WebUserCredential.IsAuthenticated = false;

            //Notify
            var authState = Task.FromResult(anonymous);
            NotifyAuthenticationStateChanged(authState);
        }

    }
}
