// ==================== FORZAR VISIBILIDAD DE MENÚS ====================
// Script AGRESIVO para asegurar que todos los menús sean visibles para usuarios no-admin

$(document).ready(function() {
    function forceShowAllMenus() {
        // Obtener usuario actual
        var currentUser = localStorage.getItem('currentUser');
        
        console.log('Force Menu - Usuario actual:', currentUser);
        
        // Si NO es admin, mostrar TODOS los menús
        if (currentUser !== 'admin') {
            console.log('Force Menu - FORZANDO visibilidad de todos los menús para usuario:', currentUser);
            
            // MÉTODO 1: Remover la clase admin-hidden
            $('.admin-hidden').removeClass('admin-hidden');
            
            // MÉTODO 2: Forzar visibilidad con CSS
            $('.admin-hidden').show();
            $('.admin-hidden').css('display', 'block !important');
            $('.admin-hidden').css('visibility', 'visible !important');
            $('.admin-hidden').css('opacity', '1 !important');
            
            // MÉTODO 3: Mostrar elementos específicos por ID/clase
            $('li[class*="admin-hidden"]').show();
            $('li[class*="admin-hidden"]').css('display', 'block !important');
            
            // MÉTODO 4: Buscar y mostrar elementos por texto
            $('li').each(function() {
                var text = $(this).text().toLowerCase();
                if (text.includes('contratos') || text.includes('inventario') || 
                    text.includes('campamentos') || text.includes('dotaciones') || 
                    text.includes('roles') || text.includes('calendario') ||
                    text.includes('servicios disponibles')) {
                    $(this).show();
                    $(this).css('display', 'block !important');
                }
            });
            
            // MÉTODO 5: Forzar visibilidad del menú completo
            $('.nav.side-menu li').show();
            $('.nav.side-menu li').css('display', 'block !important');
            
            console.log('Force Menu - Elementos admin-hidden encontrados:', $('.admin-hidden').length);
            console.log('Force Menu - Todos los li del menú:', $('.nav.side-menu li').length);
            
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
                
                // Mostrar notificación (solo una vez)
                if (typeof PNotify !== 'undefined' && !window.plataformaWelcomeShown) {
                    window.plataformaWelcomeShown = true;
                    new PNotify({
                        title: 'Bienvenido Plataforma',
                        text: 'Acceso completo a todas las funcionalidades del sistema',
                        type: 'success',
                        hide: true,
                        delay: 3000,
                        styling: 'bootstrap3'
                    });
                }
            }
        } else {
            console.log('Force Menu - Usuario admin detectado, manteniendo menú limitado');
        }
    }
    
    // Ejecutar inmediatamente
    forceShowAllMenus();
    
    // Ejecutar después de 500ms
    setTimeout(forceShowAllMenus, 500);
    
    // Ejecutar después de 1000ms
    setTimeout(forceShowAllMenus, 1000);
    
    // Ejecutar después de 2000ms
    setTimeout(forceShowAllMenus, 2000);
    
    // Ejecutar cada 3 segundos para mantenerlo
    setInterval(forceShowAllMenus, 3000);
});
