using System.Web.Mvc;

namespace LinkIT.Web
{
    public class MVCFilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}