using Autofac;
using Autofac.Integration.WebApi;
using LinkIT.Data;
using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Infrastructure.Api;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace LinkIT.Web
{
	public class WebApiDependencies
	{
		private void RegisterServices(ContainerBuilder builder)
		{
			builder.RegisterType<Log4NetExceptionLogger>().As<IExceptionLogger>();
		}

		private void RegisterRepositories(ContainerBuilder builder)
		{
			builder.RegisterType<ConnectionString>().SingleInstance();

			// TODO : Play around with generic registration.
			builder.RegisterType<SpecialOwnerRepository>().As<IRepository<SpecialOwnerDto, SpecialOwnerQuery>>();
			builder.RegisterType<ProductRepository>().As<IRepository<ProductDto, ProductQuery>>();
			builder.RegisterType<AssetRepository>().As<IAssetRepository>();
			builder.RegisterType<AssetHistoryRepository>().As<IRepository<AssetHistoryDto, AssetHistoryQuery>>();
			builder.RegisterType<UserRoleRepository>().As<IUserRoleRepository>();
		}

		public void Register(HttpConfiguration config)
		{
			var builder = new ContainerBuilder();

			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
			RegisterServices(builder);
			RegisterRepositories(builder);

			var container = builder.Build();
			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
		}
	}
}