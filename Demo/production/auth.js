// ==================== SISTEMA DE AUTENTICACIÓN OBLIGATORIO ====================
// Este archivo debe ser incluido en TODAS las páginas del sistema

// Función para validar autenticación OBLIGATORIA
function validateAuthentication() {
    var currentUser = localStorage.getItem('currentUser');
    var sessionToken = localStorage.getItem('sessionToken');
    var loginTime = localStorage.getItem('loginTime');
    
    // Verificar si hay usuario autenticado
    if (!currentUser || !sessionToken) {
        redirectToLogin();
        return false;
    }
    
    // Verificar si la sesión ha expirado (24 horas) - Solo si hay loginTime
    if (loginTime && loginTime !== 'null' && loginTime !== 'undefined') {
        var loginTimestamp = parseInt(loginTime);
        var currentTime = Date.now();
        var sessionDuration = 24 * 60 * 60 * 1000; // 24 horas en milisegundos
        
        if (!isNaN(loginTimestamp) && currentTime - loginTimestamp > sessionDuration) {
            // Sesión expirada
            clearAuthentication();
            redirectToLogin();
            return false;
        }
    }
    
    return true;
}

// Función para limpiar datos de autenticación
function clearAuthentication() {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('sessionToken');
    localStorage.removeItem('loginTime');
}

// Función para redirigir al login
function redirectToLogin() {
    // Limpiar datos de sesión
    clearAuthentication();
    
    // Mostrar mensaje de acceso restringido
    if (typeof PNotify !== 'undefined') {
        new PNotify({
            title: 'Acceso Restringido',
            text: 'Debe iniciar sesión para acceder al sistema.',
            type: 'error',
            hide: false,
            styling: 'bootstrap3'
        });
    }
    
    // Redirigir al login INMEDIATAMENTE
    window.location.href = 'login.html';
}

// Función para obtener el usuario actual (PROTEGIDA)
function getCurrentUser() {
    // Primero validar autenticación
    if (!validateAuthentication()) {
        return null; // Se redirige automáticamente
    }
    
    var user = localStorage.getItem('currentUser');
    return user;
}

// Función para establecer sesión de usuario (solo desde login)
function setUserSession(username, token) {
    localStorage.setItem('currentUser', username);
    localStorage.setItem('sessionToken', token);
    localStorage.setItem('loginTime', Date.now().toString());
}

// Función para cerrar sesión
function logout() {
    clearAuthentication();
    window.location.href = 'login.html';
}

// ==================== VALIDACIÓN AUTOMÁTICA AL CARGAR LA PÁGINA ====================
// Esta función se ejecuta automáticamente cuando se incluye este script

$(document).ready(function() {
    // Solo validar autenticación si no estamos en la página de login
    var currentPath = window.location.pathname;
    var isLoginPage = currentPath.includes('login.html');
    
    if (!isLoginPage) {
        // VALIDAR AUTENTICACIÓN OBLIGATORIA AL CARGAR CUALQUIER PÁGINA
        if (!validateAuthentication()) {
            // Si no está autenticado, se redirige automáticamente al login
            return;
        }
    }
    
    // Si está autenticado, continuar con la carga normal de la página
    console.log('Usuario autenticado:', localStorage.getItem('currentUser'));
});

// ==================== PROTECCIÓN DE FUNCIONES CRÍTICAS ====================
// Interceptar navegación directa a páginas protegidas
window.addEventListener('beforeunload', function() {
    // Opcional: Validar sesión antes de cerrar
});

// Interceptar intentos de acceso directo (solo si no es login)
var currentPath = window.location.pathname;
var isLoginPage = currentPath.includes('login.html');

if (!isLoginPage) {
    // Solo validar si no estamos en la página de login
    if (!localStorage.getItem('currentUser') || !localStorage.getItem('sessionToken')) {
        window.location.href = 'login.html';
    }
}
