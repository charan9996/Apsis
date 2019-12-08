using Apsis.Abstractions;
using Apsis.Abstractions.Repository;
using Apsis.Models.Attributes;
using Apsis.Models.Authorization;
using Apsis.Models.Constants;
using Apsis.Models.Entities;
using Apsis.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using SimpleInjector;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Apsis.API
{
    public class Startup
    {
        readonly Container container = new Container();
        public IConfiguration _configuration;
        private readonly string _adClientId;
        private readonly string _adAuthority;
        private IRepository _baseRepository;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _adClientId = configuration["AzureActiveDirectorySettings:ClientId"];
            _adAuthority = configuration["AzureActiveDirectorySettings:Authority"];
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(options =>
            {
                options.Filters.Add(new ExceptionAttribute());
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Learner", policy =>
                                  policy.RequireClaim("RoleId", Constants.LearnerRoleId.ToString(), Constants.EvaluatorRoleId.ToString(), Constants.LearningOPMRoleId.ToString()));
                options.AddPolicy("EvaluatorsAndLearningOPM", policy =>
                                  policy.RequireClaim("RoleId", Constants.EvaluatorRoleId.ToString(), Constants.LearningOPMRoleId.ToString()));
                options.AddPolicy("LearningOPMOnly", policy =>
                                  policy.RequireClaim("RoleId", Constants.LearningOPMRoleId.ToString()));
            });

            // Configure Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Audience = _adClientId;
                options.Authority = _adAuthority;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudiences = new string[] { _adClientId },
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = tokenValidatedcontext =>
                    {
                        var applicationContext = BuildApplicationContext(tokenValidatedcontext);
                        tokenValidatedcontext.HttpContext.Items["ApplicationContext"] = applicationContext;

                        var claim = new Claim("RoleId", applicationContext.CurrentUser.RoleId.ToString());
                        tokenValidatedcontext.Principal.Identities.First().AddClaim(claim);

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = authFailedcontext =>
                    {
                        Logging.Logger.LogError("Unauthorized : Invalid token");
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddTransient<IRepository, SqlBaseRepository>();

            // CORS policy setup
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddLogging();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Apsis API", Version = "v1" });
                    // Include comments from xml documentation
                    var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                foreach (var name in Directory.GetFiles(basePath, "*.XML", SearchOption.AllDirectories))
                {
                    c.IncludeXmlComments(name);
                }
            });

            NLog.Web.NLogBuilder.ConfigureNLog(Logging.Configuration.Initialize());

            DependencyResolver.Configuration.IntegrateSimpleInjector(services, container);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IRepository baseRepository)
        {
            _baseRepository = baseRepository;
            app.UseAuthentication();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Apsis API V1");
                // To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string:
                c.RoutePrefix = string.Empty;
            });

            app.UseCors("CorsPolicy");

            app.UseStatusCodePages("text/plain", "An error has occured, status code: {0}");

            app.UseMvc();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            container.Register<IContextProvider, ContextProvider>();
            DependencyResolver.Configuration.InitilizeContainer(app, container);
        }

        /// <summary>
        /// Method to get the User Context
        /// </summary>
        /// <param name="tokenValidatedcontext"></param>
        /// <returns></returns>
        private ApplicationContext BuildApplicationContext(TokenValidatedContext tokenValidatedcontext)
        {
            var requestUrl = tokenValidatedcontext.Request.Path.ToString();
            // Avoid building context for Swagger urls
            if (!IsSwaggerUrl(requestUrl))
            {
                var context = new ApplicationContext
                {
                    CurrentUser = GetCurrentUserDetails(tokenValidatedcontext.Principal)
                };
                return context;
            }
            return null;
        }

        /// <summary>
        /// Get current user details from token
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        private User GetCurrentUserDetails(ClaimsPrincipal claimsPrincipal)
        {
            string email = claimsPrincipal.FindFirst("preferred_username")?.Value;
            if (string.IsNullOrEmpty(email)) return null;
            string mid = email.Split('@')[0];
            var userRepository = new UserRepository(_baseRepository);
            var user = userRepository.GetUser(mid).Result;
            if (user == null)
                user = AddNewUser(claimsPrincipal);
            return user;
        }

        private User AddNewUser(ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = claimsPrincipal.FindFirst("preferred_username")?.Value,
                    Mid = claimsPrincipal.FindFirst("preferred_username")?.Value.Split('@')[0],
                    Name = claimsPrincipal.FindFirst("name")?.Value,
                    RoleId = Constants.LearnerRoleId,
                    IsExternal = false
                };
                var userRepository = new UserRepository(_baseRepository);
                userRepository.AddUsers(new List<User> { newUser });
                return newUser;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Method to check whether the url is swagger url
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        private bool IsSwaggerUrl(string requestUrl)
        {
            return !requestUrl.StartsWith("/api");
        }
    }
}
