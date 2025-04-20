# 📦 E-Commerce Application - Full Stack on AWS

A serverless full-stack E-Commerce web app using **React (frontend)** and **.NET 8 (backend)**, deployed entirely on **AWS** infrastructure. It supports product management, cart and order functionality, image uploads, authentication, and CI/CD deployment.

---

## 🚀 Features

- 🧑 User Signup and Signin with JWT authentication
- 📦 Product management: Add and View Products
- 🛒 Cart operations: Add to Cart, View Cart
- 💳 Place Order with payment method and shipping
- 🖼️ Product image upload to S3
- 🔐 Secure API with JWT
- ☁️ Fully deployed using AWS: S3, CloudFront, API Gateway, Lambda, DynamoDB
- 🔄 CI/CD Pipeline integration
- 📣 SNS for notifications (planned)

---

## 🛠️ Tech Stack

### Backend (Serverless)
- **.NET 8 on AWS Lambda**
- **Amazon API Gateway**
- **Amazon DynamoDB** (Users, Products, Cart, Orders)
- **Amazon S3** (Product Images)
- **Amazon SNS** (Order Notifications)
- **JWT Token Auth**

### Frontend
- **React** with **Tailwind CSS**
- **Axios** for API communication
- Hosted on **Amazon S3** with **CloudFront CDN**

### DevOps
- **AWS CodePipeline** and **CodeBuild** for CI/CD

---

## 📁 Backend Structure

```
/EcommerceLambdaBackend
├── Models/                 # Data models (User, Product, Order, CartItem, etc.)
├── Handlers/               # Lambda function handlers
├── Services/               # JWT generation/validation, image upload, utilities
├── Utils/                  # Helper and reusable logic
├── EcommerceLambdaBackend.csproj
└── README.md
```

---

## 🔗 API Endpoints (Backend)

| Endpoint         | Method | Description                            |
| ---------------- | ------ | -------------------------------------- |
| `/signup`        | POST   | Register a new user                    |
| `/signin`        | POST   | Authenticate user and return JWT token |
| `/products`      | POST   | Add a new product (with image upload)  |
| `/products`      | GET    | Get all products                       |
| `/cart/add`      | POST   | Add item to user's cart                |
| `/cart/{userId}` | GET    | Get all items in user's cart           |
| `/order/place`   | POST   | Place an order                         |

> **Protected routes require**: `Authorization: Bearer <JWT-Token>`

---

## 🔐 JWT Payload Example

```json
{
  "sub": "user_123",
  "email": "user@example.com",
  "username": "john_doe",
  "iat": 1745090149,
  "exp": 1745097349
}
```

---

## 🧪 Sample API Requests & Responses

### ✉️ Signup
**POST** `/signup`

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

---

### 🔑 Signin
**POST** `/signin`

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

---

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

---

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

---

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

---

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

## 🌐 Frontend (React)

- Implemented with React + Tailwind CSS
- Fully responsive UI with product listing, auth pages, cart, and order
- Axios to connect with backend APIs
- Authentication token stored in `localStorage`
- Image loading from S3 URLs
- Hosted on **AWS S3**, delivered via **CloudFront**

---

## ⚙️ CI/CD Pipeline

- AWS CodePipeline connects GitHub repo with CodeBuild
- Automatically deploys frontend to S3 and backend via Lambda updates
- S3 website hosted through CloudFront with caching & HTTPS

---

## 📬 Contact

Feel free to reach out for questions or collaboration opportunities.

---

## 📝 License

MIT License

