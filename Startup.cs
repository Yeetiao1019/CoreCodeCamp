using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoreCodeCamp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CampContext>();
            services.AddScoped<ICampRepository, CampRepository>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true;     // Request 不包含 API 版本號的話，系統自動使用預設版本
                opt.DefaultApiVersion = new ApiVersion(1, 0);       // 系統預設 API 版本
                opt.ReportApiVersions = true;       // 在 Response 的 Header 中加入系統支援的 API 版本
                //opt.ApiVersionReader = new QueryStringApiVersionReader("ver");      // 讀取 URI 的 api-version，並自訂參數名稱為 ver
                //opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");     // 讀取 Header 的 api-version，並自訂 Key 名稱為 X-Version
                opt.ApiVersionReader = ApiVersionReader.Combine(        // 使用多種方式來接收版本資訊
                    new QueryStringApiVersionReader("ver" , "version"),
                    new HeaderApiVersionReader("X-Version")
                    );
            });

            services.AddControllers(opt => opt.EnableEndpointRouting = false);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(cfg =>
            {
                cfg.MapControllers();
            });
        }
    }
}
