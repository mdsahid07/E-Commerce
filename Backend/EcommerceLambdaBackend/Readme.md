# 📦 E-Commerce Backend - AWS Lambda with .NET Core

A serverless E-Commerce backend built with **.NET 8**, **AWS Lambda**, **DynamoDB**, **S3**, and **JWT authentication**. This backend supports product management, cart operations, and order placement through RESTful APIs exposed via API Gateway.

---

## 🚀 Features

- User Signup and Signin with JWT token generation
- Product creation and retrieval
- Cart: Add, View, and Clear items
- Place order and save to DynamoDB
- Upload product images to S3
- Authentication middleware for protected endpoints

---

## 🛠️ Technologies Used

- **.NET 8 Lambda (C#)**
- **Amazon API Gateway**
- **AWS Lambda**
- **Amazon DynamoDB**
- **Amazon S3**
- **JWT Authentication**
- **Amazon SNS** (for future use: order notifications)

---

## 📁 Project Structure

```
/EcommerceLambdaBackend
│
├── Models/                 # Data models (User, Product, Order, CartItem, etc.)
├── Handlers/               # Lambda function handlers
├── Services/               # JWT generation/validation, image upload, utilities
├── Utils/                  # Helper and reusable logic
├── EcommerceLambdaBackend.csproj
└── README.md
```

---

## 📦 API Endpoints

| Endpoint         | Method | Description                            |
| ---------------- | ------ | -------------------------------------- |
| `/signup`        | POST   | Register a new user                    |
| `/signin`        | POST   | Authenticate user and return JWT token |
| `/products`      | POST   | Add a new product (with image upload)  |
| `/products`      | GET    | Get all products                       |
| `/cart/add`      | POST   | Add item to user's cart                |
| `/cart/{userId}` | GET    | Get all items in user's cart           |
| `/order/place`   | POST   | Place an order                         |

> **Note**: JWT Bearer token must be sent in `Authorization` header for protected endpoints.

Example:

```
Authorization: Bearer <your-token>
```

---

## 🔐 JWT Authentication

JWT is used to protect endpoints. During `signin`, a token is generated. You must attach this token in subsequent requests to protected APIs.

### Token Payload (Claims)

Example decoded JWT payload:

```json
{
  "sub": "userId-from-db",
  "email": "user@example.com",
  "username": "john_doe",
  "iat": 1745090149,
  "exp": 1745097349
}
```

You can extract `email` from the token and fetch user details from DynamoDB.

---

## 💾 Sample DynamoDB JSON (Product)

```json
{
  "ProductId": "prod_001",
  "Name": "Wireless Mouse",
  "Price": 29.99,
  "Description": "Ergonomic wireless mouse with USB receiver",
  "Category": "Electronics",
  "ImageUrl": "https://your-bucket.s3.amazonaws.com/prod_001.jpg"
}
```

---

## 📝 Sample API Requests & Responses

### ✉️ Signup
**POST** `/signup`

**Request:**
```json
{
  "email": "john@example.com",
  "password": "P@ssword123",
  "username": "john_doe"
}
```
**Response:**
```json
{
  "message": "User registered successfully"
}
```

### 🔑 Signin
**POST** `/signin`

**Request:**
```json
{
  "email": "john@example.com",
  "password": "P@ssword123"
}
```
**Response:**
```json
{
  "token": "<JWT-Token>",
  "username": "john_doe"
}
```

### 📚 Get All Products
**GET** `/products`

**Response:**
```json
[
  {
    "productId": "prod_001",
    "name": "Wireless Mouse",
    "price": 29.99,
    "description": "Ergonomic wireless mouse",
    "category": "Electronics",
    "imageUrl": "https://your-bucket.s3.amazonaws.com/prod_001.jpg"
  }
]
```

### ➕ Add Product
**POST** `/products`

**Headers:**
```
Authorization: Bearer <JWT-Token>
```
**Form-Data:**
- name: Wireless Mouse
- price: 29.99
- description: Ergonomic wireless mouse
- category: Electronics
- image: [upload file]

**Response:**
```json
{
  "message": "Product created successfully",
  "productId": "prod_001"
}
```

### 🛒 Add To Cart
**POST** `/cart/add`

**Headers:**
```
Authorization: Bearer <JWT-Token>
```
**Request:**
```json
{
  "userId": "user_123",
  "productId": "prod_001",
  "quantity": 2,
  "price": 29.99
}
```
**Response:**
```json
{
  "message": "Item added to cart"
}
```

### 💳 Place Order
**POST** `/order/place`

**Headers:**
```
Authorization: Bearer <JWT-Token>
```
**Request:**
```json
{
  "userId": "user_123",
  "items": [
    { "productId": "prod_001", "quantity": 2, "price": 29.99 }
  ],
  "shippingAddress": "123 Main Street, NY",
  "paymentMethod": "credit_card"
}
```
**Response:**
```json
{
  "message": "Order placed successfully",
  "orderId": "order_456"
}
```

---

## 🧪 Local Testing

You can use **Amazon Lambda Test Tool** for local testing.

```bash
dotnet lambda test-tool-8.0
```

Use Postman or cURL to send requests to the local endpoint.

> For testing auth-protected routes, include the Authorization header:
```
Authorization: Bearer <token>
```

---

## 📸 Image Upload to S3

When adding a product, you can upload an image file. The backend stores the file to an S3 bucket and returns the image URL in the product record.

Ensure your IAM Role has `s3:PutObject` permission for the bucket.

---

## ✅ To-Do Features

- Email notifications on order placement (SNS)
- Admin panel for managing products
- Search and filter functionality
- Unit tests with xUnit

---

## 📬 Contact

If you have questions or need help, feel free to open an issue or contact me.

---

## 📝 License

This project is licensed under the MIT License.

