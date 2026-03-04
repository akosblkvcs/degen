FROM python:3.12-slim
WORKDIR /app

ENV PYTHONDONTWRITEBYTECODE=1
ENV PYTHONUNBUFFERED=1

COPY --from=ghcr.io/astral-sh/uv:latest /uv /usr/local/bin/uv

COPY pyproject.toml uv.lock ./
RUN uv venv && uv sync --frozen --no-dev

COPY src ./src

EXPOSE 8000
CMD ["/app/.venv/bin/uvicorn", "src.main:app", "--host=0.0.0.0", "--port=8000"]