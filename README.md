```diff
--- a/README.md
+++ b/README.md
@@ -0,0 +1,151 @@
+# <img src="https://dotnet.microsoft.com/static/images/redesign/dotnet-logo.svg" alt="dotnet logo" width="50"/> E-Commerce API Project
+
+[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
+[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
+[![SQLite](https://img.shields.io/badge/SQLite-07405E?style=for-the-badge&logo=sqlite&logoColor=white)](https://www.sqlite.org/index.html)
+[![Redis](https://img.shields.io/badge/redis-%23DD0031.svg?style=for-the-badge&logo=redis&logoColor=white)](https://redis.io/)
+[![Ocelot](https://img.shields.io/badge/Ocelot-.NET%20API%20Gateway-blue?style=for-the-badge)](https://ocelot.readthedocs.io/en/latest/)
+
+
+
+Welcome to the E-Commerce API Project! This repository contains the source code for a scalable and microservice-based E-Commerce API built using .NET 8, C#, and various cutting-edge technologies.
+
+## Project Description
+
+This project is designed to create a robust and efficient backend for an online store. It's built using a microservices architecture, which breaks down the application into a collection of smaller, independent services. This makes the application more scalable, maintainable, and flexible.
+
+The key components of this project are:
+
+*   **Product API:** Manages product catalog, including categories, products, and images.
+*   **Cart API:** Handles shopping cart functionality, allowing users to add, remove, and update items in their cart. The cart uses Redis as cache.
+*   **Order API:** Processes orders, manages order history, and updates order statuses.
+*   **Payment API:** Manages the payment logic
+*   **Gateway API:** Acts as an API Gateway, routing requests to the appropriate microservices and providing a unified entry point. The project uses Ocelot.
+
+
+## 🛠️ Setup Instructions
+
+Follow these steps to set up and run the E-Commerce API project:
+
+1.  **Prerequisites:**
+    *   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed on your machine.
+    *   [Docker](https://www.docker.com/get-started/) installed for containerization.
+    *   [Redis](https://redis.io/docs/install/install-docker/) installed.
+    * A REST client to be able to make requests (like postman)
+
+2.  **Clone the Repository:**
+
+    ```bash
+    git clone <repository-url>
+    cd <repository-directory>
+    ```
+
+3.  **Restore NuGet Packages:**
+    ```bash
+    dotnet restore
+    ```
+
+4. **Docker services**
+
+    ```bash
+    cd Cart.API/
+    docker compose up -d
+    ```
+
+5.  **Build the Project:**
+
+    ```bash
+    dotnet build
+    ```
+
+6.  **Run the API Services:**
+    ```bash
+    # Run each API service in separate terminals
+    cd Product.API
+    dotnet run
+
+    cd Cart.API
+    dotnet run
+
+    cd Order.API
+    dotnet run
+
+    cd Payment.API
+    dotnet run
+
+    cd Gateway.API
+    dotnet run
+
+    ```
+
+## 🚀 Usage Examples
+
+Here are some examples of how to use the API:
+
+### Product API
+
+*   **Get all products:** `GET /products`
+*   **Get product by ID:** `GET /products/{id}`
+*   **Create a product:** `POST /products`
+*   **Get all categories** `GET /categories`
+
+### Cart API
+
+*   **Get user cart:** `GET /cart/{userId}`
+*   **Add product to cart:** `POST /cart`
+*   **Remove product from cart:** `DELETE /cart/{cartId}`
+*   **Update product qty:** `PUT /cart`
+
+### Order API
+
+*   **Create order:** `POST /orders`
+*   **Get order by ID:** `GET /orders/{id}`
+*   **Get order by user ID:** `GET /orders/user/{userId}`
+
+### Payment API
+
+*   **Get payment:** `GET /payment`
+*   **Create payment:** `POST /payment`
+
+You can use Postman collection and environment variables to easily test the different endpoints.
+
+## 📄 API Documentation
+
+Each API service provides Swagger/OpenAPI documentation:
+
+*   **Product API:** `https://localhost:<port>/swagger/index.html`
+*   **Cart API:** `https://localhost:<port>/swagger/index.html`
+*   **Order API:** `https://localhost:<port>/swagger/index.html`
+*   **Payment API:** `https://localhost:<port>/swagger/index.html`
+*   **Gateway API:** `https://localhost:<port>/swagger/index.html`
+
+*Note:* Replace `<port>` with the actual port number on which each API is running.
+
+## 🖼️ Relevant Images
+
+Here are some of the product images that are used in the Product API to illustrate the products:
+
+*   ![Product Image 1](Product.API/wwwroot/images/products/6e82a59c-1811-4d51-8ad0-d658aa71aca2.jpeg)
+*   ![Product Image 2](Product.API/wwwroot/images/products/8d61247f-49f0-4b12-b5ff-f891dea25f75.jpeg)
+*   ![Product Image 3](Product.API/wwwroot/images/products/cd4a6906-8ebb-46c0-aed3-caa36ab3ecce.jpeg)
+*   ![Product Image 4](Product.API/wwwroot/images/products/d1bb99cd-04d9-4a9a-8170-cb1a6a37a4c9.jpeg)
+
+## 🤝 Contribution Guidelines
+
+We welcome contributions from the community! To contribute to this project, please follow these steps:
+
+1.  **Fork the repository.**
+2.  **Create a new branch** for your feature or bug fix.
+3.  **Make your changes** and ensure they adhere to the project's coding standards.
+4.  **Test your changes** thoroughly.
+5.  **Submit a pull request** with a clear description of your changes.
+
+
+## 📜 License
+
+This project is licensed under the [MIT License](LICENSE).
+
+## Contact
+
+For any questions or suggestions, feel free to open an issue in this repository.
+
+---
+
+Thank you for your interest in the E-Commerce API Project!
+
+

```