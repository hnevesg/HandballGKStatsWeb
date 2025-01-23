import {
  Box, Container, Typography, Select, MenuItem, Avatar, IconButton, Table, TableBody,
  TableCell, TableContainer, TableHead, TableRow, Paper, Button
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useEffect, useState } from 'react';
import { useLocation } from 'wouter';
import { User } from '../../types/user';
import Navbar from '../../components/navBar';
import { Session } from '../../types/session';

const PlayersSessions = (): JSX.Element => {
  const [, navigate] = useLocation();
  const [player1, setPlayer1] = useState<User>();
  const [player2, setPlayer2] = useState<User>();
  const [mode, setMode] = useState('Default');
  const [level, setLevel] = useState('Beginner');
  const [player1SelectedSession, setPlayer1SelectedSession] = useState<number | null>(null);
  const [player2SelectedSession, setPlayer2SelectedSession] = useState<number | null>(null);
  const [player1Sessions, setPlayer1Sessions] = useState<Session[]>([]);
  const [player2Sessions, setPlayer2Sessions] = useState<Session[]>([]);

  const getSessionsP1 = async () => {
    if (!player1) return;
    try {
      console.log("GetSessionsP1")
      console.log(player1)
      const response = await fetch(`http://localhost:8000/api/sessions/${player1.id}?mode=${mode}&level=${level}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      if (response.ok) {
        const data = await response.json();
        setPlayer1Sessions(data);
      } else {
        console.error('Error fetching sessions for P1');
      }
    } catch (error) {
      console.error('Error fetching sessions for P1', error);
    }
  }

  const getSessionsP2 = async () => {
    if (!player2) return;
    try {
      console.log("GetSessionsP2")
      console.log(player2)
      const response = await fetch(`http://localhost:8000/api/sessions/${player2.id}?mode=${mode}&level=${level}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      if (response.ok) {
        const data = await response.json();
        setPlayer2Sessions(data);
      } else {
        console.error('Error fetching sessions for P2');
      }
    } catch (error) {
      console.error('Error fetching sessions for P2', error);
    }
  }

  useEffect(() => {
    const state = window.history.state;
    if (state?.player1) {
      setPlayer1(state.player1);
    }
    if (state?.player2) {
      setPlayer2(state.player2);
    }
  }, []);

  useEffect(() => {
    getSessionsP1();
    getSessionsP2();
  }, [player1, player2]);

  // Para cuando se cambia el modo o el nivel de dificultad
  useEffect(() => {
    getSessionsP1();
    getSessionsP2();
  }, [mode, level]);


  const handleSessionsSelect = () => {
    navigate("/comparison-details", {
      state: {
        player1: window.history.state.player1,
        player2: window.history.state.player2,
        session1: player1Sessions.find(s => s.id === player1SelectedSession),
        session2: player2Sessions.find(s => s.id === player2SelectedSession),
      }
    });
  }

  return (
    <Box>
      <Navbar />
      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 6 }}>
          <IconButton
            onClick={() => navigate('/players-comparison')}
            sx={{ mr: 2 }}
          >
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h4">Comparación de Jugadores</Typography>
        </Box>

        <Box sx={{
          display: 'flex',
          justifyContent: 'center',
          mb: 4,
          gap: 3
        }}>
          <Box sx={{ width: 200 }}>
            <Typography variant="subtitle2" gutterBottom>Game Mode</Typography>
            <Select
              fullWidth
              value={mode}
              onChange={(e) => setMode(e.target.value)}
            >
              <MenuItem value="Default">Default</MenuItem>
              <MenuItem value="Fixed Position">Fixed Position</MenuItem>
              <MenuItem value="Progressive I">Progressive I</MenuItem>
              <MenuItem value="Progressive II">Progressive II</MenuItem>
            </Select>
          </Box>

          <Box sx={{ width: 200 }}>
            <Typography variant="subtitle2" gutterBottom>Difficulty</Typography>
            <Select
              fullWidth
              value={level}
              onChange={(e) => setLevel(e.target.value)}
            >
              <MenuItem value="Beginner">Beginner</MenuItem>
              <MenuItem value="Intermediate">Intermediate</MenuItem>
              <MenuItem value="Expert">Expert</MenuItem>
            </Select>
          </Box>
        </Box>

        <Box sx={{
          display: 'flex',
          justifyContent: 'space-between',
          gap: 4,
          mb: 4
        }}>
          {/* Player 1 Section */}
          <Box sx={{ flex: 1 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <Avatar sx={{ mr: 2, width: 45, height: 45, bgcolor: '#00CED1' }}>GK1</Avatar>
              <Typography variant="h6">{player1?.name}</Typography>
            </Box>
            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Date</TableCell>
                    <TableCell>Game Mode</TableCell>
                    <TableCell>Difficulty</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {player1Sessions.map((session) => (
                    <TableRow
                      key={session.id}
                      onClick={() => { setPlayer1SelectedSession(session.id); }}
                      sx={{
                        cursor: 'pointer',
                        backgroundColor: player1SelectedSession === session.id ? 'rgb(0, 206, 209)' : 'inherit',
                        '&:hover': {
                          backgroundColor: player1SelectedSession === session.id ? 'rgb(0, 206, 209)' : 'inherit',
                        }
                      }}
                    >
                      <TableCell>{session.date}</TableCell>
                      <TableCell>{session.game_mode}</TableCell>
                      <TableCell>{session.prestige_level}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Box>

          {/* Player 2 Section */}
          <Box sx={{ flex: 1 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <Avatar sx={{ mr: 2, width: 45, height: 45, bgcolor: '#00CED1' }}>GK2</Avatar>
              <Typography variant="h6">{player2?.name}</Typography>
            </Box>
            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Date</TableCell>
                    <TableCell>Game Mode</TableCell>
                    <TableCell>Difficulty</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {player2Sessions.map((session) => (
                    <TableRow
                      key={session.id}
                      onClick={() => { setPlayer2SelectedSession(session.id); }}
                      sx={{
                        cursor: 'pointer',
                        backgroundColor: player2SelectedSession === session.id ? 'rgb(0, 206, 209)' : 'inherit',
                        '&:hover': {
                          backgroundColor: player2SelectedSession === session.id ? 'rgb(0, 206, 209)' : 'inherit',
                        }
                      }}
                    >
                      <TableCell>{session.date}</TableCell>
                      <TableCell>{session.game_mode}</TableCell>
                      <TableCell>{session.prestige_level}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Box>
        </Box>

        <Box sx={{ display: 'flex', justifyContent: 'center' }}>
          <Button
            variant="contained"
            disabled={!player1SelectedSession || !player2SelectedSession || !level || !mode}
            onClick={() => handleSessionsSelect()}
          >
            Comparar
          </Button>
        </Box>
      </Container >
    </Box >
  );
};

export default PlayersSessions;
