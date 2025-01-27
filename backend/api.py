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
from matplotlib.colors import LinearSegmentedColormap

from roles import Rol

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
    session_time = Column(String)
    n_goals = Column(Integer)
    n_saves = Column(Integer)
    shoots_initial_time = Column(String)
    shoots_final_time = Column(String)
    shoots_initial_point = Column(String)
    shoots_initial_zone = Column(String)
    shoots_final_point = Column(String)
    shoots_final_zone = Column(String)
    shoots_result = Column(String)
    saves_bodypart = Column(String)
    
class SessionTracking(Base):
    __tablename__ = "sessions_tracking"
    id = Column(Integer, primary_key=True, index=True)
    session_id = Column(Integer)
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
    session = SessionLocal()
    user = session.query(User).filter(User.email == request.email).first()
    if user:
        raise HTTPException(status_code=400, detail=f"Email {request.email} already in use.")
    else:
        try: 
            user = User(email=request.email, password=request.password, name=request.name, role=request.user_type, teamID=request.team)
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
            
@app.post("/api/login-staff")
async def login(request: LoginRequest):
    """Función para iniciar sesión del cuerpo técnico."""
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
@app.get("/api/players/{team_id}")
def get_players(team_id: int):
    """Función para obtener la lista de jugadores."""
    session = SessionLocal()
    players = session.query(User).filter(User.role == Rol.PORTERO.value, User.teamID == team_id).all()
    session.close()
    return players

@app.get("/api/user/{email}")
def get_user(email: str):
    """Función para obtener un jugador por email."""
    session = SessionLocal()
    player = session.query(User).filter(User.email == email).first()
    session.close()
    return player

# ------------------- Sessions -------------------
@app.get("/api/sessions/{player_id}")
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

@app.get("/api/team-sessions/{team_id}")
def get_team_sessions(team_id: int):
    """Función para obtener la lista de sesiones de un equipo."""
    session = SessionLocal()
    sessions = session.query(Session).filter(User.teamID == team_id).all()
    session.close()
    return sessions

@app.get("/api/sessionData/{session_id}")
def get_session_data(session_id: int):
    """Función para obtener los datos de una sesión."""
    session = SessionLocal()
    data = session.query(SessionData).filter(SessionData.session_id == session_id).first()
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
    
    shoot_results = session_data.shoots_result.split(", ")
    
    goals = 0
    saves = 0
    
    for result in shoot_results:
        if result == "Gol Recibido":
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
    ax.set_xticks(x, labels, rotation=45, ha='right')
    ax.yaxis.set_major_locator(MaxNLocator(integer=True))
    ax.legend()

    fig.tight_layout()

    buffer = BytesIO()
    fig.savefig(buffer, format='png', bbox_inches='tight')
    pyplot.close(fig)
    buffer.seek(0)
        
    return Response(buffer.getvalue(), media_type="image/png")

@app.get("/api/barchart-saves/{session_id}")
def get_barchart_saves(session_id: int):
    """Función que crea el gráfico de barras de paradas de una sesión."""
    session_data = get_session_data(session_id)
    
    save_bodypart = session_data.saves_bodypart.split(", ")
                
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
    ax.set_xticks(x, labels, rotation=45, ha='right')
    ax.yaxis.set_major_locator(MaxNLocator(integer=True))

    fig.tight_layout()

    buffer = BytesIO()
    fig.savefig(buffer, format='png', bbox_inches='tight')
    pyplot.close(fig)
    buffer.seek(0)
        
    return Response(buffer.getvalue(), media_type="image/png")

@app.get("/api/heatmap/{session_id}")
def get_heatmap(session_id: int):
    """Funcion que crea el heatmap de una sesion"""
    session_data = get_session_data(session_id)

    shoot_results = session_data.shoots_result.split(", ") 
    shoot_zones = session_data.shoots_final_zone.split(", ") 

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
            if result == "Gol Recibido":
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

@app.get("/api/scatterplot-positions/{session_id}")
def get_scatterplot_positions(session_id: int):
    """Función que crea el gráfico de dispersión de posiciones de cabeza y manos en una sesión."""
    session_tracking = get_session_tracking(session_id)
    
    handR_pos_x, handR_pos_y = [], []
    handL_pos_x, handL_pos_y = [], []
    head_pos_x, head_pos_y = [], []

    for frame in session_tracking:
        handR_pos_x.append(frame.handR_pos_x)
        handR_pos_y.append(frame.handR_pos_y)
        handL_pos_x.append(frame.handL_pos_x)
        handL_pos_y.append(frame.handL_pos_y)
        head_pos_x.append(frame.head_pos_x)
        head_pos_y.append(frame.head_pos_y)

    handR_pos_x = np.array(handR_pos_x)
    handR_pos_y = np.array(handR_pos_y)
    handL_pos_x = np.array(handL_pos_x)
    handL_pos_y = np.array(handL_pos_y)
    head_pos_x = np.array(head_pos_x)
    head_pos_y = np.array(head_pos_y)
   
    fig, ax = pyplot.subplots()

    ax.scatter(head_pos_x, head_pos_y, label="Head", color='red', alpha=0.7)
    ax.scatter(handR_pos_x, handR_pos_y, label="Right Hand", color='blue', alpha=0.7)
    ax.scatter(handL_pos_x, handL_pos_y, label="Left Hand", color='green', alpha=0.7)

    ax.set_xlabel('Width')
    ax.set_ylabel('Height')
    ax.set_title('Head and Hands Position')
    ax.legend()    

    buffer = BytesIO()
    fig.savefig(buffer, format='png', bbox_inches='tight')
    pyplot.close(fig)
    buffer.seek(0)
        
    return Response(buffer.getvalue(), media_type="image/png")
    
