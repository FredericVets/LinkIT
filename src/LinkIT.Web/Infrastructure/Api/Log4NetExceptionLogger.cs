using log4net;
using System.Web.Http.ExceptionHandling;

namespace LinkIT.Web.Infrastructure.Api
{
	/// <summary>
	/// Plugs into the ASP.NET WebApi exception logging mechanism. See WebApiConfig for registration.
	/// </summary>
	public class Log4NetExceptionLogger : ExceptionLogger
	{
		private readonly ILog _log;

		public Log4NetExceptionLogger()
		{
			_log = LogManager.GetLogger(GetType());
		}

		public override void Log(ExceptionLoggerContext context)
		{
			_log.Fatal($"An unhandled exception occurred in '{context.CatchBlock}'.", context.Exception);
		}
	}
}