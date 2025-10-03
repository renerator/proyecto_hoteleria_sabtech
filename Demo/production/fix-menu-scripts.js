// ==================== SCRIPT PARA CORREGIR MENÚS EN TODAS LAS PÁGINAS ====================
// Este script se ejecuta en cada página para asegurar que los menús funcionen correctamente

$(document).ready(function() {
    // Función para forzar visibilidad de menús
    function fixMenuVisibility() {
        var currentUser = localStorage.getItem('currentUser');
        
        if (currentUser !== 'admin') {
            console.log('Fix Menu - Usuario:', currentUser, '- Corrigiendo visibilidad del menú');
            
            // Forzar visibilidad de todos los elementos admin-hidden
            $('.admin-hidden').each(function() {
                $(this).removeClass('admin-hidden');
                $(this).show();
                $(this).css('display', 'block !important');
                $(this).css('visibility', 'visible !important');
                $(this).css('opacity', '1 !important');
            });
            
            // Buscar y mostrar elementos por texto
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
                    }
                });
            });
            
            // Forzar visibilidad del menú completo
            $('.nav.side-menu li').show();
            $('.nav.side-menu li').css('display', 'block !important');
            
            // Establecer texto correcto del usuario
            var displayText = '';
            if (currentUser === 'plataforma') {
                displayText = 'Plataforma';
            } else if (currentUser === 'huesped') {
                displayText = 'Huésped';
            } else if (currentUser === 'mantencion') {
                displayText = 'Mantención';
            } else {
                displayText = 'Usuario';
            }
            
            // Forzar el texto del usuario
            $('.profile_info h2').text(displayText);
            
            // Agregar indicador visual
            if (currentUser === 'plataforma') {
                // Limpiar badges anteriores
                $('.admin-title').remove();
                
                // Agregar badge de plataforma
                $('.navbar-brand').append('<span class="admin-title" style="background: linear-gradient(135deg, #2c3e50 0%, #34495e 100%); color: white; padding: 5px 10px; border-radius: 4px; font-size: 12px; margin-left: 10px;">PLATAFORMA</span>');
            }
        }
    }
    
    // Ejecutar inmediatamente
    fixMenuVisibility();
    
    // Ejecutar después de 500ms
    setTimeout(fixMenuVisibility, 500);
    
    // Ejecutar después de 1000ms
    setTimeout(fixMenuVisibility, 1000);
    
    // Ejecutar cada 2 segundos para mantenerlo
    setInterval(fixMenuVisibility, 2000);
});
