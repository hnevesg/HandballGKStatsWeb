import os
import shutil
import signal
import sys
import numpy as np
import logging
import uvicorn
import hashlib
import matplotlib
matplotlib.use("Agg")
from io import BytesIO
from fastapi import FastAPI, HTTPException, Response, Query, UploadFile, File
from fastapi.responses import JSONResponse
from fastapi.exceptions import RequestValidationError
from starlette.middleware.base import BaseHTTPMiddleware
from pydantic import BaseModel
from fastapi.middleware.cors import CORSMiddleware
from sqlalchemy import create_engine, Column, String, Integer, Float
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker
from matplotlib import pyplot
from matplotlib.ticker import MaxNLocator
from matplotlib.colors import LinearSegmentedColormap

from roles import Rol
from send_to_db import load_csv_to_mysql

UPLOAD_FOLDER = "uploads"
os.makedirs(UPLOAD_FOLDER, exist_ok=True)

DATABASE_URL = "mysql+pymysql://root:helena@192.168.43.173/gk_stats_web"  

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
    salt = Column(String)
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
    session_date = Column(String, primary_key=True, index=True)
    session_time = Column(String)
    n_goals = Column(Integer)
    n_saves = Column(Integer)
    n_lights = Column(Integer)
    shoots_initial_time = Column(String)
    shoots_final_time = Column(String)
    shoots_initial_point = Column(String)
    shoots_initial_zone = Column(String)
    shoots_final_point = Column(String)
    shoots_final_zone = Column(String)
    shoots_result = Column(String)
    saves_bodypart = Column(String)
    target_goals = Column(Integer)
    
class SessionTracking(Base):
    __tablename__ = "sessions_tracking"
    id = Column(Integer, primary_key=True, index=True)
    session_date = Column(String)
    Frame = Column(Integer)
    Time = Column(Float)
    HeadPosition_x = Column(Float)
    HeadPosition_y = Column(Float)
    HeadPosition_z = Column(Float)
    RHandPosition_x = Column(Float)
    RHandPosition_y = Column(Float)
    RHandPosition_z = Column(Float)
    RHandRotation_x = Column(Float)
    RHandRotation_y = Column(Float)
    RHandRotation_z = Column(Float)
    RHandVelocity_x = Column(Float)
    RHandVelocity_y = Column(Float)
    RHandVelocity_z = Column(Float)
    LHandPosition_x = Column(Float)
    LHandPosition_y = Column(Float)
    LHandPosition_z = Column(Float)
    LHandRotation_x = Column(Float)
    LHandRotation_y = Column(Float)
    LHandRotation_z = Column(Float)
    LHandVelocity_x = Column(Float)
    LHandVelocity_y = Column(Float)
    LHandVelocity_z = Column(Float)

class SessionReaction(Base):
    __tablename__ = "sessions_reaction"
    id = Column(Integer, primary_key=True, index=True)
    session_date = Column(String)
    Frame = Column(Integer)
    Time = Column(Float) 
    light_id = Column(Integer)
    status = Column(String)
            
class LoginRequest(BaseModel):
    email: str
    password: str

class RegisterRequest(BaseModel):
    name: str
    email: str
    password: str
    salt: str
    team: int
    user_type: str  # 'portero' o 'entrenador'

# ------------------- Authentication -------------------
@app.post("/register")
def register_user(request: RegisterRequest):
    """Función para registrar un nuevo usuario."""
    session = SessionLocal()
    user = session.query(User).filter(User.email == request.email).first()
    if user:
        raise HTTPException(status_code=400, detail=f"Email {request.email} already in use.")
    else:
        try: 
            user = User(email=request.email, password=request.password, salt=request.salt, name=request.name, role=request.user_type, teamID=request.team)
            session.add(user)
            session.commit()
            logging.info(f"User {request.name} registered successfully")
            return {"message": f"User {request.name} registered successfully."}
        except Exception as e:
            logging.error(f"Error registering user: {e}")
            session.close()
            raise HTTPException(status_code=500, detail="Error while registrating user.")
        finally:
            session.close()
            
