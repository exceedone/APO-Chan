using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Configuration;
using System.Web.Http.Controllers;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Apo_ChanService.Models
{
    public class CustomAttributes
    {
        /// <summary>
        /// Decimal keta
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public sealed class DecimalPrecisionAttribute : Attribute
        {
            public DecimalPrecisionAttribute(byte precision, byte scale)
            {
                Precision = precision;
                Scale = scale;
            }

            public byte Precision { get; set; }
            public byte Scale { get; set; }
        }

        public class DateFormatConverter : Newtonsoft.Json.Converters.IsoDateTimeConverter
        {
            public DateFormatConverter(string format = null)
            {
                if (string.IsNullOrWhiteSpace(format))
                {
                    format = "yyyy-MM-dd";
                }
                base.DateTimeFormat = format;
            }
        }

        public class CustomAuthentizeAttribute : AuthorizeAttribute
        {
            protected override bool IsAuthorized(HttpActionContext actionContext)
            {
                // get web.config value.
                bool isAuthentize = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["IsAuthentize"], typeof(bool));

                // if "isAuthentize" is false, always OK, so return true
                if (!isAuthentize)
                {
                    return true;
                }

                return base.IsAuthorized(actionContext);
            }
        }
    }



    public class DecimalPrecisionAttributeConvention
    : PrimitivePropertyAttributeConfigurationConvention<CustomAttributes.DecimalPrecisionAttribute>
    {
        public override void Apply(ConventionPrimitivePropertyConfiguration configuration, CustomAttributes.DecimalPrecisionAttribute attribute)
        {
            if (attribute.Precision < 1 || attribute.Precision > 38)
            {
                throw new InvalidOperationException("Precision must be between 1 and 38.");
            }

            if (attribute.Scale > attribute.Precision)
            {
                throw new InvalidOperationException("Scale must be between 0 and the Precision value.");
            }

            configuration.HasPrecision(attribute.Precision, attribute.Scale);
        }
    }
}