// ==================== SISTEMA DE AUTENTICACIÓN SIMPLE ====================
// Versión simplificada para evitar conflictos

// Función para obtener el usuario actual (SIMPLE)
function getCurrentUser() {
    var user = localStorage.getItem('currentUser');
    
    if (user && user !== 'null' && user !== 'undefined') {
        return user;
    }
    
    return 'plataforma';
}

// Función para establecer sesión de usuario (solo desde login)
function setUserSession(username, token) {
    localStorage.setItem('currentUser', username);
    localStorage.setItem('sessionToken', token);
    localStorage.setItem('loginTime', Date.now().toString());
}

// Función para limpiar datos de autenticación
function clearAuthentication() {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('sessionToken');
    localStorage.removeItem('loginTime');
}

// Función para cerrar sesión
function logout() {
    clearAuthentication();
    window.location.href = 'login.html';
}

// Función simple para validar autenticación (NO RESTRICTIVA)
function validateAuthentication() {
    var currentUser = localStorage.getItem('currentUser');
    var sessionToken = localStorage.getItem('sessionToken');
    
    // Si no hay usuario, redirigir al login
    if (!currentUser || !sessionToken) {
        // Solo redirigir si no estamos en login
        var currentPath = window.location.pathname;
        if (!currentPath.includes('login.html')) {
            setTimeout(function() {
                window.location.href = 'login.html';
            }, 100);
        }
        return false;
    }
    
    return true;
}

// Validación simple al cargar la página
$(document).ready(function() {
    // Solo validar si no estamos en login
    var currentPath = window.location.pathname;
    if (!currentPath.includes('login.html')) {
        validateAuthentication();
    }
    
    console.log('Usuario actual:', getCurrentUser());
});