def authenticate_user(user: User, password: str, role: str):
    hashed_password = hashlib.sha256((password + user.salt).encode('utf-8')).hexdigest()
    if user.password == hashed_password and user.role == role:
        return {"message": "Login successful"}
    else:
        raise HTTPException(status_code=401, detail="Invalid email or password")

@app.post("/login-staff")
async def login_staff(request: LoginRequest):
    """Función para iniciar sesión del cuerpo técnico."""
    session = SessionLocal()
    user = session.query(User).filter(User.email == request.email).first()
    session.close()
    if user:
        return authenticate_user(user, request.password, Rol.ENTRENADOR.value)
    else:
        raise HTTPException(status_code=404, detail="User not found")

@app.post("/login-players")
async def login_players(request: LoginRequest):
    """Función para iniciar sesión de jugadores."""
    session = SessionLocal()
    user = session.query(User).filter(User.email == request.email).first()
    session.close()
    if user:
        return authenticate_user(user, request.password, Rol.PORTERO.value)
    else:
        raise HTTPException(status_code=404, detail="User not found")

@app.get("/players/{team_id}")
def get_players(team_id: int):
    """Función para obtener la lista de jugadores."""
    session = SessionLocal()
    players = session.query(User).filter(User.role == Rol.PORTERO.value, User.teamID == team_id).all()
    session.close()
    return players

@app.get("/user/{email}")
def get_user(email: str):
    """Función para obtener un jugador por email."""
    session = SessionLocal()
    player = session.query(User).filter(User.email == email).first()
    session.close()
    return player

# ------------------- Sessions -------------------
@app.get("/sessions/{player_id}")
def get_sessions(player_id: int, mode: str = Query(None), level: str = Query(None), begin_date: str = Query(None), end_date: str = Query(None)):
    """Función para obtener la lista de sesiones de un jugador."""
    session = SessionLocal()
    sessions = session.query(Session).filter(Session.player_id == player_id)

    if mode:
        sessions = sessions.filter(Session.game_mode == mode)
    if level:
        sessions = sessions.filter(Session.prestige_level == level)
    if begin_date:
        sessions = sessions.filter(Session.date >= begin_date)
    if end_date:
        sessions = sessions.filter(Session.date <= end_date)

    sessions = sessions.all()
    
    session.close()
    return sessions

@app.get("/team-sessions/{team_id}")
def get_team_sessions(team_id: int):
    """Función para obtener la lista de sesiones de un equipo."""
    session = SessionLocal()
    sessions = session.query(Session).filter(User.teamID == team_id).all()
    session.close()
    return sessions

@app.get("/sessionData/{session_date}")
def get_session_data(session_date: str):
    """Función para obtener los datos de una sesión."""
    session = SessionLocal()
    data = session.query(SessionData).filter(SessionData.session_date == session_date).first()
    session.close()
    return data

@app.get("/sessionTracking/{session_date}")
def get_session_tracking(session_date: str):
    """Función para obtener el tracking de una sesión."""
    session = SessionLocal()
    tracking_data = session.query(SessionTracking).filter(SessionTracking.session_date == session_date).all()
    session.close()
    return tracking_data 

@app.get("/sessionReaction/{session_date}")
def get_session_reaction(session_date: str):
    """Función para obtener los datos de reacción de una sesión."""
    session = SessionLocal()
    reactions = session.query(SessionReaction).filter(SessionReaction.session_date == session_date).all()
    session.close()
    return reactions

# ------------------- Metrics Charts -------------------
@app.get("/barchart-shoots/{session_date}")
def get_barchart_shoots(session_date: str):
    """Función que crea el gráfico de barras de lanzamientos de una sesión."""
    session_data = get_session_data(session_date)
    
    shoot_results = session_data.shoots_result.split(",")
    
    goals = 0
    saves = 0
    
    for result in shoot_results:
        if result == "Gol recibido":
            goals += 1
        else:
            saves += 1
    
    labels = ['Goals', 'Saves']
    x = np.arange(len(labels))
    width = 0.15
    fig, ax = pyplot.subplots(figsize=(6, 4))

    ax.bar(x[0], goals, width, label='Goals', color='red')
    ax.bar(x[1], saves, width, label='Saves', color='blue')
    
    ax.set_ylabel('Nº of shoots')
    ax.set_title('Goals and saves per zone')
    ax.set_xticks(x)
    ax.set_xticklabels(labels, rotation=45, ha='right')
    ax.yaxis.set_major_locator(MaxNLocator(integer=True))
    ax.legend()

    fig.tight_layout()

    buffer = BytesIO()
    fig.savefig(buffer, format='png', bbox_inches='tight')
    pyplot.close(fig)
    buffer.seek(0)
        
    return Response(buffer.getvalue(), media_type="image/png")

