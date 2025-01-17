import {
  Box, Container, Typography, Select, MenuItem, Avatar, IconButton, Table, TableBody,
  TableCell, TableContainer, TableHead, TableRow, Paper, Button
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useEffect, useState } from 'react';
import { useLocation } from 'wouter';
import { Player } from '../../types/player';
import Navbar from '../../components/navBar';

interface Session {
  id: number;
  fecha: string;
  modo: string;
}

const PlayersSessions = (): JSX.Element => {
  const [, navigate] = useLocation();
  const [player1, setPlayer1] = useState<Player | null>(null);
  const [player2, setPlayer2] = useState<Player | null>(null);
  const [mode, setMode] = useState('Principiante');
  const [player1SelectedSession, setPlayer1SelectedSession] = useState<number | null>(null);
  const [player2SelectedSession, setPlayer2SelectedSession] = useState<number | null>(null);

  // Mock sessions data - replace with actual data
  const player1Sessions: Session[] = [
    { id: 1, fecha: '22/11/23', modo: 'Experto' },
  ];

  const player2Sessions: Session[] = [
    { id: 1, fecha: '12/12/23', modo: 'Experto' },
    { id: 2, fecha: '21/10/23', modo: 'Experto' },
  ];

  useEffect(() => {
    const state = window.history.state;
    if (state?.player1) {
      setPlayer1(state.player1);
    }
    if (state?.player2) {
      setPlayer2(state.player2);
    }
  }, []);

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
          mb: 4
        }}>
          <Box sx={{ width: 200 }}>
            <Typography variant="subtitle2" gutterBottom>Modo</Typography>
            <Select
              fullWidth
              value={mode}
              onChange={(e) => setMode(e.target.value)}
            >
              <MenuItem value="Principiante">Principiante</MenuItem>
              <MenuItem value="Intermedio">Intermedio</MenuItem>
              <MenuItem value="Experto">Experto</MenuItem>
              <MenuItem value="Posición Fija">Posición Fija</MenuItem>
              <MenuItem value="Progressivo I">Progresivo I</MenuItem>
              <MenuItem value="Progressivo II">Progresivo II</MenuItem>
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
                    <TableCell>Fecha</TableCell>
                    <TableCell>Modo</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {player1Sessions.map((session) => (
                    <TableRow
                      key={session.id}
                      onClick={() => { setPlayer1SelectedSession(session.id); }}
                      sx={{
                        cursor: 'pointer',
                        backgroundColor: player1SelectedSession === session.id ? 'rgba(96, 93, 93, 0.85)' : 'inherit',
                        '&:hover': {
                          backgroundColor: player1SelectedSession === session.id ? 'rgba(96, 93, 93, 0.85)' : 'inherit',
                        }
                      }}
                    >
                      <TableCell>{session.fecha}</TableCell>
                      <TableCell>{session.modo}</TableCell>
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
                    <TableCell>Fecha</TableCell>
                    <TableCell>Modo</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {player2Sessions.map((session) => (
                    <TableRow
                      key={session.id}
                      onClick={() => { setPlayer2SelectedSession(session.id); }}
                      sx={{
                        cursor: 'pointer',
                        backgroundColor: player2SelectedSession === session.id ? 'rgba(96, 93, 93, 0.85)' : 'inherit',
                        '&:hover': {
                          backgroundColor: player2SelectedSession === session.id ? 'rgba(96, 93, 93, 0.85)' : 'inherit',
                        }
                      }}
                    >
                      <TableCell>{session.fecha}</TableCell>
                      <TableCell>{session.modo}</TableCell>
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
            disabled={!player1SelectedSession || !player2SelectedSession || !mode}
            onClick={() => handleSessionsSelect()}
          >
            Comparar
          </Button>
        </Box>
      </Container>
    </Box>
  );
};

export default PlayersSessions;
