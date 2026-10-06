# Instrucciones de Ejecución del Sistema

## Requisitos Previos

1. **.NET 8.0 SDK** instalado
2. **MySQL Server** instalado y ejecutándose
3. **Cuenta de Firebase** configurada
4. **Visual Studio 2022** o **VS Code** (opcional)

## Pasos para Ejecutar el Proyecto

### 1. Configurar Base de Datos MySQL

```bash
# Conectar a MySQL
mysql -u root -p

# Ejecutar el script de configuración
source database_setup.sql
```

O manualmente:
```bash
mysql -u root -p < database_setup.sql
```

### 2. Configurar Firebase

Sigue las instrucciones en `CONFIGURACION_FIREBASE.md`:

1. Crear proyecto en Firebase Console
2. Habilitar Storage y Authentication
3. Obtener API Key y Bucket
4. Actualizar `appsettings.json` con las credenciales

### 3. Restaurar Paquetes NuGet

```bash
# Desde la raíz del proyecto
dotnet restore
```

### 4. Aplicar Migraciones (si es necesario)

```bash
# Navegar al proyecto DataLayer
cd DataLayer

# Crear migración inicial
dotnet ef migrations add InitialCreate --startup-project ../FinancieraBS

# Aplicar migración
dotnet ef database update --startup-project ../FinancieraBS

# Volver a la raíz
cd ..
```

### 5. Compilar el Proyecto

```bash
dotnet build
```

### 6. Ejecutar la Aplicación

```bash
# Opción 1: Ejecutar directamente
dotnet run --project FinancieraBS

# Opción 2: Con hot reload
dotnet watch run --project FinancieraBS
```

### 7. Acceder a la Aplicación

Abre tu navegador en:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001

## Estructura del Proyecto

```
FinancieraBS/
├── BusinessInterfase/      # Interfaces de lógica de negocio
├── BusinessLayer/          # Implementación de lógica de negocio
├── BusinessType/           # Modelos de dominio
├── DataInterfase/          # Interfaces de repositorios
├── DataLayer/              # Implementación de repositorios
└── FinancieraBS/           # Capa de presentación (MVC)
    ├── Controllers/        # Controladores
    ├── Views/              # Vistas Razor
    ├── Services/           # Servicios (Firebase)
    └── wwwroot/            # Archivos estáticos
```

## Configuración de appsettings.json

Asegúrate de tener configurado:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FinancieraBS;User=root;Password=TU_PASSWORD;"
  },
  "Firebase": {
    "ApiKey": "TU_API_KEY",
    "Bucket": "tu-proyecto.appspot.com",
    "AuthEmail": "rbarronfuentes@gmail.com",
    "AuthPassword": "Himz66rafterr18"
  }
}
```

## Solución de Problemas Comunes

### Error de conexión a MySQL

```bash
# Verificar que MySQL esté ejecutándose
# Windows:
net start MySQL80

# Linux/Mac:
sudo systemctl start mysql
```

### Error de paquetes NuGet

```bash
# Limpiar y restaurar
dotnet clean
dotnet restore
```

### Error de Firebase

1. Verificar que las credenciales en `appsettings.json` sean correctas
2. Verificar que Authentication esté habilitado en Firebase Console
3. Verificar que Storage esté habilitado y las reglas configuradas

### Error de compilación

```bash
# Reconstruir solución completa
dotnet build --no-incremental
```

## Comandos Útiles

```bash
# Ver logs detallados
dotnet run --project FinancieraBS --verbosity detailed

# Ejecutar en modo producción
dotnet run --project FinancieraBS --configuration Release

# Publicar aplicación
dotnet publish FinancieraBS -c Release -o ./publish

# Limpiar archivos de compilación
dotnet clean
```

## Pruebas Iniciales

Una vez ejecutando el sistema:

1. **Crear un usuario**: `/Usuarios/Create`
2. **Registrar un cliente**: `/Clientes/Create`
3. **Crear un préstamo**: `/Prestamos/Create`
4. **Registrar un pago**: `/Pagos/Create`

## Notas Importantes

- El sistema usa **sesiones** para mantener estado
- Los archivos se suben a **Firebase Storage**
- La base de datos usa **MySQL** con Entity Framework Core
- El diseño es **responsivo** y funciona en móviles

## Soporte

Si encuentras problemas:

1. Revisa los logs en la consola
2. Verifica la configuración de `appsettings.json`
3. Asegúrate de que MySQL y Firebase estén configurados correctamente
4. Revisa que todas las dependencias estén instaladas

## Credenciales de Prueba

**Base de datos MySQL**:
- Usuario: `root`
- Password: `HIMz66rafterr18`

**Firebase**:
- Email: `rbarronfuentes@gmail.com`
- Password: `Himz66rafterr18`

⚠️ **Importante**: Cambia estas credenciales en producción.