@app.get("/barchart-saves/{session_date}")
def get_barchart_saves(session_date: str):
    """Función que crea el gráfico de barras de paradas de una sesión."""
    session_data = get_session_data(session_date)
    
    save_bodypart = session_data.saves_bodypart.split(",")
                
    zone_map = {
        "LeftHand": 0,
        "LeftForearm": 1,
        "LeftUpperarm": 2,
        "RightHand": 3,
        "RightForearm": 4,
        "RightUpperarm": 5,
        "Trunk": 6,
        "Head": 7,
    }
    
    labels = [
        "Left Hand",
        "Left Forearm",
        "Left Upperarm",
        "Right Hand",
        "Right Forearm",
        "Right Upperarm",
        "Trunk",
        "Head",
    ]
        
    matrix = np.zeros(len(zone_map))
    
    for part in save_bodypart:
        if part in zone_map:
            index = zone_map[part]
            matrix[index] += 1
    
    x = np.arange(len(zone_map.keys()))
    width = 0.35
    fig, ax = pyplot.subplots(figsize=(12, 6))

    ax.bar(x, matrix, width, label='Saves', color='blue')
    
    ax.set_ylabel('Nº of shoots')
    ax.set_title('Saves per bodypart')
    ax.set_xticks(x)
    ax.set_xticklabels(labels, rotation=45, ha='right')
    ax.yaxis.set_major_locator(MaxNLocator(integer=True))

    fig.tight_layout()

    buffer = BytesIO()
    fig.savefig(buffer, format='png', bbox_inches='tight')
    pyplot.close(fig)
    buffer.seek(0)
        
    return Response(buffer.getvalue(), media_type="image/png")

@app.get("/heatmap/{session_date}")
def get_heatmap(session_date: str):
    """Funcion que crea el heatmap de una sesion"""
    session_data = get_session_data(session_date)

    shoot_results = session_data.shoots_result.split(",") 
    shoot_zones = session_data.shoots_final_zone.split(",") 

    zone_map = {
        "EscuadraIzquierda": (0, 0),
        "CentroIzquierda": (0, 1),
        "Centro": (0, 2),
        "CentroDerecha": (0, 3),
        "EscuadraDerecha": (0, 4),
        "BajoIzquierda": (1, 0),
        "CentroIzquierdaBajo": (1, 1),
        "CentroBajo": (1, 2),
        "CentroDerechaBajo": (1, 3),
        "BajoDerecha": (1, 4)
    }
    
    goal_matrix = np.zeros((2, 5))
    save_matrix = np.zeros((2, 5))
    
    for result, zone in zip(shoot_results, shoot_zones):
        if zone in zone_map:
            x, y = zone_map[zone]
            if result == "Gol recibido":
                goal_matrix[x, y] += 1
            else:
                save_matrix[x, y] += 1

    total_matrix = goal_matrix + save_matrix

    #Para las zonas sin lanzamientos
    mask = total_matrix == 0
    heatmap_data = np.ma.masked_array(goal_matrix, mask=mask)
            
    pyplot.figure(figsize=(8, 2))
    fig, ax = pyplot.subplots()
    colors = ['blue', 'lightcoral', 'red']
    ax.imshow(heatmap_data/total_matrix, cmap=LinearSegmentedColormap.from_list('custom', colors, N=256), interpolation='nearest', vmin=0, vmax=1)
    
    for i in range(2):
        for j in range(5):
            value = f"{int(goal_matrix[i, j])}/{int(total_matrix[i, j])}" if total_matrix[i, j] > 0 else "0/0"
            pyplot.text(j, i, value, ha='center', va='center', color="black")
    
    pyplot.yticks([])
    pyplot.xticks([])
    
    buffer = BytesIO()
    fig.savefig(buffer, format='png', bbox_inches='tight', pad_inches=0, transparent=True)
    pyplot.close(fig)
    buffer.seek(0)
    
    return Response(buffer.getvalue(), media_type="image/png")    

