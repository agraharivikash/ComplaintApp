# ComplaintApp

A **Complaint & Issue Tracking Portal** built using **ASP.NET Core (.NET 10)** and **Razor MVC**.  
This application allows users to register complaints, track their status, add comments, and enables administrators to manage and resolve complaints through a dedicated dashboard.

---

## 📌 Project Overview

The ComplaintApp is designed to simplify complaint management by providing a centralized platform where:

- Users can raise complaints and track their progress
- Administrators can manage, update, and resolve complaints
- All interactions are secured using role-based authentication

This project is suitable for **academic submission**, **portfolio showcase**, and **learning ASP.NET Core MVC with Entity Framework Core**.

---

## 🚀 Features

- User registration and login
- Role-based access (**User / Admin**)
- Complaint creation and tracking
- Complaint status lifecycle:  
  **Open → InProgress → Resolved**
- Commenting system on complaints
- Admin dashboard for complaint management
- Search and filter complaints
- Clean Razor UI with Bootstrap styling

---

## 🛠️ Tech Stack

| Layer | Technology |
|------|-----------|
| Backend | ASP.NET Core (.NET 10) |
| Frontend | Razor MVC |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Authentication | Cookie-based authentication |
| UI | Bootstrap 5, Font Awesome |

---

## 🧭 System Architecture

The project follows a **Layered MVC Architecture**:

Razor Views (UI)
↓
Controllers (Business Logic)
↓
Entity Framework Core (DbContext)
↓
SQL Server Database


---

## 🔐 Roles & Permissions

### Guest
- View public pages only

### User
- Register and login
- Create complaints
- View own complaints
- Add comments to own complaints

### Admin
- View all complaints
- Update complaint status
- View and add comments
- Manage the system via dashboard

---

## 🔄 Complaint Workflow

1. User registers and logs in
2. User creates a complaint
3. Complaint status starts as **Open**
4. Admin reviews and updates status
5. User and Admin add comments
6. Complaint is marked **Resolved**

---

## 🌐 Endpoint Mapping (Quick Reference)

| Method | URL | Controller Action |
|------|-----|------------------|
| GET | `/` | Home |
| GET | `/account/register` | Register |
| POST | `/account/register` | Register |
| GET | `/account/login` | Login |
| POST | `/account/login` | Login |
| GET | `/complaints` | Index |
| GET | `/complaints/create` | Create |
| POST | `/complaints/create` | Create |
| GET | `/complaints/details/{id}` | Details |
| POST | `/complaints/addcomment` | AddComment |
| GET | `/admin/dashboard` | Dashboard |
| POST | `/admin/updatestatus` | UpdateStatus |

---

## 🗂️ Database Design

The database schema includes the following tables:

- **Users**
- **People**
- **Complaints**
- **Comments**

Relationships:
- One User → Many Complaints
- One Complaint → Many Comments
- One User → Many Comments

Entity Framework Core migrations are used to manage schema changes.

---

## ▶️ Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server
- Visual Studio 2022+

### Setup Steps

1. Clone the repository
   ```bash
   git clone https://github.com/agraharivikash/ComplaintApp.git
Open the solution in Visual Studio

Configure the SQL Server connection string in appsettings.json

Apply database migrations

Update-Database
Run the application

dotnet run
🔒 Security Notes
Admin role selection during registration is enabled for development purposes only

Recommended for production:

Remove role selection from public registration

Seed admin via migration

Use ASP.NET Identity for password hashing

🚧 Limitations
No email or SMS notifications

No file attachment support

Manual role assignment

No audit logging

🔮 Future Enhancements
ASP.NET Identity integration

Email notifications

File uploads for complaints

REST API + SPA frontend (Angular/React)

CI/CD pipeline integration

📄 License
This project is open-source and intended for learning and academic purposes.
