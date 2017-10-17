using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Apo_ChanService.DataObjects;
using Apo_ChanService.Models;
using Microsoft.Azure.Mobile.Server.Tables;
using System.Net.Http;

namespace Apo_ChanService.Controllers
{
    public abstract class BaseController<T> : TableController<T> where T : class, ITableData
    {
        protected Apo_ChanContext context;

        /// <summary>
        /// get object by query string
        /// </summary>
        /// <param name="key"></param>
        /// <returns>if length(query) == 0 or query-keys has no Object key then null. else object.</returns>
        protected T1 getObjByQuery<T1>() where T1 : class, new()
        {
            T1 t = new T1();
            // get all items from query
            var list = Request.GetQueryNameValuePairs();
            if(list.Count() == 0)
            {
                return null;
            }

            bool hasKey = false;
            foreach (var item in list)
            {
                // set value if obj has item.Key
                var prop = typeof(T1).GetProperty(item.Key);
                if (prop != null)
                {
                    dynamic d;
                    // check enum
                    if (prop.PropertyType.IsEnum)
                    {
                        d = Enum.Parse(prop.PropertyType, item.Value);
                    }
                    // check nulable enum
                    else if (prop.PropertyType.IsNullableEnum())
                    {
                        d = Enum.Parse(Nullable.GetUnderlyingType(prop.PropertyType), item.Value);
                    }
                    // other
                    else
                    {
                        d = Convert.ChangeType(item.Value, prop.PropertyType);
                    }

                    prop.SetValue(t, d);
                    hasKey = true;
                }
            }
            if (!hasKey) { return null; }
            return t;
        }
    }
}