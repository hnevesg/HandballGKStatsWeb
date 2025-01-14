from roles import Rol

class Usuario:
    def __init__(self, nombre, email, contraseña, equipo, rol: Rol):
        self.nombre = nombre
        self.email = email
        self.contraseña = contraseña
        self.equipo = equipo
        self.rol = rol