using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using System.Net.Http;
using Microsoft.Azure.Mobile.Server;
using Apo_ChanService.DataObjects;
using Apo_ChanService.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Apo_ChanService.Controllers
{
    [CustomAttributes.CustomAuthentize]
    [MobileAppController]
    public class TokenController : ApiController
    {
        // GET api/token/X-MS-TOKEN-FACEBOOK-ACCESS-TOKEN
        [HttpGet]
        public string Get()
        {
            // get querystring "key".
            var keys = Request.GetQueryNameValuePairs().Where(x => x.Key == "key");
            if (!keys.Any())
            {
                return string.Empty;
            }
            // get header token.
            IEnumerable<string> headers;
            if (Request.Headers.TryGetValues(keys.First().Value, out headers))
            {
                return headers.FirstOrDefault();
            }
            return string.Empty;
        }
    }
}