@app.get("/api/plot-times/{session_id}")
def get_plot_times(session_id: int):
    """Función que crea el gráfico de dispersión de la velocidad de reacción en cada lanzamiento."""
    session_data = get_session_data(session_id)
    
    shoots_initial_time = session_data.shoots_initial_time.split(",")
    shoots_final_time = session_data.shoots_final_time.split(",")

    reaction_times = []
    for initial_time, final_time in zip(shoots_initial_time, shoots_final_time):
        logging.info(f"Initial time: {initial_time}, Final time: {final_time}")
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

@app.get("/api/saves-progress/{player_id}")
def get_saves_progress(player_id: int, begin_date: str, end_date: str, mode: str, level: str):
    """Función que crea el gráfico de progreso de paradas de sesiones."""
    sessions = get_sessions(player_id, mode, level, begin_date, end_date)
   
    i = 0
    saves = []
    session_dates = []
    
    if sessions:
        for session in sessions:
            logging.info(f"Session: {session.id}")
            data = get_session_data(session.id)
            saves.append(data.n_saves)
            session_dates.append(session.date)
            i += 1
        
        fig, ax = pyplot.subplots()
        
        ax.plot(session_dates, saves, label='Saves', color='blue', marker='o', linestyle='-')
        ax.set_ylim(0, max(saves)+1) 
        ax.set_yticks(range(0, max(saves) + 1))  
        ax.set_ylabel('Nº of saves')
        ax.set_xlabel('Sessions Dates')
        ax.set_xticks(ticks=session_dates, labels=session_dates, rotation=45, ha='right')
        ax.legend()
        
        fig.tight_layout()

        buffer = BytesIO()
        fig.savefig(buffer, format='png', bbox_inches='tight')
        pyplot.close(fig)
        buffer.seek(0)
            
        return Response(buffer.getvalue(), media_type="image/png")
    else:
        raise HTTPException(status_code=404, detail="No se encontraron sesiones")

@app.get("/api/heatmap-progress/{player_id}")
def get_heatmap_progress(player_id: int, begin_date: str, end_date: str, mode: str, level: str):
    """Función que crea el heatmap de progreso de sesiones."""
    sessions = get_sessions(player_id, mode, level, begin_date, end_date)

    shoot_results = []
    shoot_zones = []
    
    if sessions:
        for session in sessions:
            data = get_session_data(session.id)
            shoot_results.append(data.shoots_result.split(", "))
            shoot_zones.append(data.shoots_final_zone.split(", "))

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
                    if result == "Gol Recibido":
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

@app.get("/api/times-progress/{player_id}")
def get_times_progress(player_id: int, begin_date: str, end_date: str, mode: str, level: str):
    """Función que crea el gráfico de progreso de tiemop de duración de sesiones."""
    sessions = get_sessions(player_id, mode, level, begin_date, end_date)
   
    times = []
    session_dates = []
    
    if sessions:
        for session in sessions:
            data = get_session_data(session.id)
                        
            times.append(float(data.session_time))
            session_dates.append(session.date)
                  
        fig, ax = pyplot.subplots()
        
        ax.plot(session_dates, times, label='Times', color='blue', marker='o', linestyle='-')
        ax.set_xticks(ticks=session_dates, labels=session_dates, rotation=45, ha='right')
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
    
@app.get("/api/barchart-comparison/{session_id1}/{session_id2}")
def get_barchart_comparison(session_id1: int, session_id2: int):
    """Función que crea el gráfico de barras comparativo de lanzamientos de dos sesiones."""
    sessionP1 = get_session_data(session_id1)
    sessionP2 = get_session_data(session_id2)
    
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
    ax.set_xticks(x, labels, ha='right')
    ax.yaxis.set_major_locator(MaxNLocator(integer=True))
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

if __name__ == "__main__":
    import uvicorn
    Base.metadata.create_all(bind=engine)
    uvicorn.run(app, host="0.0.0.0", port=5000)