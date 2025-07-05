# EShop - Persian E-commerce Platform / پلتفرم تجارت الکترونیک فارسی

## 📋 Overview / مرور کلی

This is a comprehensive e-commerce platform built with ASP.NET Core microservices architecture, featuring Persian language support, RTL layout, and modern payment integration including IPG and Cash payment methods.

این یک پلتفرم جامع تجارت الکترونیک است که با معماری میکروسرویس ASP.NET Core ساخته شده و دارای پشتیبانی از زبان فارسی، طراحی RTL و تکامل پرداخت مدرن شامل روش‌های پرداخت IPG و نقدی است.

## 🏗️ Architecture / معماری

### Microservices / میکروسرویس‌ها

1. **Identity.API** - Authentication & Authorization / احراز هویت و مجوزها
2. **Product.API** - Product Management / مدیریت محصولات
3. **Order.API** - Order Management / مدیریت سفارشات
4. **Payment.API** - Payment Processing / پردازش پرداخت
5. **Notification.API** - Notifications / اطلاع‌رسانی
6. **ApiGateway** - API Gateway / دروازه API

### Frontend Applications / اپلیکیشن‌های Frontend

1. **Web.UI** - Customer-facing website / وب‌سایت مشتری
2. **Admin.UI** - Admin panel / پنل مدیریت

### Building Blocks / بلوک‌های ساختاری

1. **EventBus.Messages** - Shared event messages / پیام‌های رویداد مشترک

## 🛠️ Technology Stack / فناوری‌های استفاده شده

- **Backend**: ASP.NET Core 8.0, Entity Framework Core
- **Database**: SQL Server
- **Authentication**: JWT Bearer
- **Message Bus**: MassTransit + RabbitMQ
- **API Documentation**: Swagger/OpenAPI
- **Mapping**: AutoMapper
- **Validation**: FluentValidation
- **Frontend**: ASP.NET Core MVC with RTL support
- **Styling**: Bootstrap with RTL customizations

## 🚀 Features / ویژگی‌ها

### Core Features / ویژگی‌های اصلی

- ✅ **Microservices Architecture** / معماری میکروسرویس
- ✅ **Persian Language Support** / پشتیبانی زبان فارسی
- ✅ **RTL Layout** / طراحی راست به چپ
- ✅ **JWT Authentication** / احراز هویت JWT
- ✅ **Role-based Authorization** / مجوزهای مبتنی بر نقش
- ✅ **Product Management** / مدیریت محصولات
- ✅ **Category Management** / مدیریت دسته‌بندی
- ✅ **Shopping Cart** / سبد خرید
- ✅ **Order Management** / مدیریت سفارشات
- ✅ **Payment Integration** / یکپارچگی پرداخت
- ✅ **Admin Panel** / پنل مدیریت
- ✅ **Customer Portal** / پورتال مشتری
- ✅ **Search & Filtering** / جستجو و فیلترگذاری
- ✅ **Responsive Design** / طراحی ریسپانسیو

### Payment Methods / روش‌های پرداخت

- **IPG (Internet Payment Gateway)** / درگاه پرداخت اینترنتی
- **Cash on Delivery** / پرداخت نقدی هنگام تحویل

### Persian-specific Features / ویژگی‌های مخصوص فارسی

- **Dual Language Support** / پشتیبانی دو زبانه
- **Persian Calendar** / تقویم فارسی
- **Persian Numbers** / اعداد فارسی
- **RTL Text Direction** / جهت متن راست به چپ
- **Persian Search** / جستجو فارسی

## 📁 Project Structure / ساختار پروژه

```
EShop/
├── src/
│   ├── Services/
│   │   ├── Identity/
│   │   │   └── Identity.API/
│   │   ├── Product/
│   │   │   └── Product.API/
│   │   ├── Order/
│   │   │   └── Order.API/
│   │   ├── Payment/
│   │   │   └── Payment.API/
│   │   └── Notification/
│   │       └── Notification.API/
│   ├── ApiGateways/
│   │   └── ApiGateway/
│   ├── Web/
│   │   ├── Web.UI/
│   │   └── Admin.UI/
│   └── BuildingBlocks/
│       └── EventBus.Messages/
├── EShop.sln
└── README.md
```

## 🔧 Setup Instructions / راهنمای نصب

### Prerequisites / پیش‌نیازها

1. **.NET 8.0 SDK** or later
2. **SQL Server** (LocalDB or full instance)
3. **Visual Studio 2022** or **Visual Studio Code**
4. **Git**

### Installation Steps / مراحل نصب

1. **Clone the repository / کلون کردن مخزن**
   ```bash
   git clone <repository-url>
   cd EShop
   ```

2. **Restore NuGet packages / بازیابی بسته‌های NuGet**
   ```bash
   dotnet restore
   ```

3. **Update connection strings / به‌روزرسانی رشته‌های اتصال**
   
   Update the connection strings in `appsettings.json` files for each service:
   
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=EShopIdentityDb;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

