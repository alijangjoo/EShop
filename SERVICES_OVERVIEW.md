# EShop Services Overview / مرور کلی سرویس‌های ای‌شاپ

## 🏗️ Microservices Architecture / معماری میکروسرویس

This document provides an overview of all microservices in the EShop platform.

### 1. Identity.API 🔐
**Purpose**: Authentication and Authorization / احراز هویت و مجوزها

**Features:**
- JWT Token-based authentication
- User registration and login
- Role-based authorization (Admin, Customer)
- Persian language support in user profiles
- Default admin user creation

**Endpoints:**
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `GET /api/auth/profile` - Get user profile
- `POST /api/auth/logout` - User logout

**Database**: EShopIdentityDb
- AspNetUsers, AspNetRoles, AspNetUserRoles tables

---

### 2. Product.API 📦
**Purpose**: Product and Category Management / مدیریت محصولات و دسته‌بندی

**Features:**
- Complete product catalog with Persian support
- Category hierarchy management
- Product images and attributes
- Advanced search and filtering
- Featured products and sales management
- Stock management

**Endpoints:**
- `GET /api/product` - Get products with filtering
- `GET /api/product/{id}` - Get specific product
- `POST /api/product` - Create product (Admin only)
- `PUT /api/product/{id}` - Update product (Admin only)
- `DELETE /api/product/{id}` - Delete product (Admin only)
- `GET /api/product/featured` - Get featured products
- `GET /api/product/on-sale` - Get products on sale
- `GET /api/product/search` - Search products

**Database**: EShopProductDb
- Products, Categories, ProductImages, ProductAttributes tables

---

### 3. Order.API 🛒
**Purpose**: Order Management / مدیریت سفارشات

**Features:**
- Complete order lifecycle management
- Persian language support for shipping addresses
- Order status tracking
- Payment method selection (IPG, Cash)
- Event publishing for order checkout
- Order filtering and search

**Endpoints:**
- `GET /api/order` - Get orders with filtering
- `GET /api/order/{id}` - Get specific order
- `POST /api/order` - Create new order
- `PUT /api/order/{id}/status` - Update order status (Admin only)
- `PUT /api/order/{id}/payment-status` - Update payment status (Admin only)
- `DELETE /api/order/{id}` - Cancel order (Admin only)

**Database**: EShopOrderDb
- Orders, OrderItems tables

**Events Published:**
- OrderCheckoutEvent (to Payment and Notification services)

---

### 4. Payment.API 💳
**Purpose**: Payment Processing / پردازش پرداخت

**Features:**
- Support for IPG (Internet Payment Gateway)
- Cash on Delivery support
- Payment status tracking
- Persian bank name support
- Transaction logging

**Database**: EShopPaymentDb
- Payments table

**Status**: Basic structure ready for implementation

---

### 5. Notification.API 📧
**Purpose**: Notifications and Communications / اطلاع‌رسانی و ارتباطات

**Features:**
- Email notifications
- SMS notifications (ready for Iranian SMS providers)
- Event-driven notifications
- Persian language templates

**Status**: Basic structure ready for implementation

---

### 6. EventBus.Messages 📨
**Purpose**: Shared Event Messages / پیام‌های رویداد مشترک

**Features:**
- Shared DTOs for inter-service communication
- MassTransit integration
- RabbitMQ message bus
- OrderCheckoutEvent implementation

---

### 7. ApiGateway 🌐
**Purpose**: API Gateway / دروازه API

**Features:**
- Ocelot-based API Gateway
- Request routing to microservices
- JWT authentication integration
- Rate limiting (ready for configuration)
- Load balancing (ready for configuration)

**Port**: 5000

---

### 8. Web.UI 🖥️
**Purpose**: Customer-facing Website / وب‌سایت مشتری

**Features:**
- ASP.NET Core MVC application
- RTL layout support
- Session-based shopping cart
- Persian language interface
- Responsive design

**Port**: 5100

---

### 9. Admin.UI 👨‍💼
**Purpose**: Admin Panel / پنل مدیریت

**Features:**
- ASP.NET Core MVC application
- Admin dashboard
- Product management interface
- Order management interface
- Customer management interface
- Persian language interface

