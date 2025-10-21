using System.Web.Mvc;
using System.Web.Routing;

namespace Front_Hoteleria
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Rutas por atributo
            routes.MapMvcAttributeRoutes();

            // Atajos limpios
            routes.MapRoute(
                name: "HabitacionesRoot",
                url: "Habitaciones",
                defaults: new { controller = "Habitaciones", action = "Index" }
            );
            routes.MapRoute(
                name: "ReservasRoot",
                url: "Reservas",
                defaults: new { controller = "Reservas", action = "Index" }
            );
            routes.MapRoute(
                name: "ServiciosRoot",
                url: "Servicios",
                defaults: new { controller = "Servicios", action = "Index" }
            );

            // Ruta por defecto (si no hay login, manda a Account/Login)
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