@app.get("/2D-scatterplot-positions/{session_date}")
def get_scatterplot_positions(session_date: str):
    """Función que crea el gráfico 2D de dispersión de posiciones de cabeza y manos en una sesión."""
    session_tracking = get_session_tracking(session_date)
    
    handR_pos_x, handR_pos_y = [], []
    handL_pos_x, handL_pos_y = [], []
    head_pos_x, head_pos_y = [], []

    for frame in session_tracking:
        handR_pos_x.append(frame.RHandPosition_x)
        handR_pos_y.append(frame.RHandPosition_y)
        handL_pos_x.append(frame.LHandPosition_x)
        handL_pos_y.append(frame.LHandPosition_y)
        head_pos_x.append(frame.HeadPosition_x)
        head_pos_y.append(frame.HeadPosition_y)

    handR_pos_x = np.array(handR_pos_x)
    handR_pos_y = np.array(handR_pos_y)
    handL_pos_x = np.array(handL_pos_x)
    handL_pos_y = np.array(handL_pos_y)
    head_pos_x = np.array(head_pos_x)
    head_pos_y = np.array(head_pos_y)
   
    fig, ax = pyplot.subplots()

   # ax.scatter(head_pos_x, head_pos_y, label="Head", color='red', alpha=0.7)
    ax.scatter(handR_pos_x, handR_pos_y, label="Right Hand", color='blue', alpha=0.7)
    ax.scatter(handL_pos_x, handL_pos_y, label="Left Hand", color='green', alpha=0.7)

    ax.set_xlabel('Width')
    ax.set_ylabel('Height')
    ax.set_title('2D Head and Hands Position')
    ax.legend()   

    buffer = BytesIO()
    fig.savefig(buffer, format='png', bbox_inches='tight')
    pyplot.close(fig)
    buffer.seek(0)
        
    return Response(buffer.getvalue(), media_type="image/png")

@app.get("/3D-scatterplot-positions/{session_date}")
def get_3D_scatterplot_positions(session_date: str):
    """Función que crea el gráfico 3D de dispersión de posiciones de cabeza y manos en una sesión."""
    session_tracking = get_session_tracking(session_date)
    
    handR_pos_x, handR_pos_y, handR_pos_z = [], [], []
    handL_pos_x, handL_pos_y, handL_pos_z = [], [], []
    head_pos_x, head_pos_y, head_pos_z = [], [], []

    for frame in session_tracking:
        handR_pos_x.append(frame.RHandPosition_x)
        handR_pos_y.append(frame.RHandPosition_y)
        handR_pos_z.append(frame.RHandPosition_z)
        handL_pos_x.append(frame.LHandPosition_x)
        handL_pos_y.append(frame.LHandPosition_y)
        handL_pos_z.append(frame.LHandPosition_z)
        head_pos_x.append(frame.HeadPosition_x)
        head_pos_y.append(frame.HeadPosition_y)
        head_pos_z.append(frame.HeadPosition_z)

    handR_pos_x = np.array(handR_pos_x)
    handR_pos_y = np.array(handR_pos_y)
    handR_pos_z = np.array(handR_pos_z)
    handL_pos_x = np.array(handL_pos_x)
    handL_pos_y = np.array(handL_pos_y)
    handL_pos_z = np.array(handL_pos_z)
    head_pos_x = np.array(head_pos_x)
    head_pos_y = np.array(head_pos_y)
    head_pos_z = np.array(head_pos_z)
   
    fig = pyplot.figure()
    ax = fig.add_subplot(111, projection='3d') 

   # ax.scatter(head_pos_x, head_pos_y, head_pos_z, label="Head", color='red', alpha=0.7)
    ax.scatter(handR_pos_x, handR_pos_y, handR_pos_z, label="Right Hand", color='blue', alpha=0.7)
    ax.scatter(handL_pos_x, handL_pos_y, handL_pos_z, label="Left Hand", color='green', alpha=0.7)

    ax.set_xlabel('Width')
    ax.set_ylabel('Height')
    ax.set_zlabel('Depth')
