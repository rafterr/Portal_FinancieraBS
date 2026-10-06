# Guía de Uso - Sistema de Gestión Financiera

## Descripción General

Sistema completo para la gestión de préstamos financieros con las siguientes funcionalidades:

- ✅ Gestión de Usuarios (CRUD completo)
- ✅ Gestión de Clientes (CRUD completo)
- ✅ Gestión de Préstamos (CRUD completo)
- ✅ Gestión de Pagos (CRUD completo)
- ✅ Almacenamiento de documentos en Firebase (Pagaré, INE, Comprobante de Domicilio)
- ✅ Diseño responsivo y sencillo

## Módulos del Sistema

### 1. Usuarios

**Ruta**: `/Usuarios`

Permite administrar los usuarios del sistema que gestionarán clientes, préstamos y pagos.

**Funciones**:
- Crear nuevo usuario
- Listar todos los usuarios
- Editar información de usuario
- Eliminar usuario

**Campos**:
- Nombre de usuario
- Email
- Teléfono
- Fecha de creación (automática)

### 2. Clientes

**Ruta**: `/Clientes`

Gestión completa de clientes con carga de documentos a Firebase.

**Funciones**:
- Registrar nuevo cliente
- Listar todos los clientes
- Editar información del cliente
- Eliminar cliente (elimina también sus documentos de Firebase)

**Campos**:
- Nombre
- Apellidos
- Dirección
- Teléfono
- Email
- Estatus (Activo/Inactivo)
- Documentos:
  - Pagaré (opcional)
  - INE (opcional)
  - Comprobante de Domicilio (opcional)

**Formatos aceptados**: JPG, PNG, PDF

### 3. Préstamos

**Ruta**: `/Prestamos`

Gestión de préstamos con cálculo automático de totales y carga de documentación.

**Funciones**:
- Crear nuevo préstamo
- Listar todos los préstamos
- Editar préstamo
- Eliminar préstamo

**Campos**:
- Cliente (selección)
- Monto solicitado
- Fecha de inicio
- Interés (%)
- Estatus (Pagado, En Proceso, Retraso)
- Documentos del préstamo:
  - Pagaré (opcional)
  - INE del cliente (opcional)

**Cálculos automáticos**:
- Total = Monto + (Monto × Interés / 100)
- Fecha fin = Fecha inicio + 14 semanas
- Saldo restante = Total - Pagos realizados

**Estados del préstamo**:
- 🟢 **Pagado**: Saldo restante = 0
- 🔵 **En Proceso**: Préstamo activo con saldo pendiente
- 🔴 **Retraso**: Préstamo vencido

### 4. Pagos

**Ruta**: `/Pagos`

Registro de pagos realizados por los clientes.

**Funciones**:
- Registrar nuevo pago
- Listar todos los pagos
- Editar pago
- Eliminar pago

**Campos**:
- Préstamo (selección)
- Cliente (selección)
- Monto del pago
- Fecha del pago (automática al crear)

**Comportamiento**:
- Al registrar un pago, se actualiza automáticamente el saldo restante del préstamo
- Si el saldo llega a 0, el préstamo cambia a estado "Pagado"

## Flujo de Trabajo Recomendado

### Proceso completo de un préstamo:

1. **Crear Usuario** (si no existe)
   - Ir a `/Usuarios/Create`
   - Registrar datos del usuario del sistema

2. **Registrar Cliente**
   - Ir a `/Clientes/Create`
   - Ingresar datos personales
   - Subir documentos (Pagaré, INE, Comprobante)
   - Los archivos se guardan automáticamente en Firebase

3. **Crear Préstamo**
   - Ir a `/Prestamos/Create`
   - Seleccionar cliente
   - Ingresar monto e interés
   - Opcionalmente subir documentación adicional
   - El sistema calcula automáticamente el total

4. **Registrar Pagos**
   - Ir a `/Pagos/Create`
   - Seleccionar préstamo y cliente
   - Ingresar monto del pago
   - El sistema actualiza el saldo automáticamente

5. **Seguimiento**
   - Consultar estado de préstamos en `/Prestamos`
   - Ver historial de pagos en `/Pagos`
   - Actualizar estatus según sea necesario

## Características del Diseño

### Responsivo
- ✅ Adaptable a móviles, tablets y escritorio
- ✅ Tablas con scroll horizontal en pantallas pequeñas
- ✅ Botones y formularios optimizados para touch

### Sencillo
- ✅ Navegación clara en el menú superior
- ✅ Formularios organizados en columnas
- ✅ Validación de campos requeridos
- ✅ Mensajes de confirmación para eliminaciones
- ✅ Badges de colores para estados

### Accesible
- ✅ Labels descriptivos en formularios
- ✅ Contraste adecuado de colores
- ✅ Botones con texto claro

## Tecnologías Utilizadas

- **Backend**: ASP.NET Core 8.0 MVC
- **Base de datos**: MySQL
- **Almacenamiento**: Firebase Storage
- **Frontend**: Bootstrap 5, HTML5, CSS3
- **Arquitectura**: Capas (Presentación, Negocio, Datos)

## Seguridad

- Validación de formularios en cliente y servidor
- Tokens anti-falsificación en formularios
- Autenticación de Firebase para subida de archivos
- Eliminación segura de archivos al borrar registros

## Soporte

Para problemas o dudas:
1. Revisar logs de la aplicación
2. Verificar configuración de Firebase
3. Comprobar conexión a base de datos MySQL
