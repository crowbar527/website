using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.DynamoDBv2.DataModel;
using AspNetCore.Identity.DynamoDB;
using CrowbarWebsite.Helpers;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

namespace CrowbarWebsite
{
    public class DynamoDbSettings
    {
        public string ServiceUrl { get; set; }
        public string UsersTableName { get; set; }
        public string RolesTableName { get; set; }
        public string RoleUsersTableName { get; set; }
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            var options = app.ApplicationServices.GetService<IOptions<DynamoDbSettings>>();

            var credentials = AWSHelpers.getCreds().Result;
            var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.APSoutheast2);

            var context = new DynamoDBContext(client);

            var userStore = app.ApplicationServices
                    .GetService<IUserStore<DynamoIdentityUser>>()
                as DynamoUserStore<DynamoIdentityUser, DynamoIdentityRole>;
            var roleStore = app.ApplicationServices
                    .GetService<IRoleStore<DynamoIdentityRole>>()
                as DynamoRoleStore<DynamoIdentityRole>;
            var roleUsersStore = app.ApplicationServices
                .GetService<DynamoRoleUsersStore<DynamoIdentityRole, DynamoIdentityUser>>();

            userStore.EnsureInitializedAsync(client, context, "CrowbarUsers").Wait();
            roleStore.EnsureInitializedAsync(client, context, "CrowbarRoles").Wait();
            roleUsersStore.EnsureInitializedAsync(client, context, "CrowbarRoleUser").Wait();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