#    pyplot.tight_layout()
    ax.set_title('3D Head and Hands Position')
    ax.legend()    

    buffer = BytesIO()
    fig.savefig(buffer, format='png', bbox_inches='tight')
    pyplot.close(fig)
    buffer.seek(0)
        
    return Response(buffer.getvalue(), media_type="image/png")
    
@app.get("/plot-times/{session_date}")
def get_plot_times(session_date: str):
    """Función que crea el gráfico de dispersión de la velocidad de reacción en cada lanzamiento."""
    session_data = get_session_data(session_date)
    
    shoots_initial_time = session_data.shoots_initial_time.split(",")
    shoots_final_time = session_data.shoots_final_time.split(",")

    reaction_times = []
    for initial_time, final_time in zip(shoots_initial_time, shoots_final_time):
        reaction_times.append(float(final_time) - float(initial_time))
    
    reaction_times = np.array(reaction_times)
    n_shots = np.arange(1, len(reaction_times) + 1) 

    fig, ax = pyplot.subplots()

    ax.plot(n_shots, reaction_times, label="Reaction Time per Shot", color='red', alpha=0.7)

    ax.set_xlabel('Shots')
    ax.set_ylabel('Time in seconds')
    ax.set_title('Reaction Time per Shot')

    ax.set_xticks(n_shots)
    ax.set_ylim(bottom=0, top=max(reaction_times) + 0.5)
    fig.tight_layout()

    buffer = BytesIO()
    fig.savefig(buffer, format='png', bbox_inches='tight')
    pyplot.close(fig)
    buffer.seek(0)
        
    return Response(buffer.getvalue(), media_type="image/png")

@app.get("/saves-progress/{player_id}")
def get_saves_progress(player_id: int, begin_date: str, end_date: str, mode: str, level: str):
    """Función que crea el gráfico de progreso de paradas de sesiones."""
    sessions = get_sessions(player_id, mode, level, begin_date, end_date)
   
    i = 0
    saves = []
    session_dates = []
    
    if sessions:
        for session in sessions:
            data = get_session_data(session.date)
            saves.append(data.n_saves)
            session_dates.append(session.date)
            i += 1
        
        fig, ax = pyplot.subplots()
        
        ax.plot(session_dates, saves, label='Saves', color='blue', marker='o', linestyle='-')
        ax.set_ylim(0, max(saves)+1) 
        ax.set_ylabel('Nº of saves')
        ax.set_xlabel('Sessions Dates')
        ax.set_xticks(session_dates)
        ax.set_xticklabels(session_dates, rotation=45, ha='right')
        ax.legend()
        
        fig.tight_layout()

        buffer = BytesIO()
        fig.savefig(buffer, format='png', bbox_inches='tight')
        pyplot.close(fig)
        buffer.seek(0)
            
        return Response(buffer.getvalue(), media_type="image/png")
    else:
        raise HTTPException(status_code=404, detail="No se encontraron sesiones")