**Port**: 5200

---

## 🔌 Integration Points / نقاط یکپارچگی

### Message Bus Communication
- **Publisher**: Order.API
- **Consumers**: Payment.API, Notification.API
- **Events**: OrderCheckoutEvent

### API Communication
- All services communicate through the API Gateway
- JWT tokens used for authentication across services
- HTTP clients used for synchronous communication

### Database Design
- Each service has its own database (Database per Service pattern)
- No direct database sharing between services
- Data consistency through eventual consistency patterns

---

## 🔧 Technology Stack / فناوری‌های استفاده شده

### Backend
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: JWT Bearer
- **Message Bus**: MassTransit + RabbitMQ
- **API Gateway**: Ocelot
- **Documentation**: Swagger/OpenAPI

### Frontend
- **Framework**: ASP.NET Core MVC
- **Styling**: Bootstrap 5 with RTL support
- **Languages**: Dual language (English/Persian)

### DevOps
- **Containerization**: Docker
- **Orchestration**: Docker Compose
- **Development**: Visual Studio 2022

---

## 🌍 Persian Language Support / پشتیبانی زبان فارسی

Every service includes comprehensive Persian language support:

### Database Level
- Dual language fields (English/Persian) in all entities
- Persian-specific fields for addresses, names, descriptions
- RTL-compatible data structures

### API Level
- Persian search capabilities
- Persian filtering and sorting
- Persian validation messages

### UI Level
- RTL layout support
- Persian fonts integration
- Persian number formatting
- Persian date handling

---

## 🚀 Deployment / استقرار

### Development Environment
```bash
# Run individual services
dotnet run --project src/Services/Identity/Identity.API
dotnet run --project src/Services/Product/Product.API
dotnet run --project src/Services/Order/Order.API

# Or use Docker Compose
docker-compose up --build
```

### Production Considerations
- Use Azure SQL Database or SQL Server cluster
- Configure Redis for distributed caching
- Set up Azure Service Bus or RabbitMQ cluster
- Configure SSL certificates
- Set up monitoring and logging (Application Insights)
- Configure auto-scaling for services

---

## 📊 Service Ports / پورت‌های سرویس

| Service | Port | Purpose |
|---------|------|---------|
| API Gateway | 5000 | Main entry point |
| Identity.API | 5001 | Authentication |
| Product.API | 5002 | Products |
| Order.API | 5003 | Orders |
| Payment.API | 5004 | Payments |
| Notification.API | 5005 | Notifications |
| Web.UI | 5100 | Customer website |
| Admin.UI | 5200 | Admin panel |
| RabbitMQ Management | 15672 | Message bus admin |
| SQL Server | 1433 | Database |

---

## 🔐 Security Considerations / ملاحظات امنیتی

### Authentication & Authorization
- JWT tokens with proper expiration
- Role-based access control
- Secure password policies
- Protection against common attacks

### Data Security
- SQL injection prevention
- Input validation and sanitization
- HTTPS enforcement
- Sensitive data encryption

### API Security
- Rate limiting
- CORS configuration
- API versioning
- Request/response logging

---

## 📈 Scalability & Performance / مقیاس‌پذیری و عملکرد

### Horizontal Scaling
- Each service can be scaled independently
- Database per service pattern allows for targeted scaling
- Message bus handles asynchronous communication

### Performance Optimization
- Database indexing on frequently queried fields
- Caching strategies for frequently accessed data
- Pagination for large datasets
- Lazy loading for related entities

### Monitoring
- Application logging in all services
- Health checks for each service
- Performance metrics collection
- Error tracking and alerting

---

## 🔄 Future Enhancements / بهبودهای آینده

### Short Term
- Complete Payment.API implementation with Iranian banks
- Complete Notification.API with SMS providers
- Add product reviews and ratings
- Implement shopping cart persistence

### Medium Term
- Add inventory management
- Implement discount and coupon system
- Add recommendation engine
- Mobile applications (iOS/Android)

### Long Term
- AI-powered search and recommendations
- Advanced analytics and reporting
- Multi-tenant support
- Kubernetes deployment