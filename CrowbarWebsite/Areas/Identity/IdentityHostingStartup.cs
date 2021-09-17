using System;
using CrowbarWebsite.Areas.Identity.Data;
using CrowbarWebsite.Data;
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
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<CrowbarWebsiteContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("CrowbarWebsiteContextConnection")));

                services.AddDefaultIdentity<CrowbarWebsiteUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<CrowbarWebsiteContext>();
            });
        }
    }
}