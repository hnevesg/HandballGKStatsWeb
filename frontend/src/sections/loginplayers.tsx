import { JSX, useState } from 'react';
import {
  Button,
  Box,
  Avatar,
  Typography,
  Container,
  Paper,
  IconButton
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import SportsHandballIcon from '@mui/icons-material/SportsHandball';
import { useLocation } from 'wouter';
import { TextInput } from '@mantine/core';

const LoginPlayers = (): JSX.Element => {
  const [email, setEmail] = useState('');
  const [emailError, setEmailError] = useState('');
  const [passwordError, setPasswordError] = useState('');
  const [password, setPassword] = useState('');
  const [loginError, setLoginError] = useState(''); 
  const [, setLocation] = useLocation();

  const handleLogin = async () => {
    setEmailError('');
    setPasswordError('');
    setLoginError('');

    if (email === '') {
      setEmailError('Email is required');
    }

    if (password === '') {
      setPasswordError('Password is required');
    }

    if (email && password) {
      //call backend api to login
      fetchData()
    }
  };

  const fetchData = async () => {
    try {
      const response = await fetch('http://localhost:8000/api/login-players', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: 
          JSON.stringify({ email, password })
      });
      const data = await response.json();
      if (response.ok) {
        setLocation('/home');
      }else{
        setLoginError(data.detail || 'Login failed'); 
      }
    }
    catch (error) {
      console.error('Error:', error);
    }
  }

  return (
    <Container maxWidth="sm">
      <Paper
        elevation={3}
        sx={{
          mt: 8,
          p: 4,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          backgroundColor: 'rgba(255, 255, 255, 0.9)',
          backdropFilter: 'blur(10px)',
          position: 'relative'
        }}
      >
        <IconButton
          sx={{
            position: 'absolute',
            left: 16,
            top: 16
          }}
          onClick={() => setLocation('/')}
        >
          <ArrowBackIcon />
        </IconButton>

        <Avatar
          sx={{
            m: 1,
            bgcolor: '#00CED1CC',
            width: 56,
            height: 56
          }}
        >
          <SportsHandballIcon />
        </Avatar>

        <Typography component="h1" variant="h5" sx={{ mb: 3 }}>
          Iniciar Sesión
        </Typography>

        <Box component="form" sx={{ mt: 1, width: '100%' }}>
          <TextInput
            required
            label="Correo Electrónico"
            name="email"
            autoComplete="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            error={!!emailError}
            placeholder="you@example.com"
            mb="md"
          />

          <TextInput
            required
            name="password"
            label="Contraseña"
            type="password"
            autoComplete="new-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            error={!!passwordError}
            placeholder="Your password"
            mb="md"
          />

          <Button
            fullWidth
            variant="contained"
            onClick={handleLogin}
            sx={{
              mt: 3,
              mb: 2,
              bgcolor: '#00CED1',
              '&:hover': {
                bgcolor: '#40E0D0'
              }
            }}
          >
            Entrar
          </Button>
          {loginError && <p>{loginError}</p>} 
        </Box>
      </Paper>
    </Container>
  );
};

export default LoginPlayers;
