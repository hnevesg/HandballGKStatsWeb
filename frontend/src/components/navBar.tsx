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
import { User } from '../types/user';

const Navbar = ({ user }: { user: User | null }): JSX.Element => {
  const [, setLocation] = useLocation();

  return (
    <AppBar position="static" sx={{ backgroundColor: 'white', color: 'black' }}>
      <Toolbar sx={{ justifyContent: 'space-between' }}>
        <Typography variant="h6">Handball GK Stats Web</Typography>
        <Box sx={{ display: 'flex', gap: 2 }}>
          <Button
            color="inherit"
            onClick={() => setLocation('/home', {
              state: { mail: user?.email }
            })}
            disabled={!user}
          >
            Home
          </Button>
          <Button
            color="inherit"
            onClick={() => setLocation('/player-section', {
              state: { mail: user?.email }
            })}
            disabled={!user}
          >
            Player section
          </Button>
          <Button
            color="inherit"
            onClick={() => setLocation('/players-comparison', {
              state: { mail: user?.email }
            })}
            disabled={!user}
          >
            GKs' Comparison
          </Button>
          <Button
            color="inherit"
            onClick={() => setLocation('/streaming', {
              state: { mail: user?.email }
            })}
            disabled={!user}
          >
            Streaming
          </Button>
          <Button
            color="inherit"
            onClick={() => setLocation('/about', {
              state: { mail: user?.email }
            })}
            disabled={!user}
          >
            About
          </Button>
        </Box>
        <Button
          variant="outlined"
          startIcon={<AccountCircleIcon />}
          onClick={() => setLocation('/')}
        >
          Log Out
        </Button>
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;
