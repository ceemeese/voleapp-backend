# VoleApp Backend

API REST para la gestión de reservas de pistas de pádel. Permite a clubes gestionar sus instalaciones y a usuarios reservar pistas con pago integrado mediante Stripe.

## Tecnologías

- **[.NET 8](https://dotnet.microsoft.com/)** — framework principal
- **[ASP.NET Core](https://docs.microsoft.com/aspnet/core)** — capa HTTP y controladores
- **[Entity Framework Core 8](https://docs.microsoft.com/ef/core)** + **[Pomelo MySQL](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)** — ORM y base de datos
- **[MediatR](https://github.com/jbogard/MediatR)** — patrón CQRS (commands y queries)
- **[AutoMapper](https://automapper.org/)** — mapeo de entidades a DTOs
- **[FluentValidation](https://docs.fluentvalidation.net/)** — validación de comandos
- **[ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)** — gestión de usuarios y autenticación
- **[JWT Bearer](https://jwt.io/)** — autenticación stateless mediante tokens con refresh token
- **[Stripe.net](https://github.com/stripe/stripe-dotnet)** — integración de pagos con Stripe Checkout
- **[OpenWeatherMap](https://openweathermap.org/)** — datos meteorológicos para el cálculo de descuentos por condiciones climáticas
- **[SendGrid](https://sendgrid.com/)** — envío de emails transaccionales
- **[Serilog](https://serilog.net/)** + **[Seq](https://datalust.co/seq)** — logging estructurado
- **[Docker](https://www.docker.com/)** — contenedores para base de datos y Seq

## Arquitectura

El proyecto sigue **Clean Architecture** combinada con **CQRS** (via MediatR), **Repository Pattern** y **DDD**, organizado en cinco proyectos:

```
src/
├── SharedKernel/    # Tipos base: Result, Error, IDomainEvent, Entity
├── Domain/          # Entidades, value objects, domain events, domain services e interfaces de repositorio
├── Application/     # Casos de uso organizados en Commands y Queries, DTOs y validaciones
├── Infrastructure/  # EF Core, repositorios, Identity, Stripe, SendGrid, OpenWeatherMap y seeding
└── Web.Api/         # Controladores HTTP, configuración de DI y Swagger
```


## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker](https://www.docker.com/) (para MySQL y Seq)

## Configuración local

### 1. Levantar la base de datos y Seq

```bash
docker-compose -f docker-compose.dev.yml up -d
```

Esto levanta:
- **MySQL 8.3** en `localhost:3306`
- **Seq** (visor de logs) en `http://localhost:8082`

### 2. Variables de entorno

Crea el archivo `src/Web.Api/.env.dev` con el siguiente contenido:

**Para el contenedor Docker** (`docker-compose.dev.yml` lo lee automáticamente):

```env
# MySQL (solo necesario para el contenedor Docker)
MYSQL_ROOT_PASSWORD=dev-root-secret
MYSQL_DATABASE=voleapp_dev
MYSQL_USER=dev
MYSQL_PASSWORD=dev-secret
```

**Para la API:**

```env
ASPNETCORE_ENVIRONMENT=Development

# Connection string
ConnectionStrings__DefaultConnection=Server=localhost;Port=3306;Database=voleapp_dev;User=dev;Password=dev-secret;

# JWT
Jwt__Secret=tu_clave_secreta_minimo_32_caracteres

# OpenWeatherMap (https://openweathermap.org/)
WeatherApi__ApiKey=tu_api_key

# SendGrid
SendGrid__ApiKey=tu_api_key

# Stripe (modo test)
Stripe__SecretKey=sk_test_...

# Seeding
Seeding__BaseEmail=admin@ejemplo.com
Seeding__AdminPassword=Admin1234!
Seeding__UserPassword=User1234!
SeedDatabase=true
```

Para cargar las variables en la API fuera de Docker, usa secrets de .NET:

```bash
dotnet user-secrets set "Jwt__Secret" "tu_clave"
```

### 3. Ejecutar la API

```bash
dotnet run --project src/Web.Api
```

La API estará disponible en `https://localhost:7XXX`. Swagger en `/swagger`.

> Al arrancar, la aplicación aplica automáticamente las migraciones pendientes de ambos contextos (`ApplicationDbContext` e `IdentityDbContext`). Si `SeedDatabase=true` en el `.env`, también ejecuta el seeder que crea los datos iniciales (usuarios de prueba con cada rol, clubs, pistas, etc.).

## Roles

| Rol | Descripción |
|-----|-------------|
| `User` | Usuario registrado. Puede hacer reservas. |
| `Admin` | Administrador de un club. Gestiona su club. |
| `Superadmin` | Acceso total a la plataforma. |

Al ejecutar con `SeedDatabase=true` se generan automáticamente datos de prueba: usuarios con cada rol (usando las credenciales del `.env`), clubs, pistas, horarios, miembros y reservas. Los datos generados por Bogus (emails, teléfonos, etc.) son completamente ficticios. Los nombres y direcciones de los clubs están hardcodeados en el seeder.

## Flujo de pago

1. El usuario crea una reserva → estado `Pending`
2. El backend genera una sesión de Stripe Checkout y devuelve `checkoutUrl`
3. El frontend redirige al usuario a la URL de Stripe
4. Tras el pago, Stripe redirige al frontend con el `session_id`
5. El frontend llama a `POST /api/reservations/{id}/confirm-payment` con el `session_id`
6. El backend verifica el pago con Stripe y cambia el estado a `Confirmed`
7. Se envía un email de confirmación al usuario

Si el usuario cancela el pago en Stripe, la reserva pasa automáticamente a `Cancelled` y el slot queda libre. De momento no es posible reintentar el pago sobre una reserva existente — el usuario deberá crear una nueva reserva.

Solo las reservas en estado `Confirmed` bloquean el slot de la pista.

## Documentación de la API

Consulta la [Wiki del proyecto](../../wiki) para la documentación completa de todos los endpoints.

Swagger disponible en local en `/swagger` con la API en ejecución.

## Entornos

| Entorno | Archivo compose | Archivo env |
|---------|----------------|-------------|
| Development | `docker-compose.dev.yml` | `.env.dev` |
| Staging | `docker-compose.staging.yml` | `.env.staging` |
| Production | `docker-compose.production.yml` | `.env.production` |

El entorno de producción está desplegado en servidor. Se incluye el `docker-compose.production.yml` por si se desea ejecutar en local con configuración de producción.