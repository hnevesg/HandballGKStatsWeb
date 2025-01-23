import {
  Box, Container, Typography, TextField, Select, MenuItem,
  Avatar, IconButton, FormControl, Button
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useEffect, useState } from 'react';
import { useLocation } from 'wouter';
import Navbar from '../../components/navBar';
import { User } from '../../types/user';

const PlayerProgress = (): JSX.Element => {
  const [, navigate] = useLocation();
  const [beginDate, setBeginDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [mode, setMode] = useState('Default');
  const [level, setLevel] = useState('Beginner');
  const [player, setPlayer] = useState<User | null>(null);
  const [progressSavesGraphURL, setProgressSavesGraphURL] = useState<string | null>(null);
  const [heatmapURL, setHeatmapURL] = useState<string | null>(null);
  const [showMetrics, setShowMetrics] = useState(false);

  useEffect(() => {
    const state = window.history.state;
    if (state?.player) {
      setPlayer(state.player);
    }
  }, []);

  const getSessionsProgress = () => {
    if (!player) return;

    const timestamp = new Date().getTime();

    let progressSavesGraphUrl = `http://localhost:8000/api/saves-progress/${player?.id}?begin_date=${beginDate}&end_date=${endDate}&mode=${mode}&level=${level}&t=${timestamp}`;
    setProgressSavesGraphURL(progressSavesGraphUrl);

    let heatmapUrl = `http://localhost:8000/api/heatmap-progress/${player?.id}?begin_date=${beginDate}&end_date=${endDate}&mode=${mode}&level=${level}&t=${timestamp}`;
    setHeatmapURL(heatmapUrl)
  }

  const handleStatistics = () => {
    getSessionsProgress();
    setShowMetrics(true);
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
          <Typography variant="h4">Personal Progress</Typography>
        </Box>

        <Box sx={{
          display: 'flex',
          alignItems: 'center',
          mb: 4,
          gap: 4
        }}>
          <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <Avatar sx={{ width: 60, height: 60, bgcolor: '#00CED1' }} />
            <Typography variant="h6" sx={{ textAlign: 'center', display: '-webkit-box' }}>{player?.name} </Typography>
          </Box>
          <Box>
            <Typography variant="subtitle2" gutterBottom>Begin Date</Typography>
            <TextField
              type="date"
              value={beginDate}
              onChange={(e) => setBeginDate(e.target.value)}
              sx={{ width: 200 }}
            />
          </Box>

          <Box>
            <Typography variant="subtitle2" gutterBottom>End Date</Typography>
            <TextField
              type="date"
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
              sx={{ width: 200 }}
            />
          </Box>

          <Box>
            <Typography variant="subtitle2" gutterBottom>Mode</Typography>
            <FormControl sx={{ width: 200 }}>
              <Select
                value={mode}
                onChange={(e) => setMode(e.target.value)}
              >
                <MenuItem value="Default">Default</MenuItem>
                <MenuItem value="Fixed Position">Fixed Position</MenuItem>
                <MenuItem value="Progressive I">Progressive I</MenuItem>
                <MenuItem value="Progressive II">Progressive II</MenuItem>
              </Select>
            </FormControl>
          </Box>

          <Box>
            <Typography variant="subtitle2" gutterBottom>Difficulty</Typography>
            <FormControl sx={{ width: 200 }}>
              <Select
                value={level}
                onChange={(e) => setLevel(e.target.value)}
              >
                <MenuItem value="Beginner">Beginner</MenuItem>
                <MenuItem value="Intermediate">Intermediate</MenuItem>
                <MenuItem value="Expert">Expert</MenuItem>
              </Select>
            </FormControl>
          </Box>

          <Box>
            <Button variant="contained"
              disabled={!beginDate || !endDate || endDate < beginDate || !mode || !level}
              onClick={() => handleStatistics()}
            >
              Search sessions
            </Button>
          </Box>
        </Box>


        {showMetrics && (
          <Box sx={{
            display: 'grid',
            gridTemplateColumns: 'repeat(3, 1fr)',
            gap: 44
          }}>
            <Box>
              <Typography variant="h6" gutterBottom>Metric 1</Typography>
              <Box sx={{
                height: '500%',
                width: '150%',
                border: '1px solid #e0e0e0',
                borderRadius: 1,
                p: 2
              }}>
                {progressSavesGraphURL ? (
                  <img src={progressSavesGraphURL} alt="Progress Graph" style={{ position: 'absolute', width: '30%', objectFit: 'contain' }} />
                ) : (
                  <Typography variant="body2" color="textSecondary">
                    Loading progress graph...
                  </Typography>
                )}
              </Box>
            </Box>

            <Box>
              <Typography variant="h6" gutterBottom>Metric 2</Typography>
              <Box sx={{
                height: '500%',
                width: '150%',
                border: '1px solid #e0e0e0',
                borderRadius: 1,
                gap: 4,
                p: 2
              }}>
                <img src="/porteria.png" alt="Portería" style={{ position: 'absolute', width: "22%", height: '20%', objectFit: 'contain', zIndex: 1 }} />
                {heatmapURL ? (
                  <img src={heatmapURL} alt="Heat Map" style={{ position: 'absolute', width: '23.5%', height: '60', top: '27%', right: '29.2%', objectFit: 'contain', zIndex: 2 }} />
                ) : (
                  <Typography variant="body2" color="textSecondary">Loading heat map...</Typography>
                )}
              </Box>
            </Box>
          </Box>
        )}
      </Container>
    </Box>
  );
};

export default PlayerProgress;
