using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication7LogAllRequests.Utility;

namespace WebApplication7LogAllRequests
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Log all requests
            app.Use(LogAllRequestsMiddleware(env));

            app.UseMvc();
        }

        private static Func<HttpContext, Func<Task>, Task> LogAllRequestsMiddleware(IHostingEnvironment env)
        {
            return async (context, next) =>
            {
                var traceIdentifier = context.TraceIdentifier;
                var requestHeader = RequestHelper.GetRequestMethodAndUrl(context.Request);
                var headers = RequestHelper.GetAllHeaders(context.Request);
                var body = RequestHelper.ReadBodyIntoString(context.Request);

                var directory = Path.Combine(env.ContentRootPath, "Logs");
                var filename = traceIdentifier.Replace(':', '_');
                const string extension = "http";
                //if (context.Request.ContentType?.Contains("xml") == true)
                //{
                //    extension = "xml";
                //}
                //else if (context.Request.ContentType?.Contains("json") == true)
                //{
                //    extension = "json";
                //}
                //else
                //{
                //    extension = "txt";
                //}
                var filepath = $@"{directory}\{filename}.{extension}";
                File.WriteAllText(filepath, $"{requestHeader}\n");
                File.AppendAllLines(filepath, headers.Select(kv => $"{kv.Item1}: {kv.Item2}"));
                File.AppendAllText(filepath, "\n");
                File.AppendAllText(filepath, body);

                await next.Invoke();
            };
        }
    }
}
