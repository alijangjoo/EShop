# Notification Service

## Overview
The Notification Service is a microservice responsible for sending email and SMS notifications in the EShop e-commerce platform. It supports Persian language content and handles various notification types including order confirmations, payment confirmations, and welcome messages.

## Features

### ✅ Email Notifications
- **Order Confirmation**: Sent when a customer places an order
- **Payment Confirmation**: Sent when payment is successfully processed
- **Payment Failure**: Sent when payment fails
- **Welcome Email**: Sent when a new user registers
- **Custom Email**: Generic email sending capability

### ✅ SMS Notifications
- **Order Confirmation SMS**: Brief order confirmation message
- **Payment Success SMS**: Payment confirmation message
- **Payment Failure SMS**: Payment failure notification
- **Welcome SMS**: Welcome message for new users
- **Custom SMS**: Generic SMS sending capability

### ✅ Persian Language Support
- **RTL Email Templates**: Right-to-left email templates with Persian content
- **Persian SMS Messages**: SMS messages in Persian
- **Persian Date Formatting**: Proper Persian date formatting in notifications
- **Persian Number Formatting**: Currency formatting in Persian style

### ✅ Event-Driven Architecture
- **Order Checkout Events**: Listens to order checkout events from Order Service
- **Payment Processed Events**: Listens to payment events from Payment Service
- **User Registration Events**: Listens to user registration events from Identity Service

## Configuration

### Email Settings (appsettings.json)
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "noreply@eshop.com",
    "FromName": "EShop - فروشگاه آنلاین"
  }
}
```

### SMS Settings (appsettings.json)
```json
{
  "SmsSettings": {
    "ApiKey": "your-sms-api-key",
    "Username": "your-sms-username",
    "Password": "your-sms-password",
    "From": "50004001500015",
    "BaseUrl": "https://rest.payamak-panel.com/api/SendSMS/"
  }
}
```

### Event Bus Settings (appsettings.json)
```json
{
  "EventBusSettings": {
    "HostAddress": "amqp://guest:guest@localhost:5672"
  }
}
```

## API Endpoints

### Email Endpoints

#### Send Email
```
POST /api/notification/email
Authorization: Bearer {token}
Content-Type: application/json

{
  "to": "user@example.com",
  "subject": "Test Email",
  "body": "<h1>Hello World</h1>",
  "isHtml": true,
  "toName": "John Doe"
}
```

#### Send Order Confirmation Email
```
POST /api/notification/order-confirmation/email
Authorization: Bearer {token} (Admin only)
Query Parameters:
- toEmail: Customer email
- toName: Customer name
- orderNumber: Order number
- totalAmount: Total amount
- paymentMethod: Payment method (IPG/Cash)
```

#### Send Payment Confirmation Email
```
POST /api/notification/payment-confirmation/email
Authorization: Bearer {token} (Admin only)
Query Parameters:
- toEmail: Customer email
- toName: Customer name
- orderNumber: Order number
- amount: Payment amount
- paymentMethod: Payment method (IPG/Cash)
```

### SMS Endpoints

#### Send SMS
```
POST /api/notification/sms
Authorization: Bearer {token}
Content-Type: application/json

