# Handball GK Stats Web

Este repositorio contiene una aplicación web desarrollada con React y TypeScrit en el frontend y FastAPI en el backend.

## Instalación
### Frontend

El frontend está desarrollado utilizando **React** y **TypeScript** y utiliza varias librerías para mejorar el diseño de la interfaz de usuario.
1. **Dependencias del proyecto:**

   ```bash
   npm install
   ```

### Backend

El backend está desarrollado utilizando **FastAPI**, una moderna framework para APIs en Python. Este backend utiliza **SQLAlchemy** y **PyMySQL** para interactuar con la base de datos MySQL.

1. **Crear un entorno virtual:**

   ```bash
   python -m venv venv
   ```
2. **Activar el entorno virtual:**
   - En Windows:

     ```bash
     .\venv\Scripts\activate
     ```

   - En MacOS/Linux:

     ```bash
     source venv/bin/activate
     ```
3. **Instalar dependencias:**
   Se instalan desde el archivo generado con todas las dependencias instaladas (mediante `pip freeze > requirements.txt`)

   ```bash
   pip install -r requirements.txt
   ```
---
## Ejecución
- **Iniciar el servidor de desarrollo:**

   ```bash
   npm run dev
   ```

   Este comando lanzará la aplicación en modo de desarrollo utilizando **Vite**. El proyecto se ejecutará en `https://gkstatsweb.duckdns.org:4443`.

- **Iniciar el servidor de backend:**

   ```bash
   pyton api.py
   ```
   Esto iniciará el servidor en `https://gkstatsweb.duckdns.org:12345`.

---

## Estructura del Proyecto

```plaintext
.
├── backend/
│   ├── api.py           # Archivo principal de FastAPI
│   ├── usuario.py        
│   ├── roles.py   
│   └── requirements.txt # Requerimientos del backend
├── frontend/
│   ├── src/
│   │   ├── components/  # Componentes comunes para todas las secciones
│   │   ├── sections/
│   │   └── types/       # _Interface_ de TypeScrpit, similares a los _struct_ de C
│   ├── index.tsx        # Punto de entrada de React
│   ├── vite.config.ts   # Configuración de Vite
│   └── package.json     # Dependencias del frontend
└── README.md            # Este archivo
```

---

## Notas Adicionales

- La base de datos MySQL debe estar en funcionamiento y correctamente configurada antes de interactuar con el backend.
- El archivo `api.py` en el backend contiene la definición de las rutas de la API y los controladores para interactuar con la base de datos.

