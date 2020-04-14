using Autofac;
using Autofac.Integration.WebApi;
using LinkIT.Data;
using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Infrastructure.Api;
using LinkIT.Web.Infrastructure.Api.Shibboleth;
using LinkIT.Web.Infrastructure.Api.Shibboleth.Auth;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace LinkIT.Web
{
	public class WebApiDependencies
	{
		private void RegisterControllers(ContainerBuilder builder) =>
			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

		private void RegisterServices(ContainerBuilder builder)
		{
			builder.RegisterType<Log4NetExceptionLogger>().As<IExceptionLogger>();

			builder.RegisterType<ShibbolethAuthorizer>();
			if (ShibbolethAttributesMock.ShouldMock)
			{
				builder.Register(x => ShibbolethAttributesMock.FromConfig()).SingleInstance();
			}
			else
			{
				builder.Register(x => ShibbolethAttributes.FromHeader()).InstancePerRequest();
			}
		}

		private void RegisterRepositories(ContainerBuilder builder)
		{
			builder.RegisterType<ConnectionString>().SingleInstance();

			builder.RegisterType<SpecialOwnerRepository>().As<IRepository<SpecialOwnerDto, SpecialOwnerQuery>>();
			builder.RegisterType<ProductRepository>().As<IRepository<ProductDto, ProductQuery>>();
			builder.RegisterType<AssetRepository>().As<IAssetRepository>();
			builder.RegisterType<AssetHistoryRepository>().As<IRepository<AssetHistoryDto, AssetHistoryQuery>>();
			builder.RegisterType<UserRoleRepository>().As<IUserRoleRepository>();
		}

		public void Register(HttpConfiguration config)
		{
			var builder = new ContainerBuilder();

			RegisterControllers(builder);
			RegisterServices(builder);
			RegisterRepositories(builder);

			var container = builder.Build();
			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
		}
	}
}