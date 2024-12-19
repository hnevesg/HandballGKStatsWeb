// components/Navbar.tsx
import { 
  AppBar, 
  Toolbar, 
  Typography, 
  Box, 
  Button 
} from '@mui/material';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import { useLocation } from 'wouter';

const Navbar = (): JSX.Element => {
  const [, setLocation] = useLocation();

  return (
    <AppBar position="static" sx={{ backgroundColor: 'white', color: 'black' }}>
      <Toolbar sx={{ justifyContent: 'space-between' }}>
        <Typography variant="h6">Handball GK Stats Web</Typography>
        <Box sx={{ display: 'flex', gap: 2 }}>
          <Button 
            color="inherit"
            onClick={() => setLocation('/home')}
          >
            Home
          </Button>
          <Button 
            color="inherit"
            onClick={() => setLocation('/player-section')}
          >
            Sección de Jugadores
          </Button>
          <Button 
            color="inherit"
            onClick={() => setLocation('/players-comparison')}
          >
            Comparación de Porteros
          </Button>
          <Button 
            color="inherit"
            onClick={() => setLocation('/streaming')}
          >
            Streaming
          </Button>
          <Button 
            color="inherit"
            onClick={() => setLocation('/about')}
          >
            About
          </Button>
        </Box>
        <Button 
          variant="outlined" 
          startIcon={<AccountCircleIcon />}
          onClick={() => setLocation('/')}
        >
          Cerrar Sesión
        </Button>
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;
