using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace Quests.App
{
    public class UidConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route,
            string routeKey, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            try
            {
                Uid uid = values[routeKey]?.ToString();

                return uid != new Uid();
            }
            catch
            {
                return false;
            }
        }
    }
}
