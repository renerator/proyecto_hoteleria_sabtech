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
                url: "SolicitudServicio",
                defaults: new { controller = "SolicitudServicio", action = "Index" }
            );
            routes.MapRoute(
            name: "PanelPrincipalRoot",
            url: "PanelPrincipal",
            defaults: new { controller = "PanelPrincipal", action = "Index" }
           );

            routes.MapRoute(
            name: "ServicioDisponiblesPrincipalRoot",
            url: "ServiciosDisponibles",
            defaults: new { controller = "ServiciosDisponibles", action = "Index" }
           );
            routes.MapRoute(
           name: "ContratosRoot",
           url: "Contratos",
           defaults: new { controller = "Contratos", action = "Index" }
            );
            routes.MapRoute(
           name: "DotacionesRoot",
           url: "Dotaciones",
           defaults: new { controller = "Dotaciones", action = "Index" }
            );
            routes.MapRoute(
          name: "ReservasHuespedRoot",
          url: "ReservasHuesped",
          defaults: new { controller = "ReservasHuesped", action = "Index" }
           );
            routes.MapRoute(
          name: "ReclamosHuespedRoot",
          url: "ReclamosHuesped",
          defaults: new { controller = "ReclamosHuesped", action = "Index" }
           );
            routes.MapRoute(
         name: "ServiciosHuespedRoot",
         url: "ServiciosHuesped",
         defaults: new { controller = "ServiciosHuesped", action = "Index" }
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
