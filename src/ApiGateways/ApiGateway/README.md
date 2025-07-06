# EShop API Gateway

## Overview / مرور کلی

This API Gateway serves as the single entry point for all client requests to the EShop microservices platform. It's built using ASP.NET Core 8.0 and Ocelot API Gateway framework.

این دروازه API به عنوان نقطه ورودی واحد برای تمام درخواست‌های کلاینت به پلتفرم میکروسرویس‌های EShop عمل می‌کند. این دروازه با استفاده از ASP.NET Core 8.0 و فریم‌ورک Ocelot API Gateway ساخته شده است.

## Features / ویژگی‌ها

- **Centralized Routing** - Single entry point for all microservices
- **JWT Authentication** - Secure token-based authentication
- **Request/Response Logging** - Comprehensive logging for monitoring
- **Load Balancing** - Round-robin load balancing for high availability
- **Rate Limiting** - Configurable rate limiting to prevent abuse
- **Circuit Breaker** - Fault tolerance with circuit breaker pattern
- **Swagger Integration** - Unified API documentation
- **CORS Support** - Cross-origin resource sharing configuration
- **Health Checks** - Health monitoring endpoints
- **Persian Language Support** - Bilingual error messages and documentation

## Architecture / معماری

```
Client Applications (Web UI, Mobile Apps, etc.)
                    ↓
            API Gateway (Port 5000)
                    ↓
    ┌─────────────────────────────────────────────────────────┐
    │                                                         │
    ▼               ▼               ▼               ▼         ▼
Identity.API    Product.API    Order.API      Payment.API  Notification.API
(Port 5001)     (Port 5002)   (Port 5003)    (Port 5004)   (Port 5005)
```

## Service Routing / مسیریابی سرویس‌ها

### Identity Service Routes
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `GET /api/auth/profile` - Get user profile
- `POST /api/auth/logout` - User logout
- `GET /api/user/{id}` - Get user details (Auth required)
- `PUT /api/user/{id}` - Update user profile (Auth required)

### Product Service Routes
- `GET /api/product` - Get products with filtering
- `GET /api/product/{id}` - Get specific product
- `POST /api/product` - Create product (Admin only)
- `PUT /api/product/{id}` - Update product (Admin only)
- `DELETE /api/product/{id}` - Delete product (Admin only)
- `GET /api/product/featured` - Get featured products
- `GET /api/product/search` - Search products
- `GET /api/category` - Get all categories
- `POST /api/category` - Create category (Admin only)

### Order Service Routes
- `GET /api/order` - Get orders (Auth required)
- `GET /api/order/{id}` - Get specific order (Auth required)
- `POST /api/order` - Create new order (Auth required)
- `PUT /api/order/{id}/status` - Update order status (Admin only)
- `GET /api/cart` - Get shopping cart (Auth required)
- `POST /api/cart/items` - Add item to cart (Auth required)
- `DELETE /api/cart/items/{id}` - Remove item from cart (Auth required)

### Payment Service Routes
- `POST /api/payment/process` - Process payment (Auth required)
- `GET /api/payment/{id}` - Get payment details (Auth required)
- `GET /api/payment/methods` - Get available payment methods

### Notification Service Routes
- `GET /api/notification` - Get notifications (Auth required)
- `POST /api/notification/email` - Send email notification (Auth required)
- `POST /api/notification/sms` - Send SMS notification (Auth required)

## Configuration / پیکربندی

### JWT Authentication Settings
```json
{
  "JwtSettings": {
    "Key": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong",
    "Issuer": "EShopIdentityAPI",
    "Audience": "EShopApp",
    "TokenExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 30
  }
}
```

### Rate Limiting Configuration
```json
{
  "RateLimitSettings": {
    "EnableRateLimiting": true,
    "DefaultRateLimit": 100,
    "DefaultTimeWindow": 60,
    "ClientIdHeader": "X-ClientId"
  }
}
```

### CORS Configuration
```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:5100",
      "http://localhost:5200"
    ],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
    "AllowedHeaders": ["Authorization", "Content-Type", "Accept"]
  }
}
```

