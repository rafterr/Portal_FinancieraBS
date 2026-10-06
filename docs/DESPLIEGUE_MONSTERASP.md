# Publicar en MonsterASP.NET

Los nombres de las opciones del panel pueden variar. Si alguna no coincide, busca la equivalente en tu panel de MonsterASP.

## 1. Base de datos
1. En el panel de MonsterASP, crea una base de datos **MySQL** y anota servidor, puerto, nombre de la base, usuario y contraseña.
2. No hace falta crear tablas: la aplicación las crea al arrancar con las migraciones.
   Si prefieres crearlas a mano, ejecuta `database/esquema_inicial.sql` desde el administrador de base de datos del panel (por ejemplo phpMyAdmin).

> **Si tu plan solo ofrece SQL Server (MSSQL):** hay que cambiar el proveedor de EF Core (`UseMySql` → `UseSqlServer`) y regenerar las migraciones. Es un cambio pequeño, pero las migraciones actuales son específicas de MySQL.

## 2. Configuración de producción (secretos)
1. Copia `FinancieraBS/appsettings.Production.example.json` como `FinancieraBS/appsettings.Production.json`.
2. Llena la cadena de conexión con los datos del paso 1 y define el administrador inicial (`AdminInicial`). Usa una contraseña distinta a la local.
3. **No subas este archivo al repositorio.** Ya está en `.gitignore`. Visual Studio lo incluye al publicar porque está dentro del proyecto.

Si el panel permite definir variables de entorno, puedes usarlas en lugar del archivo. Se escriben con doble guion bajo, por ejemplo `ConnectionStrings__DefaultConnection` y `AdminInicial__Password`.

## 3. Publicar desde Visual Studio
1. En el panel de MonsterASP, descarga el **perfil de publicación** (Web Deploy) de tu sitio.
2. En Visual Studio: clic derecho en `FinancieraBS` → **Publicar** → **Importar perfil** → selecciona el archivo descargado.
3. En la configuración del perfil:
   - Configuración: **Release**. Plataforma de destino: **net8.0**.
   - Modo de implementación: **Dependiente del marco** (framework-dependent), si el hosting tiene .NET 8. Si no, usa **Autocontenido** con `win-x64`.
   - **No** marques "Quitar archivos adicionales en el destino": borraría `App_Data/documentos`, donde están los documentos subidos.
4. Publica y abre el sitio. El primer arranque crea las tablas, los roles y el administrador.

Alternativa sin Visual Studio:
```bash
dotnet publish FinancieraBS -c Release -o publicar
```
Después sube el contenido de la carpeta `publicar` por FTP o con el administrador de archivos del panel.

## 4. Después de publicar
- Entra con el administrador y crea los usuarios Cobrador desde **Usuarios**.
- Cambia la contraseña del administrador. Cuando ya exista un administrador puedes quitar `AdminInicial:Password` del archivo de producción, porque solo se usa la primera vez.
- Activa el **SSL/HTTPS** del sitio en el panel. La cookie de sesión solo viaja por HTTPS en producción.
- Verifica que la aplicación puede escribir en `App_Data`. Ahí se guardan los documentos (`App_Data/documentos`) y las llaves de sesión (`App_Data/keys`). Si al subir un documento aparece un error de permisos, revisa los permisos de esa carpeta en el panel.

## 5. Respaldos
- **Base de datos:** exporta la base MySQL desde el panel con regularidad.
- **Documentos:** descarga la carpeta `App_Data/documentos` por FTP. No está incluida en el respaldo de la base.

## 6. Actualizaciones
Vuelve a publicar con el mismo perfil. Si el cambio incluye migraciones nuevas, se aplican solas al reiniciar la aplicación.
