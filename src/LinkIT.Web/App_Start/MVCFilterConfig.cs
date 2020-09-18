using System.Web.Mvc;

namespace LinkIT.Web
{
    public class MVCFilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // When no filter is applied, errors are passed on to the Application_Error() method in global.asax.
            // filters.Add(new HandleErrorAttribute());
        }
    }
}