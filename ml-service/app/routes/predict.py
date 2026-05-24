from fastapi import APIRouter, HTTPException
from pydantic import BaseModel, Field

router = APIRouter()

class PredictRequest(BaseModel):
    attendance_percentage: float = Field(..., ge=0, le=100)
    assignment_average:    float = Field(..., ge=0, le=100)
    test_average:          float = Field(..., ge=0, le=100)
    lms_login_count:       int   = Field(..., ge=0)

    class Config:
        json_schema_extra = {
            "example": {
                "attendance_percentage": 48.5,
                "assignment_average":    41.0,
                "test_average":          38.5,
                "lms_login_count":       12
            }
        }

class PredictResponse(BaseModel):
    pass_probability:  float
    fail_probability:  float
    risk_level:        str
    predicted_outcome: str
    recommendation:    str
    model_version:     str
    feature_breakdown: dict