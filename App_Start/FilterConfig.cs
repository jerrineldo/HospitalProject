using System.Web;
using System.Web.Mvc;

namespace Red_Lake_Hospital_Redesign_Team6
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
