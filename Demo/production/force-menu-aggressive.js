// ==================== FORZAR VISIBILIDAD AGRESIVA ====================
// Script ULTRA AGRESIVO para mostrar todos los menús

$(document).ready(function() {
    function ultraForceMenus() {
        var currentUser = localStorage.getItem('currentUser');
        
        if (currentUser !== 'admin') {
            console.log('Ultra Force - Usuario:', currentUser, '- Forzando TODOS los menús');
            
            // MÉTODO ULTRA AGRESIVO: Buscar por texto y mostrar
            var menuItems = [
                'Contratos', 'Inventario', 'Campamentos', 'Dotaciones', 
                'Roles', 'Calendario', 'Servicios Disponibles'
            ];
            
            menuItems.forEach(function(item) {
                $('a').each(function() {
                    if ($(this).text().toLowerCase().includes(item.toLowerCase())) {
                        $(this).closest('li').show();
                        $(this).closest('li').css('display', 'block !important');
                        $(this).closest('li').removeClass('admin-hidden');
                        console.log('Ultra Force - Mostrado:', item);
                    }
                });
            });
            
            // Forzar visibilidad de todos los elementos con clase admin-hidden
            $('.admin-hidden').each(function() {
                $(this).removeClass('admin-hidden');
                $(this).show();
                $(this).css('display', 'block !important');
                $(this).css('visibility', 'visible !important');
                $(this).css('opacity', '1 !important');
                $(this).css('height', 'auto !important');
                $(this).css('overflow', 'visible !important');
            });
            
            // Mostrar todos los li del menú
            $('.nav.side-menu li').show();
            $('.nav.side-menu li').css('display', 'block !important');
            
            // Mostrar todos los a del menú
            $('.nav.side-menu a').show();
            $('.nav.side-menu a').css('display', 'block !important');
        }
    }
    
    // Ejecutar múltiples veces
    ultraForceMenus();
    setTimeout(ultraForceMenus, 100);
    setTimeout(ultraForceMenus, 500);
    setTimeout(ultraForceMenus, 1000);
    setTimeout(ultraForceMenus, 2000);
    setTimeout(ultraForceMenus, 3000);
    
    // Ejecutar cada 2 segundos
    setInterval(ultraForceMenus, 2000);
});

// También ejecutar cuando el DOM esté listo
$(document).on('DOMContentLoaded', function() {
    setTimeout(function() {
        var currentUser = localStorage.getItem('currentUser');
        if (currentUser !== 'admin') {
            $('.admin-hidden').removeClass('admin-hidden').show();
        }
    }, 100);
});
