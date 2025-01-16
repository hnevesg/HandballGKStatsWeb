# Handball GK Stats Web

Este repositorio contiene una aplicación web desarrollada con React y TypeScrit en el frontend y FastAPI en el backend.

## Instalación
### Frontend

El frontend está desarrollado utilizando **React** y **TypeScript** y utiliza varias librerías para mejorar el diseño de la interfaz de usuario.
1. **Dependencias del proyecto:**

   ```bash
   npm install react
   npm install vite
   npm install @types/react
   npm install @mantine/core @mantine/hooks
   npm install wouter
   npm install @mui/material @mui/styled-engine-sc styled-components @mui/icons-material
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

   Este comando lanzará la aplicación en modo de desarrollo utilizando **Vite**. Por defecto, el proyecto se ejecutará en `http://localhost:5173`.

- **Iniciar el servidor de backend:**

   ```bash
   uvicorn api:app --reload
   ```
   Esto iniciará el servidor en `http://localhost:8000` por defecto. La opción `--reload` permite que el servidor se recargue automáticamente cuando se realicen cambios en el código.

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

