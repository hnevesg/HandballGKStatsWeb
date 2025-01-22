import {
  Box, Container, Typography, TextField, Select, MenuItem,
  Avatar, IconButton, FormControl, Button
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useEffect, useState } from 'react';
import { useLocation } from 'wouter';
import Navbar from '../../components/navBar';
import { Player } from '../../types/player';
import { Session } from '../../types/session';

const PlayerProgress = (): JSX.Element => {
  const [, navigate] = useLocation();
  const [beginDate, setBeginDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [mode, setMode] = useState('Normal');
  const [level, setLevel] = useState('Principiante');
  const [player, setPlayer] = useState<Player | null>(null);
  const [showMetrics, setShowMetrics] = useState(false);
  const [progressSavesGraphURL, setProgressSavesGraphURL] = useState<any>(null);

  useEffect(() => {
    const state = window.history.state;
    if (state?.player) {
      setPlayer(state.player);
    }
  }, []);

  const getSessionsProgress = () => {
    const timestamp = new Date().getTime(); 

    let progressSavesGraphUrl = `http://localhost:8000/api/sessions-saves-progress/${player?.id}?begin_date=${beginDate}&end_date=${endDate}&mode=${mode}&level=${level}&t=${timestamp}`;
    setProgressSavesGraphURL(progressSavesGraphUrl);
 }

  const handleStatistics = () => {
    getSessionsProgress();
    //setShowMetrics(true);
  }

  return (
    <Box>
      <Navbar />
      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 6 }}>
          <IconButton
            onClick={() => navigate('/player-section')}
            sx={{ mr: 2 }}
          >
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h4">Progreso Personal</Typography>
        </Box>

        <Box sx={{
          display: 'flex',
          alignItems: 'center',
          mb: 4,
          gap: 4
        }}>
          <Avatar sx={{ width: 60, height: 60, bgcolor: '#00CED1' }} />
          <Typography variant="h6">{player?.name}</Typography>
          <Box>
            <Typography variant="subtitle2" gutterBottom>Fecha de inicio</Typography>
            <TextField
              type="date"
              value={beginDate}
              onChange={(e) => setBeginDate(e.target.value)}
              sx={{ width: 200 }}
            />
          </Box>

          <Box>
            <Typography variant="subtitle2" gutterBottom>Fecha de fin</Typography>
            <TextField
              type="date"
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
              sx={{ width: 200 }}
            />
          </Box>

          <Box>
            <Typography variant="subtitle2" gutterBottom>Modo</Typography>
            <FormControl sx={{ width: 200 }}>
              <Select
                value={mode}
                onChange={(e) => setMode(e.target.value)}
              >
                <MenuItem value="Normal">Normal</MenuItem>
                <MenuItem value="Posición Fija">Posición Fija</MenuItem>
                <MenuItem value="Progressivo I">Progresivo I</MenuItem>
                <MenuItem value="Progressivo II">Progresivo II</MenuItem>
              </Select>
            </FormControl>
          </Box>

          <Box>
            <Typography variant="subtitle2" gutterBottom>Nivel de dificultad</Typography>
            <FormControl sx={{ width: 200 }}>
              <Select
                value={level}
                onChange={(e) => setLevel(e.target.value)}
              >
                <MenuItem value="Principiante">Principiante</MenuItem>
                <MenuItem value="Intermedio">Intermedio</MenuItem>
                <MenuItem value="Experto">Experto</MenuItem>
              </Select>
            </FormControl>
          </Box>

          <Box>
            <Button variant="contained"
              disabled={!beginDate || !endDate || endDate < beginDate || !mode || !level}
              onClick={() => handleStatistics()}
            >
              Buscar sesiones
            </Button>
          </Box>
        </Box>

        {(
          <Box sx={{
            display: 'grid',
            gridTemplateColumns: 'repeat(3, 1fr)',
            gap: 4
          }}>
            <Box>
              <Typography variant="h6" gutterBottom>Métrica 1</Typography>
              <Box sx={{
                height: 300,
                border: '1px solid #e0e0e0',
                borderRadius: 1,
                p: 2
              }}>
                {progressSavesGraphURL && (
                  <img src={progressSavesGraphURL} alt="Progress Graph" />
                )}
              </Box>
            </Box>

            <Box>
              <Typography variant="h6" gutterBottom>Métrica 2</Typography>
              <Box sx={{
                height: 300,
                border: '1px solid #e0e0e0',
                borderRadius: 1,
                p: 2
              }}>
                {/* Chart component would go here */}
              </Box>
            </Box>
          </Box>
        )}
      </Container>
    </Box>
  );
};

export default PlayerProgress;
