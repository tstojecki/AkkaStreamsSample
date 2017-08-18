using AkkaStreamsSample;
using AkkaStreamsSample.WebApi.Hubs;
using EasyNetQ;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.BuilderProperties;
using Owin;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;

namespace AkkaStreamsSample.WebApi
{
    public class Startup
    {
        private static readonly IBus Bus = RabbitHutch.CreateBus("host=localhost");

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            ConfigureRoutes(config);
            ConfigureFormatters(config);

            app.UseWebApi(config);
            app.MapSignalR();

            ConfigureListeners();

            ConfigureShutdown(app);
        }

        void ConfigureRoutes(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute
            (
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        void ConfigureFormatters(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }

        void ConfigureListeners()
        {
            Bus.Subscribe<Message>("sub_id_1", OnMessageReceived);
        }

        void OnMessageReceived(Message message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ApplicationHub>();

            context.Clients.All.sendMessage(message);
        }

        void ConfigureShutdown(IAppBuilder app)
        {
            var properties = new AppProperties(app.Properties);
            var cancellationToken = properties.OnAppDisposing;
            if (cancellationToken != CancellationToken.None)
            {
                cancellationToken.Register(() =>
                {
                    Bus?.SafeDispose();
                });
            }
        }
    }
}