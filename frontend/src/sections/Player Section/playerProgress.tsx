import {
  Box, Container, Typography, TextField, Select, MenuItem,
  Avatar, IconButton, FormControl, Button,
  Grid,
  Paper
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
  const [progressTimesGraphURL, setProgressTimesGraphURL] = useState<string | null>(null);
  const [noSessions, setNoSessions] = useState(false);
  const [loggedUser, setLoggedUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const state = window.history.state;
    if (state?.player) {
      setPlayer(state.player);
    }
    if (state?.user) {
      setLoggedUser(state.user)
    }
  }, []);

  const handleStatistics = async () => {
    if (!player) return;

    const timestamp = new Date().getTime();

    let progressSavesGraphUrl = `http://localhost:8000/api/saves-progress/${player?.id}?begin_date=${beginDate}&end_date=${endDate}&mode=${mode}&level=${level}&t=${timestamp}`;
    let heatmapUrl = `http://localhost:8000/api/heatmap-progress/${player?.id}?begin_date=${beginDate}&end_date=${endDate}&mode=${mode}&level=${level}&t=${timestamp}`;
    let progressTimesGraphUrl = `http://localhost:8000/api/times-progress/${player?.id}?begin_date=${beginDate}&end_date=${endDate}&mode=${mode}&level=${level}&t=${timestamp}`;

    try {
      let savesResponse = await fetch(progressSavesGraphUrl);
      let heatmapResponse = await fetch(heatmapUrl);

      if (!savesResponse.ok || !heatmapResponse.ok) {
        setNoSessions(true);
        setProgressSavesGraphURL(null);
        setHeatmapURL(null);
        setProgressTimesGraphURL(null);
        return;
      } else {
        setNoSessions(false);
        setProgressSavesGraphURL(progressSavesGraphUrl);
        setHeatmapURL(heatmapUrl);
        setProgressTimesGraphURL(progressTimesGraphUrl);
      }
    } catch (error) {
      console.error("Error fetching session data", error);
      setNoSessions(true);
      setProgressSavesGraphURL(null);
      setHeatmapURL(null);
      setProgressTimesGraphURL(null);
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box>
      <Navbar user={loggedUser} />
      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 6 }}>
          <IconButton
            onClick={() => navigate('/player-section', {
              state: { mail: loggedUser?.email }
            })}
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


        {noSessions ? (
          <Typography variant="body1" align='center' color="error" sx={{
            backgroundColor: '#fce4ec',
            border: '1px solid #f8bbd0',
            borderRadius: '8px',
            padding: '12px 16px',
            marginBottom: '24px',
            fontWeight: 600,
            boxShadow: '0px 4px 6px rgba(0, 0, 0, 0.1)',
          }}>No sessions were found for the selected date range.</Typography>
        ) : loading ? (
          <Typography variant="body2" align="center" color="textSecondary">Select a range of dates...</Typography>
        ) : (
          <Grid container spacing={4}>
            <Grid item xs={12} md={9}>
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 4, justifyContent: 'center', width: '100%' }}>

                <Box sx={{ display: 'flex', flexDirection: 'row', gap: 4, width: '135%' }}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                    <Paper sx={{ p: 2, flex: 1, width: '100%' }}>
                      <Typography variant="h6" gutterBottom>Metric 1</Typography>
                      <Box sx={{
                        width: '100%',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center'
                      }}>
                        {progressSavesGraphURL ? (
                          <img src={progressSavesGraphURL} alt="Progress Graph" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                        ) : (
                          <Typography variant="body2" color="textSecondary">
                            Loading progress graph...
                          </Typography>
                        )}
                      </Box>
                    </Paper>
                  </Box>

                  <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                    <Paper sx={{ p: 2, width: '100%' }}>
                      <Typography variant="h6" gutterBottom>Metric 2</Typography>
                      <Box sx={{
                        width: '100%',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center'
                      }}>
                        {progressTimesGraphURL ? (
                          <img src={progressTimesGraphURL} alt="Sessions duration progression" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                        ) : (
                          <Typography variant="body2" color="textSecondary">Loading times progress...</Typography>
                        )}
                      </Box>
                    </Paper>
                  </Box>
                </Box>

                <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                  <Paper sx={{ p: 2, width: '100%' }}>
                    <Typography variant="h6" gutterBottom>Metric 3</Typography>
                    <Box sx={{
                      position: 'relative',
                      backgroundSize: 'contain',
                      paddingBottom: '56.25%', // 16:9 aspect ratio
                      backgroundPosition: 'center',
                      backgroundRepeat: 'no-repeat',
                      alignItems: 'center',
                      justifyContent: 'center',
                      overflow: 'hidden',
                    }}>
                      <img src="/porteria.png" alt="Portería" style={{ position: 'absolute', width: "100%", height: '86%', objectFit: 'contain', zIndex: 1 }} />
                      {heatmapURL ? (
                        <img src={heatmapURL} alt="Heat Map" style={{ position: 'absolute', width: '100%', height: '67.8%', top: '5%', objectFit: 'contain', zIndex: 2 }} />
                      ) : (
                        <Typography variant="body2" color="textSecondary">Loading heat map...</Typography>
                      )}
                    </Box>
                  </Paper>
                </Box>
              </Box>
            </Grid>
          </Grid>
        )
        }
      </Container >
    </Box >
  );
};

export default PlayerProgress;