@app.get("/heatmap-progress/{player_id}")
def get_heatmap_progress(player_id: int, begin_date: str, end_date: str, mode: str, level: str):
    """Función que crea el heatmap de progreso de sesiones."""
    sessions = get_sessions(player_id, mode, level, begin_date, end_date)

    shoot_results = []
    shoot_zones = []
    
    if sessions:
        for session in sessions:
            data = get_session_data(session.date)
            shoot_results.append(data.shoots_result.split(","))
            shoot_zones.append(data.shoots_final_zone.split(","))

        zone_map = {
            "EscuadraIzquierda": (0, 0),
            "CentroIzquierda": (0, 1),
            "Centro": (0, 2),
            "CentroDerecha": (0, 3),
            "EscuadraDerecha": (0, 4),
            "BajoIzquierda": (1, 0),
            "CentroIzquierdaBajo": (1, 1),
            "CentroBajo": (1, 2),
            "CentroDerechaBajo": (1, 3),
            "BajoDerecha": (1, 4)
       }
        
        goal_matrix = np.zeros((2, 5))
        save_matrix = np.zeros((2, 5))
        
        for results, zones in zip(shoot_results, shoot_zones):
            for zone, result in zip(zones, results):
                if zone in zone_map:
                    x, y = zone_map[zone]
                    if result == "Gol recibido":
                        goal_matrix[x, y] += 1
                    else:
                        save_matrix[x, y] += 1

        total_matrix = goal_matrix + save_matrix

        #Para las zonas sin lanzamientos
        mask = total_matrix == 0

        heatmap_data = np.ma.masked_array(goal_matrix, mask=mask)
                
        fig, ax = pyplot.subplots(figsize=(8, 2))
        colors = ['blue', 'lightcoral', 'red']
        ax.imshow(heatmap_data/total_matrix, cmap=LinearSegmentedColormap.from_list('custom', colors, N=256), interpolation='nearest', vmin=0, vmax=1)

        for i in range(2):
            for j in range(5):
                value = f"{int(goal_matrix[i, j])}/{int(total_matrix[i, j])}" if total_matrix[i, j] > 0 else "0/0"
                ax.text(j, i, value, ha='center', va='center', color="black")
        
        ax.set_yticks([])
        ax.set_xticks([])
        
        buffer = BytesIO()
        fig.savefig(buffer, format='png', bbox_inches='tight', pad_inches=0, transparent=True)
        pyplot.close(fig)
        buffer.seek(0)
        
        return Response(buffer.getvalue(), media_type="image/png")    
    else:
        raise HTTPException(status_code=404, detail="No se encontraron sesiones")

@app.get("/times-progress/{player_id}")
def get_times_progress(player_id: int, begin_date: str, end_date: str, mode: str, level: str):
    """Función que crea el gráfico de progreso de tiempo de duración de sesiones."""
    sessions = get_sessions(player_id, mode, level, begin_date, end_date)
   
    times = []
    session_dates = []
    
    if sessions:
        for session in sessions:
            data = get_session_data(session.date)
                        
            times.append(float(data.session_time))
            session_dates.append(session.date)
                  
        fig, ax = pyplot.subplots()
        
        ax.plot(session_dates, times, label='Times', color='blue', marker='o', linestyle='-')
        ax.set_xticks(session_dates)
        ax.set_xticklabels(session_dates, rotation=45, ha='right')
        ax.set_ylim(0, max(times)+1)
        ax.set_ylabel('Duration of each session')
        ax.set_xlabel('Sessions Dates')
        ax.legend()
        
        fig.tight_layout()

        buffer = BytesIO()
        fig.savefig(buffer, format='png', bbox_inches='tight')
        pyplot.close(fig)
        buffer.seek(0)
            
        return Response(buffer.getvalue(), media_type="image/png")
    else:
        raise HTTPException(status_code=404, detail="No se encontraron sesiones")
  
@app.get("/lights-progress/{player_id}")
def get_saves_progress(player_id: int, begin_date: str, end_date: str, mode: str, level: str):
    """Función que crea el gráfico de progreso de luces tocadas de varias sesiones."""
    sessions = get_sessions(player_id, mode, level, begin_date, end_date)
   
    i = 0
    lights = []
    session_dates = []
    
    if sessions:
        for session in sessions:
            data = get_session_data(session.date)
            lights.append(data.n_lights)
            session_dates.append(session.date)
            i += 1
        
        fig, ax = pyplot.subplots()
        
        ax.plot(session_dates, lights, label='Saves', color='blue', marker='o', linestyle='-')
        ax.set_ylim(0, max(lights)+1) 
        ax.set_ylabel('Nº of touched lights')
        ax.set_xlabel('Sessions Dates')
        ax.set_xticks(session_dates)
        ax.set_xticklabels(session_dates, rotation=45, ha='right')
        ax.legend()
        
        fig.tight_layout()

        buffer = BytesIO()
        fig.savefig(buffer, format='png', bbox_inches='tight')
        pyplot.close(fig)
        buffer.seek(0)
            
        return Response(buffer.getvalue(), media_type="image/png")
    else:
        raise HTTPException(status_code=404, detail="No se encontraron sesiones")
  
