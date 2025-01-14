import { useState } from 'react';
import {
  Button,
  Box,
  Avatar,
  Typography,
  Container,
  Paper,
  Radio,
  RadioGroup,
  FormControl,
  FormControlLabel,
  IconButton,
  NativeSelect,
} from '@mui/material';
import SportsSoccerIcon from '@mui/icons-material/SportsSoccer';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useLocation } from 'wouter';
import { TextInput, Title } from '@mantine/core';

const Register = () => {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [emailError, setEmailError] = useState('');
  const [passwordError, setPasswordError] = useState('');
  const [, setLocation] = useLocation();
  const [userType, setUserType] = useState('');
  const [selectedTeam, setSelectedTeam] = useState('1');

  const teams = [
    { label: 'BM Granollers', id: 1 },
    { label: 'FC Barcelona', id: 2 },
    { label: 'Atlético Valladolid', id: 3 }
  ];

  const handleRegister = async () => {
    setEmailError('');
    setPasswordError('');

    if (email === '') {
      setEmailError('Please enter your email');
      return;
    }

    if (password === '') {
      alert('Please enter a password');
      return;
    }

    if (password.length < 7) {
      alert('Password must be 8 characters or longer');
      return;
    }

    if(userType === ''){
      alert('Please select a user type');
      return;
    }

    if (!/^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$/.test(email)) {
      setEmailError('Please enter a valid email address');
      return;
    }

    registerData()
  };

  const registerData = async () => {
    try {
      const response = await fetch('http://localhost:8000/api/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          name,
          email,
          password,
          team: parseInt(selectedTeam, 10),
          user_type: userType
        })
      });
      const data = await response.json();
      if (response.ok) {
        alert('Cuenta creada correctamente');
        setLocation('/home');
      } else {
        //the api returns an http 400 error print the detail
        alert(data.detail || 'Error creating account');
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
        }}
      >
        <IconButton
          sx={{ position: 'absolute', left: 16, top: 16 }}
          onClick={() => setLocation('/')}
        >
          <ArrowBackIcon />
        </IconButton>

        <Avatar
          sx={{
            m: 1,
            bgcolor: '#FFD700CC',
            width: 56,
            height: 56
          }}
        >
          <SportsSoccerIcon />
        </Avatar>
        <Title order={2} mb="md">
          Register
        </Title>

        <Box component="form" sx={{ mt: 1, width: '100%' }}>
          <TextInput
            required
            label="Name"
            name="name"
            autoComplete="name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Your name"
            mb="md"
          />

          <TextInput
            required
            label="Email Address"
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
            label="Password"
            type="password"
            autoComplete="new-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            error={!!passwordError}
            placeholder="Your password"
            mb="md"
          />

          <FormControl fullWidth>
            <Typography
              variant="subtitle1"
              sx={{
                fontSize: '0.9rem',
                fontWeight: 700,
                color: 'rgba(0, 0, 0, 0.87)',
                mb: 1
              }}
            >
              Select your team
            </Typography>
            <NativeSelect
              value={selectedTeam}
              onChange={(e) => setSelectedTeam(e.target.value)}
              fullWidth
              sx={{
                backgroundColor: 'white',
                borderRadius: 1,
                '& select': {
                  padding: '10px 14px',
                },
                '& .MuiNativeSelect-select': {
                  borderColor: '#00CED1'
                },
                '&:hover .MuiNativeSelect-select': {
                  borderColor: '#40E0D0'
                }
              }}
            >
              {teams.map((team) => (
                <option key={team.id} value={team.id.toString()}>
                  {team.label}
                </option>
              ))}
            </NativeSelect>
          </FormControl>

          <Box component="form" sx={{ mt: 1, width: '100%' }}>
            <FormControl component="fieldset" sx={{ mb: 2, width: '100%' }}>
              <RadioGroup
                row
                name="userType"
                value={userType}
                onChange={(e) => setUserType(e.target.value)}
                sx={{
                  display: 'flex',
                  gridTemplateColumns: 'repeat(2, 1fr)',
                  gap: 4,
                  justifyContent: 'center',
                }}
              >
                <FormControlLabel
                  value="entrenador"
                  control={<Radio sx={{ color: '#32CD32' }} />}
                  label="Entrenador@"
                />
                <FormControlLabel
                  value="portero"
                  control={<Radio sx={{ color: '#40E0D0' }} />}
                  label="Jugador@"
                />
              </RadioGroup>
            </FormControl>
          </Box>

          <Button
            fullWidth
            variant="contained"
            onClick={handleRegister}
            sx={{
              mt: 3,
              mb: 2,
              bgcolor: '#FFD700CC',
              '&:hover': {
                bgcolor: '#40E0D0'
              }
            }}
          >
            Register
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default Register;
