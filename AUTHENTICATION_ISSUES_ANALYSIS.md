# Authentication Issues Analysis and Solutions

## Issues Identified

### 1. Admin.UI Authentication Issue (401 Errors)

**Root Cause:**
- Admin.UI service is calling `/api/auth/admin-login` endpoint but Identity.API only provides `/api/auth/login`
- Admin.UI has no authentication controller or login page - all controllers require `[Authorize]` attribute
- Missing admin authentication flow

**Evidence:**
- `AdminApiService.cs` line 724: calls `api/auth/admin-login` 
- `AuthController.cs` in Identity.API only has `api/auth/login` endpoint
- Admin.UI Controllers have `[Authorize]` attribute but no login mechanism

### 2. Web.UI Registration Issue

**Root Cause:**
- API endpoints are configured incorrectly in appsettings.json
- Identity API should be accessible via API Gateway but direct URLs are used
- Configuration mismatch between services

**Evidence:**
- Web.UI `appsettings.json` has `"IdentityApi": "https://localhost:7001/"` 
- Docker-compose shows Identity API on port 5001, API Gateway on port 5000
- Registration calls should go through API Gateway

## Configuration Issues

### Current Configuration Problems:
1. **Web.UI appsettings.json:**
   ```json
   "ApiSettings": {
     "BaseUrl": "https://localhost:7001/",
     "IdentityApi": "https://localhost:7001/"
   }
   ```

2. **Admin.UI appsettings.json:**
   ```json
   "ApiSettings": {
     "BaseUrl": "https://localhost:7001/",
     "IdentityApi": "https://localhost:7001/"
   }
   ```

3. **Docker-compose actual services:**
   - Identity API: port 5001
   - API Gateway: port 5000  
   - Web UI: port 5100
   - Admin UI: port 5200

## Solutions

### Solution 1: Add Admin Authentication Endpoint to Identity API

Add admin-specific login endpoint to `AuthController.cs`:

```csharp
[HttpPost("admin-login")]
public async Task<IActionResult> AdminLogin([FromBody] UserLoginDto model)
{
    try
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var userRoles = await _userManager.GetRolesAsync(user);
        if (!userRoles.Contains("Admin"))
            return Unauthorized("Access denied. Admin role required.");

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            var token = await _jwtService.GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(3),
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles.ToList()
            });
        }

        return Unauthorized("Invalid credentials");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred during admin login");
        return StatusCode(500, "Internal server error");
    }
}
```

### Solution 2: Add Admin Authentication Controller to Admin.UI

Create `AuthController.cs` in Admin.UI:

```csharp
[Route("Auth")]
public class AuthController : Controller
{
    private readonly IAdminApiService _adminApiService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAdminApiService adminApiService, ILogger<AuthController> logger)
    {
        _adminApiService = adminApiService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var token = await _adminApiService.LoginAsync(model);
            
            if (!string.IsNullOrEmpty(token))
            {
                HttpContext.Session.SetString("AdminAuthToken", token);
                
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                
                return RedirectToAction("Index", "Dashboard");
            }
            
            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin login");
            ModelState.AddModelError("", "Login error");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Remove("AdminAuthToken");
        return RedirectToAction("Login");
    }
}
```

### Solution 3: Fix API Configuration

Update both `appsettings.json` files to use correct API endpoints:

**Web.UI/appsettings.json:**
```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5000/",
    "IdentityApi": "http://localhost:5000/",
    "ProductApi": "http://localhost:5000/",
    "OrderApi": "http://localhost:5000/",
    "PaymentApi": "http://localhost:5000/"
  }
}
```

**Admin.UI/appsettings.json:**
```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5000/",
    "IdentityApi": "http://localhost:5000/",
    "ProductApi": "http://localhost:5000/",
    "OrderApi": "http://localhost:5000/",
    "PaymentApi": "http://localhost:5000/"
  },
  "IdentityServer": {
    "Authority": "http://localhost:5000/"
  }
}
```

### Solution 4: Update Admin.UI Program.cs

Fix the authentication configuration in `Program.cs`:

```csharp
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["ApiSettings:IdentityApi"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false
        };
    });
```

### Solution 5: Add Login View for Admin.UI

Create `Views/Auth/Login.cshtml`:
```html
@model LoginViewModel
@{
    ViewData["Title"] = "Admin Login";
}

<div class="row justify-content-center">
    <div class="col-md-6">
        <div class="card">
            <div class="card-header">
                <h3>Admin Login</h3>
            </div>
            <div class="card-body">
                <form method="post">
                    <div asp-validation-summary="All" class="text-danger"></div>
                    
                    <div class="form-group">
                        <label asp-for="Email"></label>
                        <input asp-for="Email" class="form-control" />
                        <span asp-validation-for="Email" class="text-danger"></span>
                    </div>
                    
                    <div class="form-group">
                        <label asp-for="Password"></label>
                        <input asp-for="Password" class="form-control" type="password" />
                        <span asp-validation-for="Password" class="text-danger"></span>
                    </div>
                    
                    <div class="form-group">
                        <input asp-for="RememberMe" class="form-check-input" />
                        <label asp-for="RememberMe" class="form-check-label"></label>
                    </div>
                    
                    <button type="submit" class="btn btn-primary">Login</button>
                </form>
            </div>
        </div>
    </div>
</div>
```

## Implementation Priority

1. **High Priority:** Fix Admin.UI authentication (Solutions 1, 2, 5)
2. **Medium Priority:** Fix API configuration (Solution 3)
3. **Low Priority:** Update authentication setup (Solution 4)

## Testing Steps

1. Start services with docker-compose
2. Access Admin.UI at http://localhost:5200
3. Should redirect to login page
4. Test admin login with valid credentials
5. Test Web.UI registration at http://localhost:5100
6. Verify API calls go through API Gateway

## Notes

- Admin role must exist in Identity database
- Create admin user in Identity seeding
- Consider implementing role-based authorization
- Add proper error handling and logging
- Test in containerized environment