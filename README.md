# real-estate-booking-be

REST API for the real estate booking platform. Handles property listings, reviews, visit bookings, and authentication. Built with ASP.NET Core and a clean layered architecture.

---

## Tech stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 (Web API) |
| Language | C# 12 |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL (via Npgsql) |
| Authentication | JWT Bearer tokens |
| Migrations | EF Core Migrations |

---

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL running locally or via Docker

### Setup

Clone the repository and restore dependencies:

```bash
dotnet restore
```

Update the connection string in `RealEstateBooking.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=real_estate_dev;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "real-estate-api",
    "Audience": "real-estate-client",
    "ExpiresInMinutes": 60
  }
}
```

Apply migrations and start the API:

```bash
dotnet ef database update --project RealEstateBooking.Infrastructure --startup-project RealEstateBooking.API
dotnet run --project RealEstateBooking.API
```

---

## Solution structure

The solution is split into four projects with a strict one-way dependency flow:

```
RealEstateBooking.sln
├── RealEstateBooking.API/             # Entry point — controllers, middleware, config
├── RealEstateBooking.Application/     # Business logic — services, interfaces, DTOs
├── RealEstateBooking.Domain/          # Core — entities, enums, domain rules
└── RealEstateBooking.Infrastructure/  # Data — EF Core, repositories, migrations
```


### Dependency flow
 
| Project | Depends on |
|---|---|
| `RealEstateBooking.API` | Application, Infrastructure |
| `RealEstateBooking.Application` | Domain |
| `RealEstateBooking.Infrastructure` | Application, Domain |
| `RealEstateBooking.Domain` | — |
 
- `Domain` has no dependencies on any other project
- `Application` depends only on `Domain`
- `Infrastructure` implements interfaces defined in `Application`
- `API` wires everything together via dependency injection
 
---

## Project breakdown

### Domain

Pure C# classes with no framework dependencies. This is where your entities and business rules live.

```
RealEstateBooking.Domain/
├── Entities/
│   ├── Property.cs
│   ├── Booking.cs
│   ├── Review.cs
│   └── User.cs
└── Enums/
    ├── BookingStatus.cs
    └── PropertyType.cs
```

Example entity:

```csharp
// Domain/Entities/Property.cs
public class Property
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public PropertyType Type { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
```

---

### Application

Business logic and contracts. Services, DTOs, and repository interfaces all live here. No EF Core, no HTTP — just pure logic.

```
RealEstateBooking.Application/
├── Interfaces/
│   ├── Repositories/
│   │   ├── IPropertyRepository.cs
│   │   ├── IBookingRepository.cs
│   │   └── IUserRepository.cs
│   └── Services/
│       ├── IPropertyService.cs
│       └── IAuthService.cs
├── Services/
│   ├── PropertyService.cs
│   └── AuthService.cs
└── DTOs/
    ├── Property/
    │   ├── PropertyDto.cs
    │   ├── CreatePropertyDto.cs
    │   └── UpdatePropertyDto.cs
    └── Auth/
        ├── LoginDto.cs
        └── TokenDto.cs
```

Example interface and service:

```csharp
// Application/Interfaces/Repositories/IPropertyRepository.cs
public interface IPropertyRepository
{
    Task<IEnumerable<Property>> GetAllAsync();
    Task<Property?> GetByIdAsync(Guid id);
    Task AddAsync(Property property);
    Task UpdateAsync(Property property);
    Task DeleteAsync(Guid id);
}
```

```csharp
// Application/Services/PropertyService.cs
public class PropertyService(IPropertyRepository repository) : IPropertyService
{
    public async Task<IEnumerable<PropertyDto>> GetAllAsync()
    {
        var properties = await repository.GetAllAsync();
        return properties.Select(p => new PropertyDto
        {
            Id = p.Id,
            Title = p.Title,
            Address = p.Address,
            Price = p.Price,
        });
    }
}
```

---

### Infrastructure

EF Core `DbContext`, repository implementations, and migrations. This is the only project that knows about Postgres.

```
RealEstateBooking.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   └── Configurations/
│       ├── PropertyConfiguration.cs
│       └── BookingConfiguration.cs
├── Repositories/
│   ├── PropertyRepository.cs
│   ├── BookingRepository.cs
│   └── UserRepository.cs
└── Migrations/
```

Example repository implementation:

```csharp
// Infrastructure/Repositories/PropertyRepository.cs
public class PropertyRepository(AppDbContext context) : IPropertyRepository
{
    public async Task<IEnumerable<Property>> GetAllAsync() =>
        await context.Properties.AsNoTracking().ToListAsync();

    public async Task<Property?> GetByIdAsync(Guid id) =>
        await context.Properties.FindAsync(id);

    public async Task AddAsync(Property property)
    {
        await context.Properties.AddAsync(property);
        await context.SaveChangesAsync();
    }
}
```

---

### API

Controllers, middleware, and the composition root (`Program.cs`). This is where DI is wired up and JWT is configured.

```
RealEstateBooking.API/
├── Controllers/
│   ├── PropertiesController.cs
│   ├── BookingsController.cs
│   └── AuthController.cs
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
```

Example controller:

```csharp
// API/Controllers/PropertiesController.cs
[ApiController]
[Route("api/[controller]")]
public class PropertiesController(IPropertyService propertyService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await propertyService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var property = await propertyService.GetByIdAsync(id);
        return property is null ? NotFound() : Ok(property);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreatePropertyDto dto)
    {
        var created = await propertyService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
```

`Program.cs` wires everything together:

```csharp
// API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Repositories
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

---

## Adding a new resource

Follow these steps every time you add a new resource (e.g. `Review`):

1. **Domain** — add the entity in `Domain/Entities/`
2. **Infrastructure** — add a `DbSet` to `AppDbContext`, add a configuration class, create a migration
3. **Application** — add the repository interface in `Application/Interfaces/Repositories/`, add DTOs, add the service interface and implementation
4. **Infrastructure** — implement the repository
5. **API** — register the repository and service in `Program.cs`, add the controller

```bash
# After updating AppDbContext and entity configurations, create a migration
dotnet ef migrations add AddReviewEntity \
  --project RealEstateBooking.Infrastructure \
  --startup-project RealEstateBooking.API

# Apply it
dotnet ef database update \
  --project RealEstateBooking.Infrastructure \
  --startup-project RealEstateBooking.API
```

---

## Branching strategy

This project uses two long-lived branches:

| Branch | Purpose |
|---|---|
| `dev` | Active development — all work goes here |
| `main` | Stable/production-ready code — never pushed to directly |

Always push your work to `dev`. `main` is updated only via merges from `dev` when a release is ready.

### Branch naming

When working on a feature or a fix, create a short-lived branch off `dev` following this pattern:

```
feat/PRO-123-short-description
fix/PRO-456-short-description
```

| Prefix | When to use | Example |
|---|---|---|
| `feat/` | Adding new functionality | `feat/PRO-123-property-search` |
| `fix/` | Fixing a bug | `fix/PRO-456-booking-date-validation` |

```bash
git checkout dev
git pull
git checkout -b feat/PRO-123-property-filters   # branch off dev
# ... do your work ...
git push origin feat/PRO-123-property-filters   # open a PR into dev when ready
```

Keep branch names lowercase and use hyphens between words.