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
import matplotlib
matplotlib.use("Agg")
from matplotlib import pyplot
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

class SessionData(Base):
    __tablename__ = "sessions_data"
    session_id = Column(Integer, primary_key=True, index=True)
    shoots_time = Column(String)
    n_goals = Column(Integer)
    n_saves = Column(Integer)
    shoots_initial_point = Column(String)
    shoots_initial_zone = Column(String)
    shoots_final_point = Column(String)
    shoots_final_zone = Column(String)
    shoots_result = Column(String)
    saves_bodypart = Column(String)
    
class SessionTracking(Base):
    __tablename__ = "sessions_tracking"
    session_id = Column(Integer, primary_key=True, index=True)
    head_pos_x = Column(Integer)
    head_pos_y = Column(Integer)
    head_pos_z = Column(Integer)
    handR_pos_x = Column(Integer)
    handR_pos_y = Column(Integer)
    handR_pos_z = Column(Integer)
    handR_rot_x = Column(Integer)
    handR_rot_y = Column(Integer)
    handR_rot_z = Column(Integer)
    handR_speed_x = Column(Integer)
    handR_speed_y = Column(Integer)
    handR_speed_z = Column(Integer)
    handL_pos_x = Column(Integer)
    handL_pos_y = Column(Integer)
    handL_pos_z = Column(Integer)
    handL_rot_x = Column(Integer)
    handL_rot_y = Column(Integer)
    handL_rot_z = Column(Integer)
    handL_speed_x = Column(Integer)
    handL_speed_y = Column(Integer)
    handL_speed_z = Column(Integer)
    
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

@app.get("/api/sessionData/{session_id}")
def get_session_data(session_id: int):
    """Función para obtener los datos de una sesión."""
    session = SessionLocal()
    data = session.query(SessionData).filter(SessionData.session_id == session_id).all()
    session.close()
    return data

@app.get("/api/sessionTracking/{session_id}")
def get_session_tracking(session_id: int):
    """Función para obtener el tracking de una sesión."""
    session = SessionLocal()
    sessions = session.query(SessionTracking).filter(SessionTracking.session_id == session_id).all()
    session.close()
    return sessions 
# ------------------- Metrics Charts -------------------
@app.get("/api/barchart-shoots/{session_id}")
def get_barchart_shoots(session_id: int):
    """Función que crea el gráfico de barras de lanzamientos de una sesión."""
    session_data = get_session_data(session_id)
    
    if session_data:
        first_session_data = session_data[0]
        shot_results = first_session_data.shoots_result.split(", ")
        shot_zones = first_session_data.shoots_final_zone.split(", ")
                
    zone_map = {
        "escuadra izquierda": 0,
        "centro izquierda": 1,
        "centro": 2,
        "centro derecha": 3,
        "escuadra derecha": 4,
        "bajo izquierda": 5,
        "centro abajo izquierda": 6,
        "centro abajo": 7,
        "centro abajo derecha": 8,
        "bajo derecha": 9
    }
    
    goal_matrix = np.zeros(len(zone_map))
    save_matrix = np.zeros(len(zone_map))
    
    for result, zone in zip(shot_results, shot_zones):
        if zone in zone_map:
            index = zone_map[zone]
            if result == "goal":
                goal_matrix[index] += 1
            else:
                save_matrix[index] += 1
    
    x = np.arange(len(zone_map.keys()))
    width = 0.35
    pyplot.figure(figsize=(12, 6))

    pyplot.bar(x-0.2, goal_matrix, width, label='Goles', color='blue')
    pyplot.bar(x+0.2, save_matrix, width, label='Paradas', color='red')
    
    pyplot.ylabel('Cantidad de lanzamientos')
    pyplot.title('Goles y paradas por zonas')
    pyplot.xticks(x, zone_map.keys(), rotation=45, ha='right')
    pyplot.gca().yaxis.set_major_locator(MaxNLocator(integer=True))
    pyplot.legend()

    pyplot.tight_layout()

    buffer = BytesIO()
    pyplot.savefig(buffer, format='png', bbox_inches='tight')
    pyplot.close()
    buffer.seek(0)
        
    return Response(buffer.getvalue(), media_type="image/png")

