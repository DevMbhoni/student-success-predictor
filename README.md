# 🎓 Student Success Predictor

An intelligent full-stack web platform that helps universities identify academically at-risk students early and recommend targeted interventions before failure or dropout occurs.

> Built as a graduate portfolio project demonstrating full-stack software engineering, machine learning, and data analytics skills.

---

## 🌐 Live Demo

| Service | URL |
|---|---|
| **Frontend** | https://student-success-predictor-beige.vercel.app |
| **.NET API** |https://student-success-api-5l9o.onrender.com/swagger |
| **Python ML Service** | https://student-success-ml.onrender.com/api/health |

> ⚠️ Hosted on Render's free tier — the API may take ~30 seconds to wake up after inactivity.

**Demo credentials:**

| Role | Email | Password |
|---|---|---|
| Administrator | admin@university.ac.za | Admin@1234 |
| Academic Advisor | advisor@university.ac.za | Advisor@1234 |
| Lecturer | j.mokoena@university.ac.za | Lecturer@1234 |
| Student | lethabo.nkosi@student.ac.za | Student@1234 |

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [System Architecture](#system-architecture)
- [Machine Learning Models](#machine-learning-models)
- [User Roles & Dashboards](#user-roles--dashboards)
- [API Documentation](#api-documentation)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Deployment](#deployment)

---

## Overview

Universities lose students every year due to poor academic performance, low attendance, and weak engagement — often intervening only after failure has already occurred. 

**Student Success Predictor** solves this by:

- Predicting **pass/fail probability** per student per module
- Classifying **risk level** (Low / Medium / High)
- Generating **explainable intervention recommendations** based on the weakest performance areas
- Providing role-specific dashboards for Students, Lecturers, Academic Advisors, and Administrators

---

## Features

- 🔐 **JWT Authentication** with role-based access control
- 📊 **Student Dashboard** — personal performance charts, pass probability gauge, recommendations
- 👨‍🏫 **Lecturer Dashboard** — per-module at-risk analysis, one-click predictions, class performance charts
- 🎓 **Academic Advisor Dashboard** — searchable student list, intervention detail panel, programme risk breakdown
- 📈 **Admin Dashboard** — university-wide analytics, risk distribution, pass probability bands
- 🤖 **ML Prediction Engine** — three trained models with explainable AI recommendations
- 📝 **Prediction History** — every prediction stored for tracking student improvement over time
- 🌙 **Dark/Light Theme** support

---

## Tech Stack

### Frontend
| Technology | Purpose |
|---|---|
| React 18 + TypeScript | UI framework |
| Vite | Build tool |
| TailwindCSS | Styling |
| Recharts | Data visualisations |
| Zustand | State management |
| Axios | HTTP client |
| React Router | Client-side routing |

### Backend
| Technology | Purpose |
|---|---|
| ASP.NET Core 9 (Web API) | REST API |
| Entity Framework Core | ORM |
| PostgreSQL (Neon) | Database |
| BCrypt | Password hashing |
| JWT Bearer Tokens | Authentication |
| Clean Architecture | System design pattern |

### Machine Learning
| Technology | Purpose |
|---|---|
| Python 3.11 | ML runtime |
| FastAPI | ML microservice |
| Scikit-learn | ML models |
| Pandas + NumPy | Data processing |
| Joblib | Model serialisation |

### Deployment
| Service | Platform |
|---|---|
| Frontend | Vercel |
| .NET API | Render (Docker) |
| Python ML | Render |
| Database | Neon (PostgreSQL) |

---

## System Architecture

```
React Frontend (Vercel)
        ↓  JWT + REST
.NET Core API (Render)
        ↓  HTTP POST          ↓  EF Core
Python ML Service (Render)   PostgreSQL (Neon)
```

The system follows a **microservices-lite** architecture:
- The **.NET API** handles authentication, data management, and business logic
- The **Python ML service** handles model inference — kept separate so models can be updated independently
- The **React frontend** communicates only with the .NET API, never directly with the ML service

The .NET backend follows **Clean Architecture** with four layers:
```
StudentSuccess.API           ← Controllers, middleware, entry point
StudentSuccess.Application   ← Business logic, services, DTOs, interfaces
StudentSuccess.Domain        ← Entities, enums, repository interfaces
StudentSuccess.Infrastructure ← EF Core, repositories, service implementations
```

---

## Machine Learning Models

Three models were trained on a synthetic dataset of 2,000 student records generated using realistic university performance distributions.

### Features Used
| Feature | Description | Weight in Outcome |
|---|---|---|
| `attendance_percentage` | Module attendance rate (0–100%) | 30% |
| `assignment_average` | Average assignment marks (0–100%) | 35% |
| `test_average` | Average test marks (0–100%) | 35% |
| `lms_login_count` | Learning Management System activity | Supplementary |

### Models & Performance

| Model | Purpose | Accuracy | F1 Score |
|---|---|---|---|
| Logistic Regression | Pass/Fail probability | ~97.8% | ~0.986 |
| Random Forest | Risk level classification | ~97.3% | ~0.983 |
| Decision Tree | Explainable rule-based risk | ~96.5% | ~0.978 |

### Feature Importance (Random Forest)
```
attendance_percentage    0.4257  ████████████████████
assignment_average       0.2407  ████████████
test_average             0.2281  ███████████
lms_login_count          0.1055  █████
```

### Prediction Output
```json
{
  "pass_probability": 0.8234,
  "fail_probability": 0.1766,
  "risk_level": "Low",
  "predicted_outcome": "Pass",
  "recommendation": "Student is performing well. Encourage continued engagement.",
  "model_version": "1.0.0"
}
```

### Explainable AI
Recommendations are generated based on the weakest performance area:
- **High Risk:** Urgent intervention — tutoring, academic advisor referral, structured study plan
- **Medium Risk:** Monitoring — extra revision sessions, LMS engagement improvement
- **Low Risk:** Encouragement to maintain performance

---

## User Roles & Dashboards

### Student
- Views personal performance trends per module
- Sees pass probability gauge from latest prediction
- Receives specific recommendations for at-risk modules
- Cannot view other students' data

### Lecturer
- Selects a module from dropdown
- Views class performance bar chart
- Runs individual or batch predictions with one click
- Sees risk distribution pie chart
- Gets urgent intervention alerts for high-risk students

### Academic Advisor
- Searches and filters all students by name, number, or risk level
- Clicks any student to see their full intervention plan
- Views risk breakdown by academic programme
- Uses the detail panel to plan individual consultations

### Administrator
- University-wide overview — all students, all modules
- Pass probability distribution across the institution
- Enrollment counts per module
- Full student risk table sorted by highest fail probability

---

## API Documentation

Full Swagger documentation available at:
```
https://https://student-success-api-5l9o.onrender.com/swagger
```

### Key Endpoints

| Method | Endpoint | Role | Description |
|---|---|---|---|
| POST | `/api/auth/register` | Public | Register a new user |
| POST | `/api/auth/login` | Public | Login and get JWT |
| GET | `/api/students` | Lecturer, Admin | Get all students |
| GET | `/api/students/me` | Student | Get own profile |
| GET | `/api/modules` | All | Get all modules |
| POST | `/api/modules` | Admin | Create a module |
| POST | `/api/enrollments` | Admin, Advisor | Enroll student in module |
| PUT | `/api/enrollments/{id}/performance` | Lecturer, Admin | Update student marks |
| POST | `/api/predictions` | Lecturer, Admin, Advisor | Run ML prediction |
| GET | `/api/predictions/student/{id}` | All | Get student predictions |
| GET | `/api/predictions/latest` | Admin, Advisor | Get latest per student |

---

## Project Structure

```
StudentSuccessPredictor/
├── backend/                          # ASP.NET Core Web API
│   ├── StudentSuccess.API/           # Controllers, Program.cs, middleware
│   ├── StudentSuccess.Application/   # Services, DTOs, interfaces
│   ├── StudentSuccess.Domain/        # Entities, enums
│   ├── StudentSuccess.Infrastructure/# EF Core, repositories, DB context
│   └── Dockerfile
│
├── frontend/                         # React + TypeScript
│   ├── src/
│   │   ├── components/               # RiskBadge, StatCard, Navbar, ProtectedRoute
│   │   ├── pages/
│   │   │   ├── student/Dashboard.tsx
│   │   │   ├── lecturer/Dashboard.tsx
│   │   │   ├── advisor/Dashboard.tsx
│   │   │   └── admin/Dashboard.tsx
│   │   ├── services/                 # Axios API calls
│   │   ├── store/                    # Zustand auth store
│   │   └── types/                    # TypeScript interfaces
│   └── .env.production
│
└── ml-service/                       # Python FastAPI + ML
    ├── app/
    │   ├── models/                   # Trained .joblib model files
    │   └── services/prediction_service.py
    ├── notebooks/
    │   ├── generate_dataset.py       # Synthetic data generation
    │   └── train_models.py           # Model training script
    ├── data/student_performance.csv
    ├── main.py
    └── requirements.txt
```

---

## Getting Started

### Prerequisites
- .NET 9 SDK
- Python 3.11
- Node.js 18+
- PostgreSQL (local) or Neon account

### 1. Clone the repository
```bash
git clone https://github.com/DevMbhoni/student-success-predictor.git
cd student-success-predictor
```

### 2. Set up the database
Create a local PostgreSQL database or use a Neon connection string.

### 3. Configure the .NET API
Create `backend/StudentSuccess.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=StudentSuccessDb;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-minimum-32-characters",
    "Issuer": "StudentSuccessAPI",
    "Audience": "StudentSuccessClient",
    "ExpiryHours": 24
  },
  "PythonMLService": {
    "BaseUrl": "http://localhost:8000"
  }
}
```

### 4. Run the .NET API
```bash
cd backend
dotnet restore
dotnet run --project StudentSuccess.API
```
The database will be created and seeded automatically on first run.

### 5. Train and run the ML service
```bash
cd ml-service
python -m venv venv
venv\Scripts\activate          # Windows
pip install -r requirements.txt

# Train the models (first time only)
cd notebooks
python train_models.py
cd ..

# Start the ML service
uvicorn main:app --reload --port 8000
```

### 6. Run the React frontend
```bash
cd frontend
npm install
npm run dev
```

Open `http://localhost:5173` and log in with the seeded credentials.

---

## Deployment

The application is deployed across three free-tier cloud services:

### Neon (PostgreSQL)
- Create a project at [neon.tech](https://neon.tech)
- Copy the direct connection string (not pooler)

### Render (Python ML Service)
- Runtime: Python 3.11
- Root directory: `ml-service`
- Build: `pip install -r requirements.txt`
- Start: `uvicorn main:app --host 0.0.0.0 --port $PORT`

### Render (.NET API)
- Runtime: Docker
- Root directory: `backend`
- Environment variables: connection string, JWT settings, ML service URL

### Vercel (React Frontend)
- Framework: Vite
- Root directory: `frontend`
- Environment variable: `VITE_API_URL=https://your-api.onrender.com`

---

## Author

**Mbhoni Shipalana**

Computer Science & Statistics Graduate  
Aspiring Software Engineer & Data Analyst

📧 Email: shipalanambhoniii@gmail.com  
💼 LinkedIn: https://www.linkedin.com/in/mbhoni-shipalana-83b9b826b

---

## Acknowledgements

- Dataset generated synthetically using realistic South African university performance distributions
- Models trained using Scikit-learn with Logistic Regression, Random Forest, and Decision Tree classifiers
- UI design inspired by modern SaaS analytics dashboards
