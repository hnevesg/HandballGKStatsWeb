import { Box, Container, Typography, Select, MenuItem, IconButton, Button} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useEffect, useState } from 'react';
import { useLocation } from 'wouter';
import { User } from '../../types/user';
import Navbar from '../../components/navBar';
import { Session } from '../../types/session';
import { PlayerTable } from '../../components/utils';
import { baseURL } from '../../components/utils';

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
  const [loggedUser, setLoggedUser] = useState<User | null>(null);

  const getSessionsP1 = async () => {
    if (!player1) return;
    try {
      const response = await fetch(`${baseURL}/sessions/${player1.id}?mode=${mode}&level=${level}`, {
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
      const response = await fetch(`${baseURL}/sessions/${player2.id}?mode=${mode}&level=${level}`, {
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
    if (state?.user) {
      setLoggedUser(state.user)
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
        user: loggedUser
      }
    });
  }

  return (
    <Box>
      <Navbar user={loggedUser} />
      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 6 }}>
          <IconButton
            onClick={() => navigate('/players-comparison', {
              state: { mail: loggedUser?.email }
            })}
            sx={{ mr: 2 }}
          >
            <ArrowBackIcon />
          </IconButton>
            <Typography variant="h4" align="center" sx={{ flexGrow: 1 }}>Players Comparison</Typography>
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
              <MenuItem value="Progressive">Progressive I</MenuItem>
              <MenuItem value="Progressive2">Progressive II</MenuItem>
              <MenuItem value="PerTime">PerTime</MenuItem>
              <MenuItem value="LightsReaction">Lights Reaction I</MenuItem>
              <MenuItem value="LightsReaction2">Lights Reaction II</MenuItem>
            </Select>
          </Box>
        </Box>

        <Box sx={{
          display: 'flex',
          justifyContent: 'space-between',
          gap: 4,
          mb: 4
        }}>
          {/* Player 1 Table */}
          <PlayerTable
            playerName={player1?.name || 'Player 1'}
            playerAvatar="GK1"
            playerSessions={player1Sessions}
            selectedSession={player1SelectedSession}
            setSelectedSession={setPlayer1SelectedSession}
          />
          {/* Player 2 Table */}
          <PlayerTable
            playerName={player2?.name || 'Player 2'}
            playerAvatar="GK2"
            playerSessions={player2Sessions}
            selectedSession={player2SelectedSession}
            setSelectedSession={setPlayer2SelectedSession}
          />
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
