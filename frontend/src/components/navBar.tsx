import {
  AppBar,
  Toolbar,
  Typography,
  Box,
  Button,
  IconButton,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  useMediaQuery
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import { useTheme } from '@mui/material/styles';
import { useState } from 'react';
import { useLocation } from 'wouter';
import { User } from '../types/user';

const navLinks = [
  { label: 'Home', path: '/home' },
  { label: 'Player section', path: '/player-section' },
  { label: "GKs' Comparison", path: '/players-comparison' },
  { label: 'Streaming', path: '/streaming' },
  { label: 'About', path: '/about' }
];

const Navbar = ({ user }: { user: User | null }): JSX.Element => {
  const [, setLocation] = useLocation();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const [drawerOpen, setDrawerOpen] = useState(false);

  const handleNav = (path: string) => {
    setLocation(path, { state: { mail: user?.email } });
    setDrawerOpen(false);
  };

  return (
    <AppBar position="static" sx={{ backgroundColor: '#004080', color: 'white' }}>
      <Toolbar sx={{ justifyContent: 'space-between' }}>
        <Typography variant="h6">Handball GK Stats Web</Typography>
        {isMobile ? (
          <>
            <IconButton
              color="inherit"
              edge="start"
              onClick={() => setDrawerOpen(true)}
              sx={{ ml: 1 }}
              aria-label="menu"
            >
              <MenuIcon />
            </IconButton>
            <Drawer
              anchor="right"
              open={drawerOpen}
              onClose={() => setDrawerOpen(false)}
              PaperProps={{
                sx: { backgroundColor: '#004080', color: 'white' }
              }}
            >
              <Box sx={{ width: 220, height: '100%', display: 'flex', flexDirection: 'column', justifyContent: 'space-between' }} role="presentation">
                <List>
                  {navLinks.map(link => (
                    <ListItem key={link.path} disablePadding>
                      <ListItemButton
                        onClick={() => handleNav(link.path)}
                        disabled={!user}
                        sx={{ color: 'white' }}
                      >
                        <ListItemText primary={link.label} />
                      </ListItemButton>
                    </ListItem>
                  ))}
                </List>
                <Box sx={{ mb: 2 }}>
                  <Box
                    sx={{
                      border: '1px solid white',
                      borderRadius: 2,
                      mx: 1,
                      bgcolor: 'rgba(255,255,255,0.08)'
                    }}
                  >
                    <ListItemButton
                      onClick={() => setLocation('/')}
                      sx={{ color: 'white' }}
                    >
                      <AccountCircleIcon sx={{ mr: 1, color: 'white' }} />
                      <ListItemText primary="Log Out" />
                    </ListItemButton>
                  </Box>
                </Box>
              </Box>
            </Drawer>
          </>
        ) : (
          <>
            <Box sx={{ flex: 1, display: 'flex', justifyContent: 'center', gap: 2 }}>
              {navLinks.map(link => (
                <Button
                  key={link.path}
                  color="inherit"
                  onClick={() => handleNav(link.path)}
                  disabled={!user}
                >
                  {link.label}
                </Button>
              ))}
            </Box>
            <Button
              variant="outlined"
              color="inherit"
              startIcon={<AccountCircleIcon />}
              onClick={() => setLocation('/')}
            >
              Log Out
            </Button>
          </>
        )}
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;
