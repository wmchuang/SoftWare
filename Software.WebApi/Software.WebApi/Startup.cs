using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.RegisterServices;
using Senparc.Weixin.TenPay;
using Senparc.Weixin.WxOpen;
using Software.WebApi.AOP;
using Software.WebApi.AuthHelper;
using Software.WebApi.Filter;
using Swashbuckle.AspNetCore.Swagger;

namespace Software.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private const string ApiName = "Software";

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICaching, MemoryCaching>();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region 小程序
            /*
             * CO2NET 是从 Senparc.Weixin 分离的底层公共基础模块，经过了长达 6 年的迭代优化，稳定可靠。
             * 关于 CO2NET 在所有项目中的通用设置可参考 CO2NET 的 Sample：
             * https://github.com/Senparc/Senparc.CO2NET/blob/master/Sample/Senparc.CO2NET.Sample.netcore/Startup.cs
             */
            services.AddSenparcGlobalServices(Configuration)//Senparc.CO2NET 全局注册
                    .AddSenparcWeixinServices(Configuration);//Senparc.Weixin 注册
            #endregion

            #region Automapper
            services.AddAutoMapper(typeof(Startup));
            #endregion

            #region Swagger
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v0.1.0",
                    Title = $"{ApiName} API",
                    Description = "框架说明文档",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact { Name = ApiName, Email = "xxx@xxx.com", Url = "" }
                });


                var xmlPath = Path.Combine(basePath, "Software.WebApi.xml");//这个就是刚刚配置的xml文件名
                c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改

                //var xmlModelPath = Path.Combine(basePath, "Blog.Core.Model.xml");//这个就是Model层的xml文件名
                //c.IncludeXmlComments(xmlModelPath);

                #region Token绑定到ConfigureServices
                // 发行人
                var IssuerName = (Configuration.GetSection("Audience"))["Issuer"];
                var security = new Dictionary<string, IEnumerable<string>> { { IssuerName, new string[] { } }, };
                c.AddSecurityRequirement(security);

                c.AddSecurityDefinition(IssuerName, new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
                #endregion
            });
            #endregion

            #region JWT Token Service
            //读取配置文件
            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            // 令牌验证参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],//发行人
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,

            };

            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var permission = new List<Permission>();
            // 角色与接口的权限要求参数
            var permissionRequirement = new PermissionRequirement(
                "/api/denied",// 拒绝授权的跳转地址（目前无用）
                permission,
                ClaimTypes.Role,//基于角色的授权
                audienceConfig["Issuer"],//发行人
                audienceConfig["Audience"],//听众
                signingCredentials,//签名凭据
                expiration: TimeSpan.FromDays(7)//接口的过期时间
                );

            //注入参数，供控制器调用
            /*
            Singleton
            单例模式，服务在第一次请求时被创建，其后的每次请求都沿用这个已创建的服务。我们不用再自己写单例了。
            Scoped
               作用域模式，服务在每次请求时被创建，整个请求过程中都贯穿使用这个创建的服务。比如Web页面的一次请求。
            Transient
            瞬态模式，服务在每次请求时被创建，它最好被用于轻量级无状态服务。
            */
            services.AddSingleton(permissionRequirement);

            services.AddAuthentication(x =>
               {
                   x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               }).AddJwtBearer(o =>
               {
                   o.TokenValidationParameters = tokenValidationParameters;
               });
            #endregion

            #region AutoFac
            //实例化 AutoFac  容器   
            var builder = new ContainerBuilder();
            //注册要通过反射创建的组件
             builder.RegisterType<CacheAOP>();//可以直接替换其他拦截器
         
            //获取项目绝对路径，请注意，这个是实现类的dll文件，不是接口 IService.dll ，注入容器当然是Activatore
            var servicesDllFile = Path.Combine(basePath, "Software.Services.dll");
            var assemblysServices = Assembly.LoadFile(servicesDllFile);//直接采用加载文件的方法
            builder.RegisterAssemblyTypes(assemblysServices).AsImplementedInterfaces();//指定已扫描程序集中的类型注册为提供所有其实现的接口。

            builder.RegisterAssemblyTypes(assemblysServices)
                      .AsImplementedInterfaces()
                      .InstancePerLifetimeScope()
                      .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                                                    // 如果你想注入两个，就这么写  InterceptedBy(typeof(BlogCacheAOP), typeof(BlogLogAOP));
                      .InterceptedBy(typeof(CacheAOP));//允许将拦截器服务的列表分配给注册。

            var repositoryDllFile = Path.Combine(basePath, "Software.Repository.dll");
            var assemblysRepository = Assembly.LoadFile(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces();

            //将services填充到Autofac容器生成器中
            builder.Populate(services);

            //使用已进行的组件登记创建新容器
            var ApplicationContainer = builder.Build();

            #endregion

            return new AutofacServiceProvider(ApplicationContainer);//第三方IOC接管 core内置DI容器
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // 使用静态文件
            app.UseStaticFiles();

            NLog.LogManager.LoadConfiguration("nlog.config");
            loggerFactory.AddNLog();//添加NLog

            #region 全局错误拦截器配置
            app.UseErrorHandling();
            #endregion

            #region 微信
            // 启动 CO2NET 全局注册，必须！
            IRegisterService register = RegisterService.Start(env, senparcSetting.Value)
                                                        //关于 UseSenparcGlobal() 的更多用法见 CO2NET Demo：https://github.com/Senparc/Senparc.CO2NET/blob/master/Sample/Senparc.CO2NET.Sample.netcore/Startup.cs
                                                        .UseSenparcGlobal();
            register.UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value)
                  .RegisterWxOpenAccount(senparcWeixinSetting.Value, "小程序")// DPBMARK_END
                                                                           //注册最新微信支付版本（V3）（可注册多个）
                  .RegisterTenpayV3(senparcWeixinSetting.Value, "小程序支付");
            #endregion

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
                c.RoutePrefix = "";//路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉
                //js注入
                c.InjectJavascript("/jquery/jquery.js");//jquery 插件
                c.InjectJavascript("/swagger-lang.js");//我们自定义的js
            });
            #endregion

            #region Authen
            // 如果你想使用官方认证，必须在上边ConfigureService 中，配置JWT的认证服务
            // .AddAuthentication 和 .AddJwtBearer 二者缺一不可
            app.UseAuthentication();
            #endregion

            app.UseHttpsRedirection();
            // 使用cookie
            app.UseCookiePolicy();
            app.UseMvc();
        }
    }
}
