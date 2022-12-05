using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAPI.Data;
using UserAPI.Services;


namespace UserAPI
{
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
            //Defini��o da utiliza��o do banco de dados 
            //UserConnection string de conex�o onde estamos definindo o servidor, o banco de dados que
            //eu chamei de UsuarioDb e o user= e password=, as nossas credenciais de acesso ao banco.
            services.AddDbContext<UserDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("UsuarioConnection")));
            //Identity
            services
                .AddIdentity<IdentityUser<int>, IdentityRole<int>>(opt =>
                {
                    //Bloqueando login de usu�rios com uma conta n�o confirmada?
                    opt.SignIn.RequireConfirmedEmail = true;
                })
                //Store indicador do armazenamento de dados que est�o sendo usados para identifica��o.
                //FrameworkStore que vamos utilizar � a nossa UserDbContext
                .AddEntityFrameworkStores<UserDbContext>()
                //Gerador de c�digos de ativa��o, confirma��es e afins.
                //ele � usado para gerar tokens para resetar senha, para trocar e-mail, para trocar
                //telefone, esse tipo de opera��o dentro do Identity.
                .AddDefaultTokenProviders();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserAPI", Version = "v1" });
            });
            //config AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<EmailService, EmailService>();
            //Inje��o de CadastroService dentro do nosso controlador
            services.AddScoped<CadastroService, CadastroService>();
            services.AddScoped<TokenService, TokenService>();
            services.AddScoped<LoginService, LoginService>();
            services.AddScoped<LogoutService, LogoutService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}