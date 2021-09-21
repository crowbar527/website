using System;
using System.Collections.Generic;
using AspNetCore.Identity.DynamoDB;
using CrowbarWebsite.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(CrowbarWebsite.Areas.Identity.IdentityHostingStartup))]
namespace CrowbarWebsite.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDefaultIdentity<DynamoIdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<DynamoIdentityRole>()
                    .AddDefaultTokenProviders();

                services
                    .AddSingleton<DynamoRoleUsersStore<DynamoIdentityRole, DynamoIdentityUser>,
                        DynamoRoleUsersStore<DynamoIdentityRole, DynamoIdentityUser>>();
                services
                    .AddSingleton<IUserStore<DynamoIdentityUser>,
                        DynamoUserStore<DynamoIdentityUser, DynamoIdentityRole>>();
                services.AddSingleton<IRoleStore<DynamoIdentityRole>, DynamoRoleStore<DynamoIdentityRole>>();

                Dictionary<string, string> goog = AWSHelpers.GetSecret().Result;
                services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        options.ClientId = goog["google-clientid"];
                        options.ClientSecret = goog["google-secret"];
                    });
            });
        }
    }
}