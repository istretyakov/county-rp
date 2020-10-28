using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using CountyRP.Forum.Domain.Interfaces;
using CountyRP.Forum.Infrastructure;
using CountyRP.Forum.Domain.Models;

namespace CountyRP.Forum.WebAPI
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
            services.AddControllers();

            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ForumContext>(options => options.UseMySql(connectionString));
            services.AddDbContext<TopicContext>(options => options.UseMySql(connectionString));
            services.AddDbContext<PostContext>(options => options.UseMySql(connectionString));

            services.AddTransient<IForumRepository, ForumRepository>();
            services.AddTransient<ITopicRepository, TopicRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
