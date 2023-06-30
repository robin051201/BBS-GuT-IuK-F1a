namespace PizzaWelt.Extension
{
    public static class ServiceExtensions
    {
        public static void ConfigureDbContext(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<PizzaWeltDbContext>(options =>
                options.UseMySQL(connectionString,
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        }

        public static void ConfigureControllers(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.ModelMetadataDetailsProviders.Add(new NewtonsoftJsonValidationMetadataProvider());
            }).AddNewtonsoftJson();
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:4200", "https://localhost:44305", "http://localhost:5000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
                        .SetIsOriginAllowedToAllowWildcardSubdomains();
                });
            });
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            //services.AddScoped<ServiceHelper>();
            //services.AddScoped<AsyncActionFilter>();
            services.AddApplicationServiceTransients();
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void ConfigureHttpsRedirection(this IServiceCollection services)
        {
            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 44305;
            });
        }

        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.RequireUniqueEmail = true;

                // SignIn settings.
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;

                //options.Tokens.EmailConfirmationTokenProvider = "";
            })
                .AddEntityFrameworkStores<PizzaWeltDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var builder = services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection("JWTSettings").GetSection("validIssuer").Value,
                    ValidAudience = configuration.GetSection("JWTSettings").GetSection("validAudience").Value,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JWTSettings")
                            .GetSection("securityKey").Value)),
                    ClockSkew = TimeSpan.Zero
                };
            }).AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"];
                microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];

                microsoftOptions.SaveTokens = true;
                microsoftOptions.Events.OnTicketReceived = context =>
                {
                    Console.WriteLine(context.HttpContext.User);
                    return Task.CompletedTask;
                };
                microsoftOptions.Events.OnCreatingTicket = context =>
                {
                    Console.WriteLine(context.Identity);
                    return Task.CompletedTask;
                };
            })
            .AddIdentityServerJwt()
            .AddCookie(o =>
            {
                o.Cookie.IsEssential = false;
            });

            services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromMinutes(20.0));

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero; // enables immediate logout, after updating the user's stat.
            });
        }

        public static void ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            byte[] key = Encoding.UTF8.GetBytes(configuration.GetSection("JWTSettings:securityKey").Value);
            SymmetricSecurityKey secret = new(key);
            SigningCredentials sc = new(secret, SecurityAlgorithms.HmacSha256);

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, PizzaWeltDbContext>(options =>
                {
                    options.SigningCredential = sc;
                });
        }

        private static void AddApplicationServiceTransients(this IServiceCollection services)
        {
            var assembly = typeof(IApplicationService).Assembly;

            var interfaces = assembly.GetTypes()
                .Where(IsAssignableApplicationServiceInterface)
                .ToList();

            foreach (var interfaceType in interfaces)
            {
                var implementingType = GetImplementingType(interfaceType, assembly);

                if (implementingType != null)
                {
                    services.AddTransient(interfaceType, implementingType);
                }
                else
                {
                    throw new InvalidOperationException($"Failed to find an implementing type for interface {interfaceType.FullName}");
                }
            }
        }

        private static bool IsAssignableApplicationServiceInterface(Type type)
        {
            return type.Name != nameof(IApplicationService) && typeof(IApplicationService).IsAssignableFrom(type) && type.IsInterface;
        }

        private static Type? GetImplementingType(Type interfaceType, Assembly assembly)
        {
            return assembly.GetTypes()
                .FirstOrDefault(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
        }
    }
}
