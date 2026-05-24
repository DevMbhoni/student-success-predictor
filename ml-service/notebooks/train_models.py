import pandas as pd
import numpy as np
import joblib
import os
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler
from sklearn.linear_model import LogisticRegression
from sklearn.ensemble import RandomForestClassifier
from sklearn.tree import DecisionTreeClassifier
from sklearn.metrics import (
    accuracy_score, precision_score,
    recall_score, f1_score, confusion_matrix,
    classification_report
)

df = pd.read_csv("../data/student_performance.csv")

FEATURES = [
    "attendance_percentage",
    "assignment_average",
    "test_average",
    "lms_login_count"
]

X = df[FEATURES]
y = df["outcome"]  

X_train, X_test, y_train, y_test = train_test_split(
    X, y, test_size=0.2, random_state=42, stratify=y
)


scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_test_scaled  = scaler.transform(X_test)

print("Training Logistic Regression...")
lr_model = LogisticRegression(random_state=42, max_iter=1000)
lr_model.fit(X_train_scaled, y_train)
lr_preds = lr_model.predict(X_test_scaled)

print("\n── Logistic Regression ──────────────────")
print(classification_report(y_test, lr_preds, target_names=["Fail", "Pass"]))

print("Training Random Forest...")
rf_model = RandomForestClassifier(
    n_estimators=100,   
    max_depth=10,       
    random_state=42
)
rf_model.fit(X_train, y_train)  
rf_preds = rf_model.predict(X_test)

print("\n── Random Forest ────────────────────────")
print(classification_report(y_test, rf_preds, target_names=["Fail", "Pass"]))

print("Training Decision Tree...")
dt_model = DecisionTreeClassifier(
    max_depth=5,        
    random_state=42
)
dt_model.fit(X_train, y_train)
dt_preds = dt_model.predict(X_test)

print("\n── Decision Tree ────────────────────────")
print(classification_report(y_test, dt_preds, target_names=["Fail", "Pass"]))

print("\n── Model Comparison ─────────────────────")
models = {
    "Logistic Regression": (lr_model, lr_preds),
    "Random Forest":       (rf_model, rf_preds),
    "Decision Tree":       (dt_model, dt_preds),
}
for name, (model, preds) in models.items():
    acc = accuracy_score(y_test, preds)
    f1  = f1_score(y_test, preds)
    print(f"{name:22} | Accuracy: {acc:.4f} | F1: {f1:.4f}")

print("\n── Feature Importance (Random Forest) ───")
for feat, imp in sorted(
    zip(FEATURES, rf_model.feature_importances_),
    key=lambda x: x[1], reverse=True
):
    print(f"  {feat:28} {imp:.4f}")

os.makedirs("../app/models", exist_ok=True)

joblib.dump(lr_model, "../app/models/logistic_regression.joblib")
joblib.dump(rf_model, "../app/models/random_forest.joblib")
joblib.dump(dt_model, "../app/models/decision_tree.joblib")
joblib.dump(scaler,   "../app/models/scaler.joblib")

print("\n✓ Models saved to app/models/")