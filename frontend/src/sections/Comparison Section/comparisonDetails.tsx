import {
  Box, Container, Typography, Avatar, IconButton, Paper,
  TableContainer, Table, TableCell, TableHead, TableRow, TableBody,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useLocation } from 'wouter';
import Navbar from '../../components/navBar';
import { useEffect, useState } from 'react';
import { User } from '../../types/user';
import { Session } from '../../types/session';
import { SessionTracking } from '../../types/sessionTracking';
import { SessionData } from '../../types/sessionData';

const ComparisonDetails = (): JSX.Element => {
  const [, navigate] = useLocation();
  const [player1, setPlayer1] = useState<User | null>(null);
  const [player2, setPlayer2] = useState<User | null>(null);
  const [session1, setSession1] = useState<Session | null>(null);
  const [session2, setSession2] = useState<Session | null>(null);
  const [barchartURL, setBarchartURL] = useState<any>(null);
  const [heatmapP1URL, setHeatmapP1URL] = useState<any>(null);
  const [heatmapP2URL, setHeatmapP2URL] = useState<any>(null);
  const [sessionTrackingP1, setSessionTrackingP1] = useState<SessionTracking[]>([]);
  const [sessionTrackingP2, setSessionTrackingP2] = useState<SessionTracking[]>([]);
  const [LhandSpeedP1, setLhandSpeedP1] = useState<number>()
  const [RhandSpeedP1, setRhandSpeedP1] = useState<number>()
  const [LhandSpeedP2, setLhandSpeedP2] = useState<number>()
  const [RhandSpeedP2, setRhandSpeedP2] = useState<number>()
  const [loggedUser, setLoggedUser] = useState<User | null>(null);
  const [sessionDataP1, setSessionDataP1] = useState<SessionData | null>();
  const [sessionDataP2, setSessionDataP2] = useState<SessionData | null>();
  const [plotTimesP1URL, setPlotTimesP1URL] = useState<any>(null);
  const [plotTimesP2URL, setPlotTimesP2URL] = useState<any>(null);

  useEffect(() => {
    const state = window.history.state;
    if (state?.player1) {
      setPlayer1(state.player1);
    }
    if (state?.player2) {
      setPlayer2(state.player2);
    }
    if (state?.session1) {
      setSession1(state.session1);
    }
    if (state?.session2) {
      setSession2(state.session2);
    }
    if (state?.user) {
      setLoggedUser(state.user)
    }
  }, []);

  const goBack = () => {
    navigate("/players-sessions", {
      state: { player1, player2, user: loggedUser }
    });
  }

  const getSessionsData = async () => {
    if (!session1 || !session2) return;
    try {
      const response = await fetch(`http://localhost:8000/api/sessionData/${session1.date}`);
      if (response.ok) {
        console.log("Session Tracking P1 ANSWER")
        const dataP1 = await response.json();
        setSessionDataP1(dataP1);
      } else {
        console.error('Error fetching P1 session tracking');
      }

      let responseP2 = await fetch(`http://localhost:8000/api/sessionData/${session2.date}`);
      if (responseP2.ok) {
        console.log("Session Tracking P2 ANSWER")
        const dataP2 = await responseP2.json();
        setSessionDataP2(dataP2);
      } else {
        console.error('Error fetching P2 session tracking');
      }
    } catch (error) {
      console.error('Error fetching session data', error);
    }
  }

  const getSessionsTracking = async () => {
    if (!session1 || !session2) return;
    try {
      const response = await fetch(`http://localhost:8000/api/sessionTracking/${session1.date}`);
      if (response.ok) {
        console.log("Session Tracking P1 ANSWER")
        const dataP1 = await response.json();
        setSessionTrackingP1(dataP1);
      } else {
        console.error('Error fetching P1 session tracking');
      }

      let responseP2 = await fetch(`http://localhost:8000/api/sessionTracking/${session2.date}`);
      if (responseP2.ok) {
        console.log("Session Tracking P2 ANSWER")
        const dataP2 = await responseP2.json();
        setSessionTrackingP2(dataP2);
      } else {
        console.error('Error fetching P2 session tracking');
      }
    } catch (error) {
      console.error('Error fetching session data', error);
    }
  }

  const getPlots = () => {
    let barchartShootsUrl = `http://localhost:8000/api/barchart-comparison/${session1?.date}/${session2?.date}`;
    setBarchartURL(barchartShootsUrl);

    let heatmapP1Url = `http://localhost:8000/api/heatmap/${session1?.date}`;
    setHeatmapP1URL(heatmapP1Url);

    let heatmapP2Url = `http://localhost:8000/api/heatmap/${session2?.date}`;
    setHeatmapP2URL(heatmapP2Url);

    let plotTimesP1Url = `http://localhost:8000/api/plot-times/${session1?.date}`;
    setPlotTimesP1URL(plotTimesP1Url);

    let plotTimesP2Url = `http://localhost:8000/api/plot-times/${session2?.date}`;
    setPlotTimesP2URL(plotTimesP2Url);
  }

  useEffect(() => {
    if (session1 && session2) {
      getSessionsTracking();
      getSessionsData();
      getPlots();
    }
  }, [session1, session2]);

  useEffect(() => {
    if (sessionTrackingP1.length > 0 && session1) {
      let speedL = 0, speedR = 0;
      sessionTrackingP1.forEach(data => {
        speedL += Math.sqrt(data.LHandVelocity_x + data.LHandVelocity_y + data.LHandVelocity_z);
        speedR += Math.sqrt(data.RHandVelocity_x + data.RHandVelocity_y + data.RHandVelocity_z);
      });
      setLhandSpeedP1(speedL);
      setRhandSpeedP1(speedR);
    }
    if (sessionTrackingP2.length > 0 && session2) {
      let speedL = 0, speedR = 0;
      sessionTrackingP2.forEach(data => {
        speedL += Math.sqrt(data.LHandVelocity_x + data.LHandVelocity_y + data.LHandVelocity_z);
        speedR += Math.sqrt(data.RHandVelocity_x + data.RHandVelocity_y + data.RHandVelocity_z);
      });
      setLhandSpeedP2(speedL);
      setRhandSpeedP2(speedR);
    }
  }, [sessionTrackingP1, sessionTrackingP2, session1, session2]);

  return (
    <Box>
      <Navbar user={loggedUser} />
      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 6 }}>
          <IconButton
            onClick={goBack}
            sx={{ mr: 2 }}
          >
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h4">Players Comparison</Typography>
        </Box>

        <Box sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          gap: 2,
          mb: 6
        }}>
          <Box sx={{ textAlign: 'center' }}>
            <Avatar
              sx={{
                width: 60,
                height: 60,
                bgcolor: '#00CED1',
                mb: 1,
              }}
            >
              {player1?.avatar}
            </Avatar>
            <Typography>{player1?.name}</Typography>
          </Box>

          <Typography variant="h6" sx={{ mx: 1 }}>vs</Typography>

          <Box sx={{ textAlign: 'center' }}>
            <Avatar
              sx={{
                width: 60,
                height: 60,
                bgcolor: '#00CED1',
                mb: 1,
                alignContent: 'center',
                alignItems: 'center'
              }}
            >
              {player2?.avatar}
            </Avatar>
            <Typography>{player2?.name}</Typography>
          </Box>
        </Box>

        <Box sx={{
          display: 'flex',
          flexDirection: 'column',
          gap: 4
        }}>
          {/* Métrica 1 */}
          <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 4 }}>

            <Paper sx={{ flex: 1, p: 3 }}>
              <Typography variant="h6" gutterBottom>
                Summary of Shoots
              </Typography>
              <Box sx={{
                width: '100%',
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center'
              }}>
                {barchartURL ? (
                  <img src={barchartURL} alt="barchart" style={{ width: '100%', objectFit: 'contain' }} />
                ) : (
                  <Typography variant="body2" color="textSecondary">Loading bar chart...</Typography>
                )}
              </Box>
            </Paper>

            {/* Métrica 2 */}
            <Paper sx={{ flex: 1, p: 3 }}>
              <TableContainer component={Paper}>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell align='center'>Metric</TableCell>
                      <TableCell align="center">Player 1</TableCell>
                      <TableCell align="center">Player 2</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    <TableRow>
                      <TableCell component="th" scope="row" align="center">Left Hand Speed</TableCell>
                      <TableCell align="center">{LhandSpeedP1?.toFixed(3)}s</TableCell>
                      <TableCell align="center">{LhandSpeedP2?.toFixed(3)}s</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row" align="center">Right Hand Speed</TableCell>
                      <TableCell align="center">{RhandSpeedP1?.toFixed(3)}s</TableCell>
                      <TableCell align="center">{RhandSpeedP2?.toFixed(3)}s</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row" align="center">Session Duration</TableCell>
                      <TableCell align="center">{sessionDataP1?.session_time}s</TableCell>
                      <TableCell align="center">{sessionDataP2?.session_time}s</TableCell>
                    </TableRow>

                  </TableBody>
                </Table>
              </TableContainer>
            </Paper>
          </Box>

          {/* Métrica 3 */}
          <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 4 }}>
            <Paper sx={{ flex: 1, p: 3 }}>
              <Typography variant="h6" gutterBottom>
                Heat Maps
              </Typography>
              <Box sx={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                gap: 4
              }}>
                {/* Player 1 Heatmap */}
                <Box sx={{ textAlign: 'center', flex: 1 }}>
                  <Typography variant="subtitle2" gutterBottom>
                    Player 1
                  </Typography>
                  <Box sx={{ position: 'relative', width: '100%', paddingBottom: '56.25%' }}>
                    <img src="/porteria.png" alt="Portería" style={{ inset: 0, position: 'absolute', width: '100%', height: '86%', objectFit: 'contain', zIndex: 1 }} />
                    {heatmapP1URL ? (
                      <img src={heatmapP1URL} alt="Player 1 Heat Map" style={{ inset: 0, position: 'absolute', width: '100%', height: '67.8%', objectFit: 'contain', zIndex: 2, top: '5%' }} />
                    ) : (
                      <Typography variant="body2" color="textSecondary">Loading heat map...</Typography>
                    )}
                  </Box>
                </Box>
                {/* Player 2 Heatmap */}
                <Box sx={{ textAlign: 'center', flex: 1 }}>
                  <Typography variant="subtitle2" gutterBottom>
                    Player 2
                  </Typography>
                  <Box sx={{ position: 'relative', width: '100%', paddingBottom: '56.25%' }}>
                    <img src="/porteria.png" alt="Portería" style={{ inset: 0, position: 'absolute', width: '100%', height: '86%', objectFit: 'contain', zIndex: 1 }} />
                    {heatmapP2URL ? (
                      <img src={heatmapP2URL} alt="Player 2 Heat Map" style={{ inset: 0, position: 'absolute', objectFit: 'contain', width: '100%', height: '67.8%', zIndex: 2, top: '5%' }} />
                    ) : (
                      <Typography variant="body2" color="textSecondary">Loading heat map...</Typography>
                    )}
                  </Box>
                </Box>
              </Box>
            </Paper>
          </Box>

          {/* Métrica 4 */}
          <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 4 }}>
            <Paper sx={{ flex: 1, p: 3 }}>
              <Typography variant="h6" gutterBottom>
                Reaction Times
              </Typography>
              <Box sx={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                gap: 4
              }}>
                {/* Player 1 Plot */}
                <Box sx={{ textAlign: 'center', flex: 1 }}>
                  <Typography variant="subtitle2" gutterBottom>
                    Player 1
                  </Typography>
                  <Box sx={{ position: 'relative', width: '100%', paddingBottom: '56.25%' }}>
                    {plotTimesP1URL ? (
                      <img src={plotTimesP1URL} alt="Player 1 Reaction Times" style={{ inset: 0, position: 'absolute', width: '100%', height: '100%', objectFit: 'contain', zIndex: 2, top: '5%' }} />
                    ) : (
                      <Typography variant="body2" color="textSecondary">Loading plot...</Typography>
                    )}
                  </Box>
                </Box>
                {/* Player 2 Plot */}
                <Box sx={{ textAlign: 'center', flex: 1 }}>
                  <Typography variant="subtitle2" gutterBottom>
                    Player 2
                  </Typography>
                  <Box sx={{ position: 'relative', width: '100%', paddingBottom: '56.25%' }}>
                    {plotTimesP2URL ? (
                      <img src={plotTimesP2URL} alt="Player 2 Reaction Times" style={{ inset: 0, position: 'absolute', objectFit: 'contain', width: '100%', height: '100%', zIndex: 2, top: '5%' }} />
                    ) : (
                      <Typography variant="body2" color="textSecondary">Loading plot...</Typography>
                    )}
                  </Box>
                </Box>
              </Box>
            </Paper>
          </Box>

        </Box>
      </Container>
    </Box>
  );
};

export default ComparisonDetails;