## Development Setup / راه‌اندازی محیط توسعه

### Prerequisites / پیش‌نیازها
- .NET 8.0 SDK
- Docker (optional for containerized deployment)
- Visual Studio 2022 or VS Code

### Running Locally / اجرای محلی

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EShop/src/ApiGateways/ApiGateway
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update configuration**
   - Modify `appsettings.json` for your environment
   - Update JWT keys and connection strings

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the API Gateway**
   - Gateway: http://localhost:5000
   - Swagger: http://localhost:5000/swagger
   - Health: http://localhost:5000/health

### Docker Deployment / استقرار داکر

```bash
# Build the image
docker build -t eshop-api-gateway .

# Run the container
docker run -p 5000:80 eshop-api-gateway

# Or use docker-compose from the root directory
docker-compose up api-gateway
```

## API Documentation / مستندات API

### Swagger UI
The API Gateway provides unified Swagger documentation for all microservices:
- Gateway Swagger: `http://localhost:5000/swagger`
- Individual service documentation is aggregated automatically

### Authentication
Most endpoints require JWT authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

### Request/Response Format
All requests and responses use JSON format with UTF-8 encoding for Persian language support.

### Error Handling
Standard HTTP status codes are used:
- `200 OK` - Success
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `429 Too Many Requests` - Rate limit exceeded
- `500 Internal Server Error` - Server error

Error responses include both English and Persian messages:
```json
{
  "error": {
    "message": "Invalid input provided",
    "messageFa": "ورودی نامعتبر ارائه شده",
    "code": "INVALID_INPUT",
    "timestamp": "2024-01-01T12:00:00Z"
  }
}
```

## Monitoring & Observability / نظارت و رصد

### Health Checks
- Health endpoint: `/health`
- Detailed health: `/health/detailed` (in development)

### Logging
Structured logging is implemented with different log levels:
- `Information` - General information
- `Warning` - Potential issues
- `Error` - Error conditions
- `Debug` - Detailed debugging (development only)

### Metrics
The gateway collects various metrics:
- Request count and duration
- Success/failure rates
- Authentication attempts
- Rate limit violations

## Security / امنیت

### Authentication & Authorization
- JWT Bearer token authentication
- Role-based access control
- Token expiration and refresh mechanism

### Rate Limiting
- Per-client rate limiting
- Configurable limits and time windows
- Automatic client identification

### CORS Protection
- Configurable allowed origins
- Proper preflight handling
- Secure header policies

### Input Validation
- Request payload validation
- Parameter sanitization
- SQL injection prevention

## Troubleshooting / عیب‌یابی

### Common Issues

1. **Service Unavailable (503)**
   - Check if downstream services are running
   - Verify service discovery configuration
   - Check network connectivity

2. **Authentication Failures (401)**
   - Verify JWT token validity
   - Check token expiration
   - Ensure correct JWT configuration

3. **Rate Limit Exceeded (429)**
   - Check rate limit configuration
   - Verify client identification
   - Consider increasing limits if needed

4. **CORS Errors**
   - Check allowed origins configuration
   - Verify request headers
   - Ensure proper preflight handling

### Debugging
Enable detailed logging in development:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Ocelot": "Debug"
    }
  }
}
```

## Performance Optimization / بهینه‌سازی عملکرد

### Caching Strategies
- Response caching for static content
- Distributed caching for session data
- CDN integration for static assets

### Load Balancing
- Round-robin algorithm
- Health check integration
- Automatic failover

### Circuit Breaker
- Configurable failure thresholds
- Automatic recovery
- Fallback mechanisms

## Contributing / مشارکت

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License / مجوز

This project is licensed under the MIT License.

## Support / پشتیبانی

For support and questions:
- Create an issue on GitHub
- Email: support@eshop.com
- Documentation: [Link to full documentation]

---

**EShop API Gateway v1.0.0**
Built with ❤️ using ASP.NET Core 8.0 and Ocelot