from datetime import datetime, timedelta, timezone

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI(title="Degen API")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000", "http://127.0.0.1:3000"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.get("/health")
def health():
    return {"ok": True}

@app.get("/api/sparkline")
def sparkline():
    now = datetime.now(timezone.utc).replace(minute=0, second=0, microsecond=0)
    start = now - timedelta(hours=120)

    points = []
    value = 100.0
    for i in range(120):
        t = start + timedelta(hours=i)
        drift = (i % 17 - 8) * 0.12
        value += drift
        points.append({"time": int(t.timestamp()), "value": round(value, 2)})

    return points
