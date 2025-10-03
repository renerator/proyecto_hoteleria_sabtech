// ==================== LIMPIAR NOTIFICACIONES DUPLICADAS ====================
// Script para limpiar notificaciones duplicadas y configurar auto-cierre

$(document).ready(function() {
    // Configurar PNotify para auto-cierre
    if (typeof PNotify !== 'undefined') {
        // Configuración global de PNotify
        PNotify.prototype.options.hide = true;
        PNotify.prototype.options.delay = 3000; // 3 segundos
        PNotify.prototype.options.styling = 'bootstrap3';
        
        // Limpiar notificaciones existentes
        PNotify.removeAll();
    }
    
    // Función para limpiar notificaciones duplicadas
    function cleanupNotifications() {
        // Remover todas las notificaciones PNotify existentes
        if (typeof PNotify !== 'undefined') {
            PNotify.removeAll();
        }
        
        // Remover elementos de notificación duplicados del DOM
        $('.pnotify').remove();
        $('.ui-pnotify').remove();
    }
    
    // Limpiar inmediatamente
    cleanupNotifications();
    
    // Limpiar cada 2 segundos
    setInterval(cleanupNotifications, 2000);
    
    // Configurar para mostrar solo una notificación de bienvenida
    window.showWelcomeNotification = function(title, text, type) {
        // Limpiar notificaciones existentes primero
        cleanupNotifications();
        
        if (typeof PNotify !== 'undefined') {
            new PNotify({
                title: title,
                text: text,
                type: type || 'success',
                hide: true,
                delay: 3000,
                styling: 'bootstrap3',
                buttons: {
                    closer: true,
                    sticker: false
                }
            });
        }
    };
});
