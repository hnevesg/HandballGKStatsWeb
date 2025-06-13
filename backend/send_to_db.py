import logging
import pandas as pd
import re
from sqlalchemy import create_engine

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

session_data_header_to_column_mapping = {
    "DATE": "session_date",
    "TIME": "session_time",
    "PARADAS": "n_saves",
    "GOLES": "n_goals",
    "N_LUCES": "n_lights",
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

session_tracking_header_to_column_mapping = {
    "Frame": "Frame",
    "Time": "Time",
    "HeadPosition_x": "HeadPosition_x",
    "HeadPosition_y": "HeadPosition_y",
    "HeadPosition_z": "HeadPosition_z",
    "HeadRotation_x": "HeadRotation_x",
    "HeadRotation_y": "HeadRotation_y",
    "HeadRotation_z": "HeadRotation_z",
    "RHandPosition_x": "RHandPosition_x",
    "RHandPosition_y": "RHandPosition_y",
    "RHandPosition_z": "RHandPosition_z",
    "RHandRotation_x": "RHandRotation_x",
    "RHandRotation_y": "RHandRotation_y",
    "RHandRotation_z": "RHandRotation_z",
    "RHandVelocity_x": "RHandVelocity_x",
    "RHandVelocity_y": "RHandVelocity_y",
    "RHandVelocity_z": "RHandVelocity_z",
    "LHandPosition_x": "LHandPosition_x",
    "LHandPosition_y": "LHandPosition_y",
    "LHandPosition_z": "LHandPosition_z",
    "LHandRotation_x": "LHandRotation_x",
    "LHandRotation_y": "LHandRotation_y",
    "LHandRotation_z": "LHandRotation_z",
    "LHandVelocity_x": "LHandVelocity_x",
    "LHandVelocity_y": "LHandVelocity_y",
    "LHandVelocity_z": "LHandVelocity_z",
    "session_date": "session_date"
}

session_reaction_header_to_column_mapping = {
    "Frame": "Frame",
    "Tiempo": "Time",
    "ID_Luz": "light_id",
    "Estatus": "status",
    "session_date": "session_date"
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

def process_date(file_data, file_path):
    match = re.search(r'(\d{8})_(\d{6})', file_path)
    if match:
        date_str = match.group(1)
        time_str = match.group(2)
        formatted_datetime = f"{date_str[:4]}-{date_str[4:6]}-{date_str[6:]} {time_str[:2]}:{time_str[2:4]}:{time_str[4:]}"
        file_data['session_date'] = formatted_datetime
    return file_data

def load_csv_to_mysql(file_path):
    try:
        engine = create_engine(f"mysql+pymysql://{DB_CONFIG['user']}:{DB_CONFIG['password']}@{DB_CONFIG['host']}/{DB_CONFIG['database']}")

        file_data = pd.read_csv(file_path, delimiter=';', header=0, index_col=False)
        table_name = ''
        
        if "Historical" in file_path:
            table_name = "sessions"
            session_data = file_data
            session_data.rename(columns=session_header_to_column_mapping, inplace=True)
            session_data = session_data[session_data['player_id'] != 'USER']
            # Drop unmapped columns
            session_data = session_data[list(session_header_to_column_mapping.values())]        
            session_data = session_data.tail(1)
            
            session_data.to_sql(table_name, engine, if_exists='append', index=False)
            logging.info(f"Data from {file_path} has been saved to {table_name} table.")
            
            table_name = "sessions_data"
            file_data = pd.read_csv(file_path, delimiter=';', header=0, index_col=False)
            file_data.rename(columns=session_data_header_to_column_mapping, inplace=True)
            file_data = file_data[file_data['session_date'] != 'DATE']
            file_data = file_data[list(session_data_header_to_column_mapping.values())]                        
            file_data = file_data.tail(1)
                        
            file_data.to_sql(table_name, engine, if_exists='append', index=False)
            logging.info(f"Data from {file_path} has been saved to {table_name} table.")
                
        elif "OculusTracking" in file_path:
            table_name = "sessions_tracking"
            file_data.rename(columns=session_tracking_header_to_column_mapping, inplace=True)
            file_data = process_date(file_data, file_path)        
            file_data = file_data[list(session_tracking_header_to_column_mapping.values())]

            file_data.to_sql(table_name, engine, if_exists='append', index=False)
            logging.info(f"Data from {file_path} has been saved to {table_name} table.")
            
        elif "ReactionSpeed" in file_path:
            table_name = "sessions_reaction"
            file_data.rename(columns=session_reaction_header_to_column_mapping, inplace=True)
            file_data = process_date(file_data, file_path)        
            file_data = file_data[list(session_reaction_header_to_column_mapping.values())]

            file_data.to_sql(table_name, engine, if_exists='append', index=False)
            logging.info(f"Data from {file_path} has been saved to {table_name} table.")        
        
        return 200
    
    except Exception as e:
        logging.error(f"Error: {e}\n .While loading data from {file_path} to {table_name} table.")
        return 500

if __name__ == "__main__":
    load_csv_to_mysql("ReactionSpeed_20250331_212706.csv")