@app.get("/barchart-comparison/{session_date1}/{session_date2}")
def get_barchart_comparison(session_date1: str, session_date2: str):
    """Función que crea el gráfico de barras comparativo de lanzamientos de dos sesiones."""
    sessionP1 = get_session_data(session_date1)
    sessionP2 = get_session_data(session_date2)
    
    labels = ['Player 1', 'Player 2']
    x = np.arange(len(labels))
    width = 0.15
    fig, ax = pyplot.subplots(figsize=(6, 4))
    
    ax.bar(x[0]-width/2, sessionP1.n_saves, width, label='Saves', color='blue')
    ax.bar(x[0]+width/2, sessionP1.n_goals, width, label='Goals', color='red')

    ax.bar(x[1]-width/2, sessionP2.n_saves, width, color='blue')
    ax.bar(x[1]+width/2, sessionP2.n_goals, width, color='red')
    
    ax.set_ylabel('Nº of shoots')
    ax.set_title('Total nº of goals & saves')
    ax.set_xticks(x)
    ax.set_xticklabels(labels, ha='right')
    ax.yaxis.set_major_locator(MaxNLocator(integer=True))
    ax.legend()
    
    fig.tight_layout()
    
    buffer = BytesIO()
    fig.savefig(buffer, format='png', bbox_inches='tight')
    pyplot.close(fig)
    buffer.seek(0)
    
    return Response(buffer.getvalue(), media_type="image/png")

@app.get("/reaction-speed/{session_date}")
def get_reaction_speed(session_date: str):
    """Función que crea el gráfico de líneas de velocidad de reacción de una sesión de luces."""
    session_data = get_session_data(session_date)
    session_reaction = get_session_reaction(session_date)
    
    if len(session_reaction) > 0:
        lights = session_data.n_lights
        encendido = []
        tocado = []
        for frame in session_reaction:
            if frame.status == "tocado":
                tocado.append(frame.Time)
            if frame.status == "encendido":
                encendido.append(frame.Time)
        
        reaction_speed = []
        for i in range(lights): # la última no se cuenta, porque es la que se queda encendida
            reaction_speed.append(tocado[i] - encendido[i])
        
        fig, ax = pyplot.subplots()
        
        ax.plot(range(1, lights+1), reaction_speed, label='Time between light was turned on and touched', color='blue', marker='o', linestyle='-')
        ax.set_ylim(0, max(reaction_speed)+1)
        ax.set_ylabel('Reaction Speed')
    # ax.set_xticks(range(1, lights+1))
        ax.xaxis.set_major_locator(MaxNLocator(integer=True))
        ax.set_xlabel('Nº of touched lights')
        ax.legend()
        
        fig.tight_layout()

        buffer = BytesIO()
        fig.savefig(buffer, format='png', bbox_inches='tight')
        pyplot.close(fig)
        buffer.seek(0)
            
        return Response(buffer.getvalue(), media_type="image/png")

def shutdown():
    sys.exit(0)

signal.signal(signal.SIGTERM, shutdown)

@app.post("/upload_csv")
async def upload_csv(file: UploadFile = File(...), userId: str = Query(...)):
    
    if not file.filename.endswith(".csv"):
       return {"error": "Invalid file type. Only CSV files are allowed."}

    user_folder = os.path.join(UPLOAD_FOLDER, userId)
    os.makedirs(user_folder, exist_ok=True)

    file_path = os.path.join(user_folder, file.filename)
    with open(file_path, "wb") as buffer:
        shutil.copyfileobj(file.file, buffer)
    
    #Sending csv to DB
    load_csv_to_mysql(file_path)

    return JSONResponse(
        content={"message": "File uploaded successfully"},
        status_code=200  
    )
if __name__ == "__main__":
    Base.metadata.create_all(bind=engine)
    uvicorn.run(app, host="0.0.0.0", port=5000)    uvicorn.run(app, host="0.0.0.0", port=12345)