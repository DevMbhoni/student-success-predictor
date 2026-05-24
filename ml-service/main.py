from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from app.routes.predict import router as predict_router
from app.services.prediction_service import PredictionService
from pydantic import BaseModel, Field

app = FastAPI(
    title="Student Success Predictor — ML Service",
    description="Predicts student pass/fail probability and risk level",
    version="1.0.0"
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  
    allow_methods=["*"],
    allow_headers=["*"],
)

prediction_service = PredictionService()

class PredictRequest(BaseModel):
    attendance_percentage: float = Field(..., ge=0, le=100)
    assignment_average:    float = Field(..., ge=0, le=100)
    test_average:          float = Field(..., ge=0, le=100)
    lms_login_count:       int   = Field(..., ge=0)

@app.get("/")
def health_check():
    return {"status": "ML service is running", "version": "1.0.0"}

@app.post("/api/predict")
def predict(request: PredictRequest):
    result = prediction_service.predict(
        attendance     = request.attendance_percentage,
        assignment_avg = request.assignment_average,
        test_avg       = request.test_average,
        lms_logins     = request.lms_login_count
    )
    return result

@app.get("/api/health")
def detailed_health():
    return {
        "status":  "healthy",
        "models":  ["logistic_regression", "random_forest", "decision_tree"],
        "version": "1.0.0"
    }