{
  "to": "09123456789",
  "message": "Your SMS message"
}
```

#### Send Order Confirmation SMS
```
POST /api/notification/order-confirmation/sms
Authorization: Bearer {token} (Admin only)
Query Parameters:
- phoneNumber: Customer phone number
- orderNumber: Order number
- totalAmount: Total amount
```

#### Send Payment Confirmation SMS
```
POST /api/notification/payment-confirmation/sms
Authorization: Bearer {token} (Admin only)
Query Parameters:
- phoneNumber: Customer phone number
- orderNumber: Order number
- amount: Payment amount
```

### Health Check
```
GET /api/notification/health
```

## Event Consumers

### OrderCheckoutConsumer
- **Event**: `OrderCheckoutEvent`
- **Actions**: 
  - Sends order confirmation email
  - Sends order confirmation SMS
- **Triggered by**: Order Service when a new order is placed

### PaymentProcessedConsumer
- **Event**: `PaymentProcessedEvent`
- **Actions**: 
  - Sends payment confirmation email (if successful)
  - Sends payment failure email (if failed)
  - Sends corresponding SMS notifications
- **Triggered by**: Payment Service when payment is processed

### UserRegisteredConsumer
- **Event**: `UserRegisteredEvent`
- **Actions**: 
  - Sends welcome email
  - Sends welcome SMS (if phone number provided)
- **Triggered by**: Identity Service when a new user registers

## Email Templates

### Persian Email Templates
All email templates are designed with:
- **RTL Layout**: Right-to-left text direction
- **Persian Fonts**: Tahoma, Arial font stack
- **Persian Content**: All content in Persian language
- **Responsive Design**: Works on desktop and mobile
- **Professional Styling**: Clean, modern appearance

### Available Templates
1. **Order Confirmation Template**
2. **Payment Success Template**
3. **Payment Failure Template**
4. **Welcome Template**

## SMS Templates

### Persian SMS Templates
- **Character Limit**: Optimized for SMS character limits
- **Persian Content**: All messages in Persian
- **Concise Format**: Essential information only
- **Professional Tone**: Appropriate business communication

### Available SMS Templates
1. **Order Confirmation SMS**
2. **Payment Success SMS**
3. **Payment Failure SMS**
4. **Welcome SMS**

## Service Architecture

### Dependencies
- **MailKit**: For email sending
- **MimeKit**: For email message composition
- **MassTransit**: For event handling
- **RabbitMQ**: For message queuing
- **ASP.NET Core**: For API framework
- **JWT Authentication**: For API security

### Services
- **EmailService**: Handles email sending logic
- **SmsService**: Handles SMS sending logic
- **TemplateService**: Manages notification templates
- **Event Consumers**: Handle integration events

### Error Handling
- **Comprehensive Logging**: All operations are logged
- **Graceful Failures**: Service continues if notifications fail
- **Retry Logic**: Built-in retry mechanisms
- **Error Reporting**: Detailed error messages in responses

## Development Setup

### Prerequisites
- .NET 8.0 SDK
- RabbitMQ server
- SMTP server access (for email)
- SMS service API access (for SMS)

### Configuration Steps
1. Update email settings in `appsettings.json`
2. Update SMS settings in `appsettings.json`
3. Configure RabbitMQ connection string
4. Set up JWT authentication settings

### Running the Service
```bash
cd src/Services/Notification/Notification.API
dotnet run
```

The service will be available at `http://localhost:5005`

## Docker Support

### Build Docker Image
```bash
docker build -t notification-api .
```

### Run with Docker Compose
```bash
docker-compose up notification-api
```

## Security

### Authentication
- **JWT Bearer Token**: Required for all API endpoints
- **Role-Based Access**: Admin-only endpoints for certain operations
- **CORS Support**: Configured for cross-origin requests

### Data Protection
- **Secure Communication**: HTTPS support
- **Input Validation**: All inputs are validated
- **Error Handling**: Sensitive information is not exposed

## Monitoring

### Health Checks
- **Health endpoint**: `/api/notification/health`
- **Service status**: Returns current service health

### Logging
- **Structured Logging**: JSON-formatted logs
- **Log Levels**: Configurable log levels
- **Error Tracking**: Detailed error information

## Integration

### Event Bus Integration
The service integrates with the event bus to receive notifications about:
- New orders
- Payment status changes
- User registrations

### Service Dependencies
- **Identity Service**: For user authentication
- **Order Service**: For order-related events
- **Payment Service**: For payment-related events
- **RabbitMQ**: For event messaging

## Future Enhancements

### Planned Features
- **Push Notifications**: Mobile app notifications
- **Email Templates Management**: Dynamic template editing
- **Advanced Analytics**: Notification delivery statistics
- **Multi-language Support**: Additional language support
- **Notification Preferences**: User-configurable preferences

### Technical Improvements
- **Database Integration**: Store notification history
- **Caching**: Template and configuration caching
- **Rate Limiting**: Prevent abuse
- **Batch Processing**: Bulk notification sending

## Support

For questions or issues with the Notification Service, please refer to the main project documentation or contact the development team.

## License

This project is licensed under the MIT License - see the main project LICENSE file for details.