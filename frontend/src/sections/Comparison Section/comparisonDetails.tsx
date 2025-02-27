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
  const baseURL = 'http://192.168.43.173:12345';

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
      const response = await fetch(`${baseURL}/sessionData/${session1.date}`);
      if (response.ok) {
        console.log("Session Tracking P1 ANSWER")
        const dataP1 = await response.json();
        setSessionDataP1(dataP1);
      } else {
        console.error('Error fetching P1 session tracking');
      }

      let responseP2 = await fetch(`${baseURL}/sessionData/${session2.date}`);
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
      const response = await fetch(`${baseURL}/sessionTracking/${session1.date}`);
      if (response.ok) {
        console.log("Session Tracking P1 ANSWER")
        const dataP1 = await response.json();
        setSessionTrackingP1(dataP1);
      } else {
        console.error('Error fetching P1 session tracking');
      }

      let responseP2 = await fetch(`${baseURL}/sessionTracking/${session2.date}`);
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
    let barchartShootsUrl = `${baseURL}/barchart-comparison/${session1?.date}/${session2?.date}`;
    setBarchartURL(barchartShootsUrl);

    let heatmapP1Url = `${baseURL}/heatmap/${session1?.date}`;
    setHeatmapP1URL(heatmapP1Url);

    let heatmapP2Url = `${baseURL}/heatmap/${session2?.date}`;
    setHeatmapP2URL(heatmapP2Url);
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
        <Box sx={{ display: 'flex', alignItems: "center", mb: 6 }}>
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
          justifyContent: "center",
          alignItems: "center",
          gap: 2,
          mb: 6
        }}>
          <Box sx={{ textAlign: "center" }}>
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

          <Box sx={{ textAlign: "center" }}>
            <Avatar
              sx={{
                width: 60,
                height: 60,
                bgcolor: '#00CED1',
                mb: 1,
                alignContent: "center",
                alignItems: "center"
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
          <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 4 }}>

            {(session1?.prestige_level !== "LightsReaction" && session1?.prestige_level !== "LightsReaction2") && (
              <>
                {/* Métrica 1 */}
                < Paper sx={{ flex: 1, p: 3 }}>
                  <Typography variant="h6" gutterBottom align="center">Summary of Shoots</Typography>
                  <Box sx={{
                    width: '100%',
                    display: 'flex',
                    justifyContent: "center",
                    alignItems: "center"
                  }}>
                    {barchartURL ? (
                      <img src={barchartURL} alt="barchart" style={{ width: '100%', objectFit: 'contain' }} />
                    ) : (
                      <Typography variant="body2" color="textSecondary">Loading bar chart...</Typography>
                    )}
                  </Box>
                </Paper>
              </>
            )}

            {/* Métrica 2 */}
            <Paper sx={{ flex: 1, p: 3 }}>
              <Typography variant="h6" gutterBottom align="center">Summary of Data</Typography>
              <TableContainer component={Paper} sx={{ border: '1px solid black'}}>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell align="center" sx={{ borderBottom: '1px solid black', borderRight: '1px solid black'}}>Metric</TableCell>
                      <TableCell align="center" sx={{ borderBottom: '1px solid black', borderRight: '1px solid black'}}>{player1?.name}</TableCell>
                      <TableCell align="center" sx={{ borderBottom: '1px solid black'}}>{player2?.name}</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    <TableRow>
                      <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black'}}><b>Session Duration</b></TableCell>
                      <TableCell align="center" sx={{ borderRight: '1px solid black'}}>{sessionDataP1?.session_time}s</TableCell>
                      <TableCell align="center">{sessionDataP2?.session_time}s</TableCell>
                    </TableRow>
                    {(session1?.prestige_level === "LightsReaction" || session1?.prestige_level === "LightsReaction2") && (
                      <>
                        <TableRow>
                            <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black'}}><b>Nº of lights</b></TableCell>
                          <TableCell align="center" sx={{ borderRight: '1px solid black'}}>{sessionDataP1?.n_lights}</TableCell>
                          <TableCell align="center">{sessionDataP2?.n_lights}</TableCell>
                        </TableRow>
                      </>
                    )}
                    <TableRow>
                      <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black'}}><b>Left Hand Speed</b></TableCell>
                      <TableCell align="center" sx={{ borderRight: '1px solid black'}}>{LhandSpeedP1?.toFixed(3)}s</TableCell>
                      <TableCell align="center">{LhandSpeedP2?.toFixed(3)}s</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black'}}><b>Right Hand Speed</b></TableCell>
                      <TableCell align="center" sx={{ borderRight: '1px solid black'}}>{RhandSpeedP1?.toFixed(3)}s</TableCell>
                      <TableCell align="center">{RhandSpeedP2?.toFixed(3)}s</TableCell>
                    </TableRow>
                  </TableBody>
                </Table>
              </TableContainer>
            </Paper>
          </Box>

          {(session1?.prestige_level !== "LightsReaction" && session1?.prestige_level !== "LightsReaction2") && (
            <>

              {/* Métrica 3 */}
              <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 4 }}>
                <Paper sx={{ flex: 1, p: 3 }}>
                  <Typography variant="h6" gutterBottom align="center"> Heat Maps</Typography>
                  <Box sx={{
                    display: 'flex',
                    justifyContent: "center",
                    alignItems: "center",
                    gap: 4
                  }}>
                    {/* Player 1 Heatmap */}
                    <Box sx={{ textAlign: "center", flex: 1 }}>
                      <Typography variant="subtitle2" gutterBottom>
                        {player1?.name}
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
                    <Box sx={{ textAlign: "center", flex: 1 }}>
                      <Typography variant="subtitle2" gutterBottom>
                        {player2?.name}
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
            </>
          )}

        </Box>
      </Container >
    </Box >
  );
};

export default ComparisonDetails;
