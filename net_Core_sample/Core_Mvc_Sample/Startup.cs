using Mail.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Core_Mvc_Sample
{
    public class Startup
    {
        /*
       * IIS ���ý� https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-5.0.5-windows-hosting-bundle-installer ��ġ ���� 
       * https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial/install
       * IIS �������α׷� Ǯ���� .NET CLR ����  �����ڵ� ���� üũ
       */

        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
            #if DEBUG //����� ���
                            .AddJsonFile("appsettings.Development.json", optional: true)
            #else
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            #endif
          .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            #region ������ ���̽� ���� ����
            string Mail_ConnString = Configuration["ConnectiongStrings:Mail_Db"];

            //EF Core  DB ���� ��� ó��
            services.AddDbContext<MailContext>((options) =>
            {
                options.UseSqlServer(Mail_ConnString);
            });

            #endregion

            #region ���� ���� ����
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".Session";
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            #endregion

            #region ��Ű ���� ����
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
            });

            ///��� ���� httpcontext ó�� ���� �ϱ�.. 
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();

            #endregion

            #region �ѱ۱��� ���� ó��
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All); // �ѱ��� ���ڵ��Ǵ� ���� �ذ�
            });
            #endregion

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

            app.UseAuthorization();

            #region ����
            app.UseSession();
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
