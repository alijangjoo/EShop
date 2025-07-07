# Senior .NET Backend Developer Interview Questions & Answers

## Table of Contents
1. [Core .NET Framework & Runtime](#core-net-framework--runtime)
2. [C# Language Features](#c-language-features)
3. [Architecture & Design Patterns](#architecture--design-patterns)
4. [Entity Framework & Database](#entity-framework--database)
5. [Web API & REST Services](#web-api--rest-services)
6. [Performance & Optimization](#performance--optimization)
7. [Security](#security)
8. [Testing](#testing)
9. [Microservices & Distributed Systems](#microservices--distributed-systems)
10. [Azure & Cloud Technologies](#azure--cloud-technologies)
11. [DevOps & Deployment](#devops--deployment)

---

## Core .NET Framework & Runtime

### Q1: What's the difference between .NET Framework, .NET Core, and .NET 5+?

**Answer:**
- **.NET Framework**: Windows-only, legacy framework (4.x versions)
- **.NET Core**: Cross-platform, open-source rewrite (versions 1.0-3.1)
- **.NET 5+**: Unified platform combining .NET Framework and .NET Core, cross-platform

Key differences:
- **Performance**: .NET Core/5+ is significantly faster
- **Platform Support**: Core/5+ runs on Windows, Linux, macOS
- **Deployment**: Self-contained deployments possible with Core/5+
- **Side-by-side**: Multiple versions can coexist

### Q2: Explain the difference between managed and unmanaged code.

**Answer:**
- **Managed Code**: Runs under CLR supervision, garbage collected, type-safe
- **Unmanaged Code**: Runs directly on OS, manual memory management, no CLR

Benefits of managed code:
- Automatic memory management
- Type safety
- Exception handling
- Security sandboxing
- Cross-language interoperability

### Q3: What is the Global Assembly Cache (GAC) and when would you use it?

**Answer:**
GAC is a machine-wide code cache that stores assemblies specifically designated to be shared by several applications.

**Use cases:**
- Sharing assemblies across multiple applications
- Version management for shared libraries
- Security and integrity verification

**Note**: GAC is primarily for .NET Framework. .NET Core/5+ uses different deployment models.

---

## C# Language Features

### Q4: Explain the difference between `IEnumerable<T>`, `ICollection<T>`, and `IList<T>`.

**Answer:**
```csharp
// IEnumerable<T> - Basic iteration
public interface IEnumerable<T>
{
    IEnumerator<T> GetEnumerator();
}

// ICollection<T> - Add/Remove operations
public interface ICollection<T> : IEnumerable<T>
{
    int Count { get; }
    bool IsReadOnly { get; }
    void Add(T item);
    bool Remove(T item);
}

// IList<T> - Indexed access
public interface IList<T> : ICollection<T>
{
    T this[int index] { get; set; }
    int IndexOf(T item);
    void Insert(int index, T item);
}
```

**Usage:**
- Use `IEnumerable<T>` for read-only iteration
- Use `ICollection<T>` when you need to modify the collection
- Use `IList<T>` when you need indexed access

### Q5: What are delegates and how do they differ from events?

**Answer:**
```csharp
// Delegate - type-safe function pointer
public delegate void MyDelegate(string message);

public class Publisher
{
    // Event - special kind of delegate with access restrictions
    public event MyDelegate MyEvent;
    
    // Delegate field - full access
    public MyDelegate MyDelegateField;
    
    protected virtual void OnMyEvent(string message)
    {
        MyEvent?.Invoke(message);
    }
}
```

**Key Differences:**
- **Events**: Can only be raised by the declaring class, external classes can only subscribe/unsubscribe
- **Delegates**: Can be invoked directly by external classes
- **Encapsulation**: Events provide better encapsulation

### Q6: Explain async/await and when to use Task vs ValueTask.

**Answer:**
```csharp
// Task - reference type, heap allocated
public async Task<string> GetDataAsync()
{
    await Task.Delay(1000);
    return "data";
}

// ValueTask - value type, stack allocated when possible
public async ValueTask<string> GetCachedDataAsync()
{
    if (cache.ContainsKey("data"))
        return cache["data"]; // No heap allocation
    
    var result = await FetchDataAsync();
    cache["data"] = result;
    return result;
}
```

**Use ValueTask when:**
- Method might complete synchronously
- High-performance scenarios
- Hot paths in your application

**Use Task when:**
- Always completing asynchronously
- Default choice for most scenarios

---

## Architecture & Design Patterns

### Q7: Explain the SOLID principles with examples.

**Answer:**

**Single Responsibility Principle (SRP)**:
```csharp
// Bad - Multiple responsibilities
public class UserService
{
    public void CreateUser(User user) { /* ... */ }
    public void SendEmail(string email) { /* ... */ }
    public void LogActivity(string activity) { /* ... */ }
}

// Good - Single responsibility
public class UserService
{
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;
    
    public void CreateUser(User user) { /* ... */ }
}
```

**Open/Closed Principle (OCP)**:
```csharp
// Bad - Modification required for new shapes
public class AreaCalculator
{
    public double CalculateArea(object shape)
    {
        if (shape is Rectangle r) return r.Width * r.Height;
        if (shape is Circle c) return Math.PI * c.Radius * c.Radius;
        // Need to modify this method for new shapes
    }
}

// Good - Extension without modification
public interface IShape
{
    double CalculateArea();
}

public class Rectangle : IShape
{
    public double CalculateArea() => Width * Height;
}
```

### Q8: What is Dependency Injection and explain the different lifetimes in .NET Core?

**Answer:**
```csharp
// Service registration
services.AddTransient<ITransientService, TransientService>();
services.AddScoped<IScopedService, ScopedService>();
services.AddSingleton<ISingletonService, SingletonService>();
```

**Lifetimes:**
- **Transient**: New instance every time requested
- **Scoped**: One instance per HTTP request/scope
- **Singleton**: One instance for application lifetime

**Usage guidelines:**
- **Transient**: Lightweight, stateless services
- **Scoped**: Entity Framework contexts, HTTP request-specific services
- **Singleton**: Expensive-to-create services, caching services

### Q9: Explain the Repository and Unit of Work patterns.

**Answer:**
```csharp
// Repository pattern
public interface IRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

// Unit of Work pattern
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IOrderRepository Orders { get; }
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    
    public UnitOfWork(DbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Orders = new OrderRepository(_context);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
```

---

## Entity Framework & Database

### Q10: What's the difference between Code First and Database First approaches?

**Answer:**
**Code First:**
- Define models in C# classes
- Generate database from models
- Better for new projects
- Version control friendly

**Database First:**
- Start with existing database
- Generate models from database
- Better for existing databases
- Can regenerate models when database changes

### Q11: Explain different types of loading in Entity Framework.

**Answer:**
```csharp
// Eager Loading - Load related data upfront
var users = context.Users
    .Include(u => u.Orders)
    .ThenInclude(o => o.OrderItems)
    .ToList();

// Lazy Loading - Load on demand (requires virtual properties)
public class User
{
    public virtual ICollection<Order> Orders { get; set; }
}

// Explicit Loading - Manual control
var user = context.Users.First();
context.Entry(user)
    .Collection(u => u.Orders)
    .Load();
```

### Q12: How do you handle database migrations in Entity Framework?

**Answer:**
```bash
# Create migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

**Best practices:**
- Always review generated migrations
- Use meaningful migration names
- Test migrations on copy of production data
- Have rollback plan for production deployments

---

## Web API & REST Services

### Q13: Explain HTTP status codes and when to use them.

**Answer:**
- **200 OK**: Successful GET, PUT, PATCH
- **201 Created**: Successful POST with resource creation
- **204 No Content**: Successful DELETE or PUT with no response body
- **400 Bad Request**: Invalid request format/data
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Authenticated but not authorized
- **404 Not Found**: Resource doesn't exist
- **500 Internal Server Error**: Server error

### Q14: How do you implement API versioning?

**Answer:**
```csharp
// URL versioning
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public ActionResult GetUsersV1() { /* ... */ }
    
    [HttpGet]
    [MapToApiVersion("2.0")]
    public ActionResult GetUsersV2() { /* ... */ }
}

// Configuration
services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Version"));
});
```

### Q15: How do you implement global exception handling?

**Answer:**
```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new
        {
            message = exception.Message,
            statusCode = GetStatusCode(exception)
        };

        response.StatusCode = errorResponse.statusCode;
        await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}
```

---

## Performance & Optimization

### Q16: What are the key performance considerations in .NET applications?

**Answer:**
1. **Memory Management**:
   - Avoid memory leaks
   - Use `using` statements for disposable resources
   - Be careful with event handlers

2. **Async/Await**:
   - Use async for I/O operations
   - Avoid blocking async calls
   - Use `ConfigureAwait(false)` in libraries

3. **Database Optimization**:
   - Use proper indexing
   - Optimize queries
   - Use connection pooling
   - Consider read replicas

4. **Caching**:
   - Implement appropriate caching strategies
   - Use distributed caching for scalability

### Q17: Explain different caching strategies.

**Answer:**
```csharp
// Memory Cache
services.AddMemoryCache();

public class UserService
{
    private readonly IMemoryCache _cache;
    
    public async Task<User> GetUserAsync(int id)
    {
        var cacheKey = $"user_{id}";
        
        if (_cache.TryGetValue(cacheKey, out User user))
            return user;
            
        user = await _repository.GetUserAsync(id);
        _cache.Set(cacheKey, user, TimeSpan.FromMinutes(15));
        return user;
    }
}

// Distributed Cache (Redis)
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

**Cache Strategies:**
- **Cache-Aside**: Application manages cache
- **Write-Through**: Write to cache and database simultaneously
- **Write-Behind**: Write to cache first, database later

---

## Security

### Q18: How do you implement authentication and authorization in .NET Core?

**Answer:**
```csharp
// JWT Authentication
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = "your-issuer",
            ValidateAudience = true,
            ValidAudience = "your-audience"
        };
    });

// Authorization policies
services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireRole("Admin"));
    options.AddPolicy("RequireMinimumAge", policy =>
        policy.RequireClaim("age", "18", "19", "20"));
});

// Usage
[Authorize(Policy = "RequireAdminRole")]
public class AdminController : ControllerBase { }
```

### Q19: What are common security vulnerabilities and how to prevent them?

**Answer:**
1. **SQL Injection**:
   - Use parameterized queries
   - Use Entity Framework (built-in protection)

2. **Cross-Site Scripting (XSS)**:
   - Encode user input
   - Use Content Security Policy

3. **Cross-Site Request Forgery (CSRF)**:
   - Use anti-forgery tokens
   - Validate referrer headers

4. **Data Validation**:
   - Always validate input on server-side
   - Use data annotations
   - Implement custom validators

```csharp
public class CreateUserRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
```

---

## Testing

### Q20: Explain different types of testing and how to implement them.

**Answer:**
```csharp
// Unit Testing with xUnit and Moq
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly UserService _userService;
    
    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockRepository.Object);
    }
    
    [Fact]
    public async Task GetUserAsync_ValidId_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = 1, Name = "Test User" };
        _mockRepository.Setup(r => r.GetByIdAsync(1))
                      .ReturnsAsync(user);
        
        // Act
        var result = await _userService.GetUserAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test User", result.Name);
    }
}

// Integration Testing
public class UsersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public UsersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task GetUsers_ReturnsSuccessStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/users");
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
}
```

---

## Microservices & Distributed Systems

### Q21: What are the challenges of microservices architecture?

**Answer:**
**Challenges:**
1. **Distributed System Complexity**: Network latency, partial failures
2. **Data Consistency**: Eventual consistency, distributed transactions
3. **Service Discovery**: Dynamic service locations
4. **Monitoring**: Distributed tracing, centralized logging
5. **Deployment**: Orchestration, versioning

**Solutions:**
- **Circuit Breaker Pattern**: Handle service failures gracefully
- **API Gateway**: Single entry point, cross-cutting concerns
- **Service Mesh**: Infrastructure layer for service communication
- **Event Sourcing**: Capture state changes as events

### Q22: How do you handle distributed transactions?

**Answer:**
```csharp
// Saga Pattern - Choreography
public class OrderService
{
    public async Task ProcessOrderAsync(Order order)
    {
        // Step 1: Reserve inventory
        await _inventoryService.ReserveItemsAsync(order.Items);
        
        // Step 2: Process payment
        await _paymentService.ProcessPaymentAsync(order.Payment);
        
        // Step 3: Create order
        await _orderRepository.CreateAsync(order);
        
        // Publish event
        await _eventBus.PublishAsync(new OrderProcessedEvent(order.Id));
    }
}

// Compensating actions for failures
public async Task HandleInventoryReservationFailedAsync(InventoryReservationFailedEvent @event)
{
    // Compensate by releasing any reserved items
    await _inventoryService.ReleaseReservationAsync(@event.OrderId);
}
```

---

## Azure & Cloud Technologies

### Q23: What Azure services are commonly used for .NET applications?

**Answer:**
1. **App Service**: Web apps, APIs, background services
2. **Azure Functions**: Serverless compute
3. **SQL Database**: Managed SQL Server
4. **Cosmos DB**: NoSQL database
5. **Service Bus**: Messaging service
6. **Application Insights**: Monitoring and diagnostics
7. **Key Vault**: Secrets management
8. **Storage Account**: Blob, Table, Queue storage

### Q24: How do you implement health checks in .NET Core?

**Answer:**
```csharp
// Startup configuration
services.AddHealthChecks()
    .AddDbContext<ApplicationDbContext>()
    .AddSqlServer(connectionString)
    .AddRedis(redisConnectionString)
    .AddCheck<CustomHealthCheck>("custom-check");

// Custom health check
public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        // Custom logic
        var isHealthy = CheckExternalService();
        
        if (isHealthy)
            return Task.FromResult(HealthCheckResult.Healthy());
        else
            return Task.FromResult(HealthCheckResult.Unhealthy());
    }
}

// Endpoint configuration
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

---

## DevOps & Deployment

### Q25: How do you implement CI/CD for .NET applications?

**Answer:**
```yaml
# Azure DevOps Pipeline
trigger:
  branches:
    include:
      - main
      - develop

pool:
  vmImage: 'ubuntu-latest'

stages:
- stage: Build
  jobs:
  - job: Build
    steps:
    - task: DotNetCoreCLI@2
      displayName: 'Restore packages'
      inputs:
        command: 'restore'
        projects: '**/*.csproj'
    
    - task: DotNetCoreCLI@2
      displayName: 'Build solution'
      inputs:
        command: 'build'
        projects: '**/*.csproj'
        arguments: '--configuration Release'
    
    - task: DotNetCoreCLI@2
      displayName: 'Run tests'
      inputs:
        command: 'test'
        projects: '**/*Tests.csproj'
        arguments: '--configuration Release --collect:"XPlat Code Coverage"'

- stage: Deploy
  dependsOn: Build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: Deploy
    environment: 'Production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureWebApp@1
            inputs:
              azureSubscription: 'Azure-Connection'
              appName: 'my-web-app'
              package: '$(Pipeline.Workspace)/drop/**/*.zip'
```

---

## Behavioral Questions

### Q26: How do you handle technical debt in a project?

**Answer:**
1. **Identify**: Regular code reviews, static analysis tools
2. **Prioritize**: Impact on business, development velocity
3. **Plan**: Dedicate time in sprints for refactoring
4. **Communicate**: Explain business impact to stakeholders
5. **Prevent**: Coding standards, automated testing

### Q27: Describe a challenging technical problem you solved.

**Answer Structure:**
1. **Situation**: Describe the context and problem
2. **Task**: What needed to be accomplished
3. **Action**: Steps you took to solve it
4. **Result**: Outcome and lessons learned

Example: "We had a performance issue where our API was timing out under load. I identified the problem was N+1 queries in our Entity Framework code. I implemented eager loading and query optimization, reducing response time from 5 seconds to 500ms."

---

## Tips for Interview Success

1. **Prepare Code Examples**: Have real examples ready from your experience
2. **Practice Whiteboarding**: Be comfortable writing code without IDE
3. **Ask Clarifying Questions**: Don't assume requirements
4. **Think Aloud**: Explain your thought process
5. **Know Trade-offs**: Understand pros/cons of different approaches
6. **Stay Current**: Know latest .NET features and best practices
7. **System Design**: Be prepared for architecture discussions

---

*Good luck with your interview! Remember to be confident, ask questions, and demonstrate your problem-solving approach.*