@app.get("/api/barchart-saves/{session_id}")
def get_barchart_saves(session_id: int):
    """Función que crea el gráfico de barras de paradas de una sesión."""
    session_data = get_session_data(session_id)
    
    if session_data:
        first_session_data = session_data[0]
        save_bodypart = first_session_data.saves_bodypart.split(", ")
                
    zone_map = {
        "Left Hand": 0,
        "Left Forearm": 1,
        "Left Upperarm": 2,
        "Right Hand": 3,
        "Right Forearm": 4,
        "Right Upperarm": 5,
        "Trunk": 6,
        "Head": 7,
    }
    
    labels = [
        "Mano Izquierda",
        "Antebrazo Izquierdo",
        "Parte Superior del Brazo Izquierdo",
        "Mano Derecha",
        "Antebrazo Derecho",
        "Parte Superior del Brazo Derecho",
        "Tronco",
        "Cabeza",
    ]
        
    matrix = np.zeros(len(zone_map))
    
    for part in save_bodypart:
        if part in zone_map:
            index = zone_map[part]
            matrix[index] += 1
    
    x = np.arange(len(zone_map.keys()))
    width = 0.35
    pyplot.figure(figsize=(12, 6))

    pyplot.bar(x, matrix, width, label='Paradas', color='blue')
    
    pyplot.ylabel('Cantidad de lanzamientos')
    pyplot.title('Paradas por partes del cuerpo')
    pyplot.xticks(x, labels, rotation=45, ha='right')
    pyplot.gca().yaxis.set_major_locator(MaxNLocator(integer=True))
    pyplot.legend()

    pyplot.tight_layout()

    buffer = BytesIO()
    pyplot.savefig(buffer, format='png', bbox_inches='tight')
    pyplot.close()
    buffer.seek(0)
        
    return Response(buffer.getvalue(), media_type="image/png")

@app.get("/api/heatmap/{session_id}")
def get_heatmap(session_id: int):
    """Funcion que crea el heatmap de una sesion"""
    session_data = get_session_data(session_id)

    if session_data: #CAMBIAR A FOR EACH PARA PROGRESO
        first_session_data = session_data[0]
        shot_results = first_session_data.shoots_result.split(", ") 
        shot_zones = first_session_data.shoots_final_zone.split(", ") 

    zone_map = {
        "escuadra izquierda": (0, 0),
        "centro izquierda": (0, 1),
        "centro": (0, 2),
        "centro derecha": (0, 3),
        "escuadra derecha": (0, 4),
        "bajo izquierda": (1, 0),
        "centro abajo izquierda": (1, 1),
        "centro abajo": (1, 2),
        "centro abajo derecha": (1, 3),
        "bajo derecha": (1, 4)
    }
    
    goal_matrix = np.zeros((2, 5))
    save_matrix = np.zeros((2, 5))
    
    for result, zone in zip(shot_results, shot_zones):
        if zone in zone_map:
            x, y = zone_map[zone]
            if result == "goal":
                goal_matrix[x, y] += 1
            else:
                save_matrix[x, y] += 1

    total_matrix = goal_matrix + save_matrix

    #Para las zonas sin lanzamientos
    mask = total_matrix == 0
    heatmap_data = np.ma.masked_array(goal_matrix, mask=mask)
            
    pyplot.figure(figsize=(8, 2))
    heatmap = pyplot.imshow(heatmap_data, cmap='coolwarm', interpolation='nearest') #cmap='hot',
    #heatmap.cmap.set_bad('white')

    for i in range(2):
        for j in range(5):
            value = f"{int(goal_matrix[i, j])}/{int(total_matrix[i, j])}" if total_matrix[i, j] > 0 else "0/0"
            pyplot.text(j, i, value, ha='center', va='center', color="black")
    
    pyplot.yticks([])
    pyplot.xticks([])

    color_bar = pyplot.colorbar(heatmap)
    color_bar.set_label("Proporción de Goles")
    color_bar.set_ticks(range(int(goal_matrix.max()) + 1)) 
    
    buffer = BytesIO()
    pyplot.savefig(buffer, format='png', bbox_inches='tight', pad_inches=0, transparent=True)
    pyplot.close()
    buffer.seek(0)
    
    return Response(buffer.getvalue(), media_type="image/png")    

def shutdown():
    sys.exit(0)

signal.signal(signal.SIGTERM, shutdown)

if __name__ == "__main__":
    import uvicorn
    Base.metadata.create_all(bind=engine)
    uvicorn.run(app, host="0.0.0.0", port=5000)