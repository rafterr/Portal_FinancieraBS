# Configuración de Firebase Storage

## Pasos para configurar Firebase en el proyecto

### 1. Crear proyecto en Firebase Console

1. Ve a [Firebase Console](https://console.firebase.google.com/)
2. Inicia sesión con la cuenta: **rbarronfuentes@gmail.com**
3. Crea un nuevo proyecto o selecciona uno existente
4. Habilita **Firebase Storage** en el menú lateral

### 2. Configurar Authentication

1. En Firebase Console, ve a **Authentication**
2. Habilita el método de autenticación **Email/Password**
3. Agrega el usuario:
   - Email: `rbarronfuentes@gmail.com`
   - Password: `<TU_PASSWORD>`

### 3. Configurar Storage Rules

En Firebase Console, ve a **Storage > Rules** y configura las reglas:

```javascript
rules_version = '2';
service firebase.storage {
  match /b/{bucket}/o {
    match /{allPaths=**} {
      allow read, write: if request.auth != null;
    }
  }
}
```

### 4. Obtener credenciales del proyecto

1. Ve a **Project Settings** (ícono de engranaje)
2. En la pestaña **General**, busca la sección **Your apps**
3. Si no tienes una app web, crea una
4. Copia las credenciales:
   - **API Key**
   - **Storage Bucket** (formato: `tu-proyecto.appspot.com`)

### 5. Actualizar appsettings.json

Edita el archivo `FinancieraBS/appsettings.json` y reemplaza los valores:

```json
{
  "Firebase": {
    "ApiKey": "TU_API_KEY_AQUI",
    "Bucket": "tu-proyecto.appspot.com",
    "AuthEmail": "rbarronfuentes@gmail.com",
    "AuthPassword": "<TU_PASSWORD>"
  }
}
```

### 6. Estructura de carpetas en Firebase Storage

El sistema creará automáticamente estas carpetas:
- `/pagares` - Documentos de pagaré
- `/ines` - Identificaciones oficiales (INE)
- `/comprobantes` - Comprobantes de domicilio

### 7. Restaurar paquetes NuGet

Ejecuta en la terminal:

```bash
dotnet restore
```

### 8. Ejecutar el proyecto

```bash
dotnet run --project FinancieraBS
```

## Notas de Seguridad

⚠️ **IMPORTANTE**: 
- No compartas las credenciales de Firebase públicamente
- En producción, usa variables de entorno en lugar de appsettings.json
- Considera usar Firebase Admin SDK para mayor seguridad

## Formatos de archivo soportados

- Imágenes: JPG, PNG
- Documentos: PDF

## Tamaño máximo de archivo

El límite predeterminado es de 10MB por archivo. Puedes ajustarlo en las reglas de Storage si es necesario.
