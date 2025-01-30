import re
import pandas as pd
from sqlalchemy import create_engine, Table, Column, Integer, Float, String, MetaData

DB_CONFIG = {
    'host': 'localhost',
    'user': 'root',
    'password': 'helena',
    'database': 'gk_stats_web'
}

session_header_to_column_mapping = {
    "USER": "player_id",
    "DATE": "date",
    "LEVEL": "prestige_level",
    "MODE":  "game_mode",
}

session_ball_header_to_column_mapping = {
    "Frame": "Frame",
    "Tiempo": "Tiempo",
    "ID": "ID",
    "Estatus": "Estatus",
    "Posicion_Bola_x": "Posicion_Bola_x",
    "Posicion_Bola_y": "Posicion_Bola_y",
    "Posicion_Bola_z": "Posicion_Bola_z"
}

session_data_header_to_column_mapping = {
    "DATE": "session_date",
    "TIME": "session_time",
    "PARADAS": "n_saves",
    "GOLES": "n_goals",
    "GOLESOBJETIVO": "target_goals",
    "PUNTO_INICIAL_BOLA": "shoots_initial_point",
    "ID_PUNTO_INICIAL_BOLA": "shoots_initial_zone",
    "PUNTO_FINAL_BOLA": "shoots_final_point",
    "ID_PUNTO_FINAL_BOLA": "shoots_final_zone",
    "PARADA_BOLA": "shoots_result",
    "MANO_PARADA": "saves_bodypart",
    "TIEMPO_DISPARO": "shoots_initial_time",
    "TIEMPO_PARADA_O_GOL": "shoots_final_time"
}    

def process_tracking_date(file_data, file_path):
    match = re.search(r'(\d{8})_(\d{6})', file_path)
    if match:
        date_str = match.group(1)
        time_str = match.group(2)
        formatted_datetime = f"{date_str[:4]}-{date_str[4:6]}-{date_str[6:]} {time_str[:2]}:{time_str[2:4]}:{time_str[4:]}"
        file_data['session_date'] = formatted_datetime
    return file_data

def load_csv_to_mysql(file_path, table_name):
    engine = create_engine(f"mysql+pymysql://{DB_CONFIG['user']}:{DB_CONFIG['password']}@{DB_CONFIG['host']}/{DB_CONFIG['database']}")

    file_data = pd.read_csv(file_path, delimiter=';', header=0)
    
    header_rename = ''
    
    if table_name == 'sessions':
        file_data.rename(columns=session_header_to_column_mapping, inplace=True)
        file_data = file_data[file_data['player_id'] != 'USER']
        #Drop unmapped columns
        file_data = file_data[list(session_header_to_column_mapping.values())]
    #elif table_name == 'session_ball':
        #file_data.rename(columns=session_ball_header_to_column_mapping, inplace=True)
        #file_data = file_data[list(session_ball_header_to_column_mapping.values())]
    elif table_name == 'sessions_data':
        file_data.rename(columns=session_data_header_to_column_mapping, inplace=True)
        file_data = file_data[file_data['session_date'] != 'DATE']
        file_data = file_data[list(session_data_header_to_column_mapping.values())]
    elif table_name == 'sessions_tracking':
        file_data = process_tracking_date(file_data, file_path)
        file_data = file_data.drop(columns=["HandDetectedR", "HighConfidenceR", "HandDetectedL", "HighConfidenceL", "RWristTwist", "LWristTwist"])
    else:
        raise ValueError(f"Unknown table name: {table_name}")

    file_data.to_sql(table_name, engine, if_exists='append', index=False)
    print(f"Data from {file_path} has been saved to {table_name} table.")
