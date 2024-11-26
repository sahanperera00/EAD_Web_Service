# 🌐 E-Commerce Backend - .NET Core APIs  

![Thumbnail](https://miro.medium.com/v2/resize:fit:1200/1*__L_1H85EYT8wfae9VuI-A.png) 

This repository contains the **E-Commerce Web Service**, developed as part of the Enterprise Application Development module in collaboration with **IFS Academy**. The backend service handles all business logic and data processing for the **web application** and **mobile application**, ensuring seamless functionality across the e-commerce system.

---

## 🌟 **Key Features**  
1. **User Management**  
   - Endpoints for creating, updating, and deleting users with roles: Administrator, Vendor, and Customer Service Representative (CSR).  
   - Role-based access control for secure operations.  

2. **Product Management**  
   - APIs to manage product listings (create, update, delete).  
   - Activate or deactivate product categories.  

3. **Order Management**  
   - Endpoints for order creation, status updates (processing, delivered), and cancellations.  
   - Notifications to customers and vendors for status changes.  

4. **Inventory Management**  
   - APIs to track inventory levels and notify vendors of low stock.  
   - Prevent removal of stock associated with pending orders.  

5. **Vendor Management**  
   - APIs to retrieve vendor ratings and customer feedback.  
   - Average vendor ranking calculated and stored.  

---

## 🛠️ **Technologies Used**  
- **Framework**: .NET Core  
- **Language**: C#  
- **Database**: NoSQL  
- **Environment**: IIS (Internet Information Services)  

---

## 📂 **Project Structure**  
```bash
EAD_Web_Service/
├── Controllers/             # API Controllers for handling HTTP requests  
├── Dtos/                    # Data Transfer Objects for request and response models  
├── Enums/                   # Enumerations for defining constants  
├── Models/                  # Entity models for database schema  
├── Properties/              # Project properties  
├── Services/                # Service layer for business logic  
├── Util/                    # Utility classes for helper functions  
├── appsettings.json         # Application configuration  
├── Program.cs               # Application entry point  
├── EAD_Web_Service.sln      # Solution file  
└── EAD_Web_Service.csproj   # Project file  
```

---

## 🚀 **Getting Started**  

### Prerequisites  
- .NET Core SDK installed on your machine.  
- A NoSQL database instance configured (e.g., MongoDB).  
- IIS configured for hosting the service.

### Steps to Run Locally  
1. Clone the repository:  
   ```bash
   git clone https://github.com/sahanperera00/EAD_Web_Service.git
   ```  
2. Navigate to the project directory:  
   ```bash
   cd EAD_Web_Service
   ```  
3. Restore dependencies:  
   ```bash
   dotnet restore
   ```  
4. Update the `appsettings.json` file with your database connection string.  
5. Run the application:  
   ```bash
   dotnet run
   ```  
6. The APIs will be available at `https://localhost:5001` or your configured IIS address.

---
---

## 🤝 **Team Members**  
- **[Abhishek Perera](https://github.com/AbishekPerera)**  
- **[Sahan Perera](https://github.com/sahanperera00)**  
- **[Janani Mayadunna](https://github.com/Janani-Mayadunna)**  
- **[Lochani Ranasinghe](https://github.com/LochaniRanasinghe)**  

---

## 🌐 **Acknowledgments**  
Special thanks to **IFS Academy** for their invaluable guidance and support throughout the project.

---
