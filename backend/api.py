from io import BytesIO
import signal
import sys
from fastapi import FastAPI, HTTPException, Response, Query
from pydantic import BaseModel
from fastapi.middleware.cors import CORSMiddleware
from sqlalchemy import create_engine, Column, String, Integer
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker
import numpy as np
import logging
import matplotlib.pyplot as pyplot
from matplotlib.ticker import MaxNLocator

from roles import Rol
from usuario import Usuario

DATABASE_URL = "mysql+pymysql://root:helena@localhost/gk_stats_web"  
logging.basicConfig(level=logging.INFO)

engine = create_engine(DATABASE_URL)
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)
Base = declarative_base()

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ------------------- Models -------------------
class User(Base):
    __tablename__ = "users"
    id = Column(Integer, primary_key=True, index=True)
    email = Column(String)
    password = Column(String)
    name = Column(String)
    role = Column(String)
    teamID = Column(Integer)

class Session(Base):
    __tablename__ = "sessions"
    id = Column(Integer, primary_key=True, index=True)
    player_id = Column(Integer)
    date = Column(String)
    game_mode = Column(String)
    prestige_level = Column(String)

class LoginRequest(BaseModel):
    email: str
    password: str

class RegisterRequest(BaseModel):
    name: str
    email: str
    password: str
    team: int
    user_type: str  # 'portero' o 'entrenador'

# ------------------- Authentication -------------------
@app.post("/api/register")
def register_user(request: RegisterRequest):
    """Función para registrar un nuevo usuario."""
    logging.info(f"Received register request for email: {request.email}")
    session = SessionLocal()
    user = session.query(User).filter(User.email == request.email).first()
    if user:
        raise HTTPException(status_code=400, detail=f"El email {request.email} ya está en uso.")
    else:
        try: 
            user = User(email=request.email, password=request.password, name=request.name, role=request.user_type, teamID=request.team)
            session.add(user)
            session.commit()
            logging.info(f"User {request.name} registered successfully")
            return {"message": f"Usuario {request.name} registrado con éxito."}
        except Exception as e:
            logging.error(f"Error registering user: {e}")
            session.close()
            raise HTTPException(status_code=500, detail="Error al registrar el usuario.")
        finally:
            session.close()
            
@app.post("/api/login-staff")
async def login(request: LoginRequest):
    """Función para iniciar sesión del cuerpo técnico."""
    logging.info(f"Received login request for email: {request.email}")
    session = SessionLocal()
    user = session.query(User).filter(User.email == request.email).first()
    session.close()
    
    if user:
        if user.password == request.password and user.role == Rol.ENTRENADOR.value:
            return {"message": "Login successful"}
        else:
            raise HTTPException(status_code=401, detail="Invalid email or password")
    else:
        raise HTTPException(status_code=404, detail="User not found")


@app.post("/api/login-players")
async def login(request: LoginRequest):
    """Función para iniciar sesión de jugadores."""
    logging.info(f"Received login request for email: {request.email}")
    session = SessionLocal()
    user = session.query(User).filter(User.email == request.email).first()
    session.close()
    
    if user:
        if user.password == request.password and user.role == Rol.PORTERO.value:
            return {"message": "Login successful"}
        else:
            raise HTTPException(status_code=401, detail="Invalid email or password")
    else:
        raise HTTPException(status_code=404, detail="User not found")
    
    
# ------------------- Players -------------------
@app.get("/api/players")
def get_players():
    """Función para obtener la lista de jugadores."""
    session = SessionLocal()
    players = session.query(User).filter(User.role == Rol.PORTERO.value).all()
    session.close()
    return players


# ------------------- Sessions -------------------
@app.get("/api/sessions/{player_id}")
def get_sessions(player_id: int, mode: str = Query(None), level: str = Query(None)):
    """Función para obtener la lista de sesiones de un jugador."""
    session = SessionLocal()
    sessions = session.query(Session).filter(Session.player_id == player_id)

    if mode:
        sessions = sessions.filter(Session.game_mode == mode)
    if level:
        sessions = sessions.filter(Session.prestige_level == level)

    sessions = sessions.all()
    
    session.close()
    return sessions

def shutdown():
    sys.exit(0)

signal.signal(signal.SIGTERM, shutdown)

if __name__ == "__main__":
    import uvicorn
    Base.metadata.create_all(bind=engine)
    uvicorn.run(app, host="0.0.0.0", port=5000)