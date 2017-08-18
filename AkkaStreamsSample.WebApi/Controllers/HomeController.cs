using System.Web.Http;

namespace AkkaStreamsSample.WebApi.Controllers
{
    public class HomeController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok("hello from api...");
        }
    }
}