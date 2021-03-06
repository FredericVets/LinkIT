﻿using Autofac;
using Autofac.Integration.WebApi;
using LinkIT.Data;
using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Infrastructure.Api;
using LinkIT.Web.Infrastructure.Auth;
using System.Reflection;
using System.Web.Http.ExceptionHandling;

namespace LinkIT.Web
{
	public class DependencyContainerBuilder
	{
		private static void RegisterApiControllers(ContainerBuilder builder) =>
			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

		private static void RegisterServices(ContainerBuilder builder)
		{
			builder.RegisterType<Log4NetExceptionLogger>().As<IExceptionLogger>();

			RegisterJsonWebTokenValidation(builder);
		}

		private static void RegisterJsonWebTokenValidation(ContainerBuilder builder)
		{
			builder.RegisterType<JsonWebTokenAuthorizer>();

			if (JsonWebTokenWrapperMock.ShouldMock)
			{
				builder.RegisterType<JsonWebTokenWrapperMock>().As<IJsonWebTokenWrapper>().SingleInstance();

				return;
			}

			builder.Register(_ => JsonWebKeySetWrapper.FromConfig()).SingleInstance();

			builder.Register(_ => HttpHeadersWrapper.FromCurrentContext()).InstancePerRequest();
			builder.RegisterType<JsonWebTokenWrapper>().As<IJsonWebTokenWrapper>().InstancePerRequest();
		}

		private static void RegisterRepositories(ContainerBuilder builder)
		{
			builder.RegisterType<ConnectionString>().SingleInstance();

			builder.RegisterType<SpecialOwnerRepository>().As<IRepository<SpecialOwnerDto, SpecialOwnerQuery>>();
			builder.RegisterType<ProductRepository>().As<IRepository<ProductDto, ProductQuery>>();
			builder.RegisterType<AssetRepository>().As<IAssetRepository>();
			builder.RegisterType<AssetHistoryRepository>().As<IRepository<AssetHistoryDto, AssetHistoryQuery>>();
			builder.RegisterType<UserRoleRepository>().As<IUserRoleRepository>();
		}

		public IContainer Build()
		{
			var builder = new ContainerBuilder();

			RegisterApiControllers(builder);
			RegisterServices(builder);
			RegisterRepositories(builder);

			return builder.Build();
		}
	}
}