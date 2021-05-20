using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Autobarn.Website.Controllers.Api {
    public static class ObjectExtensions {
        public static dynamic ToDynamic(this object value) {
			IDictionary<string, object> expando = new ExpandoObject();
			var properties = TypeDescriptor.GetProperties(value.GetType());
			foreach (PropertyDescriptor property in properties) {
				if (property.Attributes.OfType<JsonIgnoreAttribute>().Any()) continue;
				expando.Add(property.Name, property.GetValue(value));
			}
			return (ExpandoObject)expando;
		}        
    }
}