4. **Run database migrations / اجرای migrations پایگاه داده**
   
   For each service with a database:
   ```bash
   cd src/Services/Identity/Identity.API
   dotnet ef database update
   
   cd ../../../Product/Product.API
   dotnet ef database update
   ```

5. **Start the services / شروع سرویس‌ها**
   
   You can start each service individually:
   ```bash
   # Identity Service
   cd src/Services/Identity/Identity.API
   dotnet run
   
   # Product Service
   cd src/Services/Product/Product.API
   dotnet run
   
   # Continue for other services...
   ```

   Or use Visual Studio to run multiple projects simultaneously.

## 🔐 Default Credentials / اعتبارهای پیش‌فرض

### Admin User / کاربر مدیر
- **Email**: admin@eshop.com
- **Password**: Admin123!

## 🌐 API Endpoints / نقاط پایانی API

### Identity Service (Port: 5001)
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `GET /api/auth/profile` - Get user profile
- `POST /api/auth/logout` - User logout

### Product Service (Port: 5002)
- `GET /api/product` - Get products with filtering
- `GET /api/product/{id}` - Get specific product
- `POST /api/product` - Create product (Admin only)
- `PUT /api/product/{id}` - Update product (Admin only)
- `DELETE /api/product/{id}` - Delete product (Admin only)
- `GET /api/product/featured` - Get featured products
- `GET /api/product/on-sale` - Get products on sale
- `GET /api/product/search` - Search products

### Category Service (Product.API)
- `GET /api/category` - Get all categories
- `GET /api/category/{id}` - Get specific category
- `POST /api/category` - Create category (Admin only)
- `PUT /api/category/{id}` - Update category (Admin only)
- `DELETE /api/category/{id}` - Delete category (Admin only)

## 📊 Database Schema / طرح پایگاه داده

### Identity Database
- `AspNetUsers` - User information
- `AspNetRoles` - User roles
- `AspNetUserRoles` - User-role relationships

### Product Database
- `Products` - Product information
- `Categories` - Product categories
- `ProductImages` - Product images
- `ProductAttributes` - Product attributes

## 🎨 Persian/RTL Styling / استایل فارسی/RTL

The platform includes comprehensive RTL support:

- **Bootstrap RTL** integration
- **Persian fonts** (Vazir, Sahel)
- **Right-to-left** text direction
- **Persian number** formatting
- **Persian date** picker
- **RTL form** layouts
- **Mirror layouts** for navigation

## 💳 Payment Integration / یکپارچگی پرداخت

### IPG Payment / پرداخت درگاه
- Support for popular Iranian payment gateways
- Secure payment processing
- Payment status tracking
- Automatic payment verification

### Cash on Delivery / پرداخت نقدی
- Cash payment option
- Order confirmation
- Delivery tracking
- Payment upon delivery

## 📱 Responsive Design / طراحی ریسپانسیو

The platform is fully responsive and works on:
- **Desktop computers** / کامپیوتر رومیزی
- **Tablets** / تبلت
- **Mobile phones** / گوشی همراه

## 🔍 Search & Filtering / جستجو و فیلترگذاری

Advanced search capabilities including:
- **Text search** in Persian and English
- **Category filtering** / فیلتر دسته‌بندی
- **Price range filtering** / فیلتر محدوده قیمت
- **Brand filtering** / فیلتر برند
- **Feature filtering** / فیلتر ویژگی
- **Sorting options** / گزینه‌های مرتب‌سازی

## 🛡️ Security Features / ویژگی‌های امنیتی

- **JWT Authentication** / احراز هویت JWT
- **Role-based access control** / کنترل دسترسی مبتنی بر نقش
- **Password hashing** / رمزگذاری گذرواژه
- **HTTPS encryption** / رمزگذاری HTTPS
- **Input validation** / اعتبارسنجی ورودی
- **SQL injection prevention** / جلوگیری از SQL injection

## 🚀 Performance Optimization / بهینه‌سازی عملکرد

- **Database indexing** / ایندکس‌گذاری پایگاه داده
- **Caching strategies** / استراتژی‌های کش
- **Lazy loading** / بارگذاری تنبل
- **Pagination** / صفحه‌بندی
- **Image optimization** / بهینه‌سازی تصاویر
- **CDN integration** / یکپارچگی CDN

## 📈 Monitoring & Logging / نظارت و ثبت وقایع

- **Application logging** / ثبت وقایع برنامه
- **Error tracking** / ردیابی خطاها
- **Performance monitoring** / نظارت عملکرد
- **API analytics** / تحلیل API

## 🤝 Contributing / مشارکت

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License / مجوز

This project is licensed under the MIT License.

## 📞 Support / پشتیبانی

For support and questions:
- Create an issue on GitHub
- Email: support@eshop.com

## 🔄 Version History / تاریخچه نسخه

- **v1.0.0** - Initial release with core features
- **v1.1.0** - Added Persian language support
- **v1.2.0** - Added payment integration
- **v1.3.0** - Added admin panel

---

**Happy Coding! / کدنویسی خوش!** 🎉
