using LinkIT.Data.Extensions;
using Swashbuckle.Swagger;
using System.Linq;
using System.Web.Http.Description;

namespace LinkIT.Web.Filters.Swagger
{
	/// <summary>
	/// By default Swagger generates query string parameter names for complex types as 'obj.property'. 
	/// We only want the property as this is what will effectively be passed into the query string.
	/// E.g. 'filter.createdBy' becomes 'createdBy'.
	/// See also https://stackoverflow.com/questions/37705821/swagger-ui-displaying-the-asp-net-webapi-parameter-name-with-dot-notation
	/// </summary>
	public class QueryStringParameterFilter : IOperationFilter
	{
		public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
		{
			if (operation.parameters == null)
				return;

			var queryStringParams = operation.parameters.Where(p => p.@in == "query" && p.name.Contains("."));
			foreach (var param in queryStringParams)
			{
				param.name = param.name.SplitForSeparator('.').Last();
			}
		}
	}
}