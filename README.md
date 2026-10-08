# Portal_FinancieraBS
Portal de Administración de Cobro y Captación: clientes, préstamos, pagos y sus documentos (INE, pagaré, comprobante de domicilio).

## Tecnología
- ASP.NET Core MVC (.NET 8) con Razor y Bootstrap 5
- Entity Framework Core 8 + MySQL (Pomelo) con migraciones
- ASP.NET Core Identity con roles `Admin` y `Cobrador`
- Documentos en una carpeta privada del servidor (`App_Data/documentos`), servidos solo con sesión iniciada

## Estructura
| Proyecto | Contenido |
|---|---|
| `BusinessType` | Entidades y `FinancieraContext` |
| `DataInterfase` / `DataLayer` | Repositorios, almacenamiento de documentos y migraciones (`DataLayer/Migrations`) |
| `BusinessInterfase` / `BusinessLayer` | Reglas de negocio (saldos, pagos, documentos, usuarios) |
| `FinancieraBS` | Aplicación web: controladores, vistas, configuración |
| `FinancieraBS.Tests` | Pruebas unitarias (xUnit) |

## Reglas de negocio
- **Total** = monto + (monto × interés / 100). **Fecha fin** = inicio + 14 semanas.
- **Saldo** = total − suma de pagos. Se recalcula al crear, editar o eliminar un pago.
- Con saldo 0 el préstamo pasa a **Pagado**; si después se elimina o reduce un pago, vuelve a **En Proceso**.
- No se aceptan pagos mayores al saldo. El cliente del pago se toma del préstamo.
- No se pueden eliminar préstamos con pagos ni clientes con préstamos.
- Documentos: PDF, JPG o PNG de hasta 5 MB. Se valida el contenido real del archivo. Hay un comprobante por cliente y un pagaré y una INE por préstamo; subir otro del mismo tipo lo reemplaza. Solo un `Admin` puede eliminarlos.

## Comprobantes de pago
Al registrar un pago se abre su comprobante. También se puede abrir desde la lista de Pagos o desde el detalle del préstamo. Opciones:
- **Ver / Descargar PDF**: tamaño A5, con logotipo, folio (`P-000123`), saldo anterior y saldo restante.
- **Compartir PDF**: en el celular abre el menú de compartir con el PDF adjunto (WhatsApp, correo, etc.).
- **Enviar por WhatsApp**: abre WhatsApp con el teléfono del cliente y un resumen del abono en texto. A los teléfonos de 10 dígitos se les antepone la lada `52`.

Los PDF se generan con [QuestPDF](https://www.questpdf.com/license/), con licencia *Community*: es gratuita para negocios con ingresos anuales menores a USD 1 millón.

## Datos del negocio y logotipo
En `appsettings.json`, sección `Negocio`: `Nombre`, `Telefono`, `Direccion` (aparecen en los comprobantes) y `CodigoPaisWhatsApp`.
El logotipo está en `FinancieraBS/wwwroot/img/logo.svg`. Para usar el logo propio, reemplaza ese archivo por otro **SVG cuadrado**; se usa en la barra superior, el login, el ícono del navegador y los PDF.

## Roles
- **Admin**: todo, incluido el módulo **Usuarios** (alta de usuarios, roles y contraseñas).
- **Cobrador**: clientes, préstamos, pagos y documentos.

No hay registro público. El primer administrador se crea al arrancar a partir de `AdminInicial:Email` / `AdminInicial:Password`, solo si todavía no existe ningún administrador.

## Ejecutar en local
1. Instalar .NET 8 SDK y MySQL 8.
2. Crear la base vacía:
   ```sql
   CREATE DATABASE financiera_bs CHARACTER SET utf8mb4;
   ```
3. Configurar los secretos (quedan en tu perfil de usuario, no en el repositorio):
   ```bash
   cd FinancieraBS
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=financiera_bs;User=root;Password=TU_PASSWORD;"
   dotnet user-secrets set "AdminInicial:Email" "admin@tudominio.com"
   dotnet user-secrets set "AdminInicial:Password" "tuPassword1"
   ```
   En Visual Studio: clic derecho en el proyecto `FinancieraBS` → **Administrar secretos de usuario**.
4. Ejecutar: `dotnet run --project FinancieraBS`. Al iniciar se aplican las migraciones y se crean los roles y el administrador.

> Si ya tenías la base creada con los scripts SQL anteriores, bórrala y créala vacía (paso 2). Las migraciones no pueden partir de tablas creadas a mano.

## Pruebas
```bash
dotnet test
```

## Migraciones
Después de cambiar una entidad:
```bash
dotnet tool install --global dotnet-ef --version 8.0.13   # una sola vez
dotnet ef migrations add NombreDelCambio --project DataLayer --startup-project DataLayer --output-dir Migrations
```
La aplicación aplica las migraciones pendientes al iniciar (`Database:AplicarMigracionesAlIniciar`, activado por defecto). Para generar el SQL equivalente: `dotnet ef migrations script --project DataLayer --startup-project DataLayer`.

## Publicar
Ver [docs/DESPLIEGUE_MONSTERASP.md](docs/DESPLIEGUE_MONSTERASP.md).

## Manual de usuario
[docs/Manual_de_usuario_Financiera_BS.pdf](docs/Manual_de_usuario_Financiera_BS.pdf): acceso, clientes, préstamos, pagos y comprobantes, documentos y usuarios, con capturas de pantalla.
