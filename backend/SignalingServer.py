from fastapi import WebSocket, WebSocketDisconnect
import logging
import uuid

class SignalingServer:
    def __init__(self):
        self.active_connections: list[WebSocket] = []
        self.clients_by_id: dict[str, WebSocket] = {}
        self.websockets_by_client: dict[WebSocket, str] = {}

    async def connect(self, websocket: WebSocket, role: None):
        await websocket.accept()
        self.active_connections.append(websocket)
        
        if role == "sender":
            client_id = "1"
        else:
            client_id = str(uuid.uuid4())

        self.clients_by_id[client_id] = websocket
        self.websockets_by_client[websocket] = client_id

        await websocket.send_json({
            "type": "welcome",
            "senderId": client_id
        })
        
        logging.info(f"Client connected with id {client_id}")


    def disconnect(self, websocket: WebSocket):
        self.active_connections.remove(websocket)
        
        logging.warning(f"Disconnecting client {self.websockets_by_client.get(websocket)}")
        
        client_id = self.websockets_by_client.get(websocket)
        if client_id:
            del self.clients_by_id[client_id]
            del self.websockets_by_client[websocket]
        
    async def send_personal_message(self, message: dict):
        target_id = str(message.get("targetId"))
        target_socket = self.clients_by_id.get(target_id)

        if target_socket:
            await target_socket.send_json(message)
        else:
            logging.error(f"Target client {target_id} not connected.")
                        
    async def broadcast(self, message: dict, websocket: WebSocket):
        for connection in self.active_connections:
            if connection != websocket:
                await connection.send_json(message)
