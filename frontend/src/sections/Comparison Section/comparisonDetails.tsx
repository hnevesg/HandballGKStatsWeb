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
import { formatDate } from '../../components/utils';
import { baseURL } from '../../components/utils';

const ComparisonDetails = (): JSX.Element => {
  const [, navigate] = useLocation();
  const [player1, setPlayer1] = useState<User | null>(null);
  const [player2, setPlayer2] = useState<User | null>(null);
  const [session1, setSession1] = useState<Session | null>(null);
  const [session2, setSession2] = useState<Session | null>(null);
  const [barchartURL, setBarchartURL] = useState<any>(null);
  const [heatmapP1URL, setHeatmapP1URL] = useState<any>(null);
  const [heatmapP2URL, setHeatmapP2URL] = useState<any>(null);
  const [plotReactionTimesP1URL, setPlotReactionTimesP1URL] = useState<any>(null);
  const [plotReactionTimesP2URL, setPlotReactionTimesP2URL] = useState<any>(null);
  const [sessionTrackingP1, setSessionTrackingP1] = useState<SessionTracking[]>([]);
  const [sessionTrackingP2, setSessionTrackingP2] = useState<SessionTracking[]>([]);
  const [LhandSpeedP1, setLhandSpeedP1] = useState<number>()
  const [RhandSpeedP1, setRhandSpeedP1] = useState<number>()
  const [LhandSpeedP2, setLhandSpeedP2] = useState<number>()
  const [RhandSpeedP2, setRhandSpeedP2] = useState<number>()
  const [loggedUser, setLoggedUser] = useState<User | null>(null);
  const [sessionDataP1, setSessionDataP1] = useState<SessionData | null>();
  const [sessionDataP2, setSessionDataP2] = useState<SessionData | null>();

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
        const dataP1 = await response.json();
        setSessionDataP1(dataP1);
      } else {
        console.error('Error fetching P1 session tracking');
      }

      let responseP2 = await fetch(`${baseURL}/sessionData/${session2.date}`);
      if (responseP2.ok) {
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
        const dataP1 = await response.json();
        setSessionTrackingP1(dataP1);
      } else {
        console.error('Error fetching P1 session tracking');
      }

      let responseP2 = await fetch(`${baseURL}/sessionTracking/${session2.date}`);
      if (responseP2.ok) {
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
    if (session1?.prestige_level === "LightsReaction" || session1?.prestige_level === "LightsReaction2") {
      let plotReactionTimesP1Url = `${baseURL}/reaction-speed-times/${session1?.date}`;
      setPlotReactionTimesP1URL(plotReactionTimesP1Url);

      let plotReactionTimesP2Url = `${baseURL}/reaction-speed-times/${session2?.date}`;
      setPlotReactionTimesP2URL(plotReactionTimesP2Url);
    } else {
      let barchartShootsUrl = `${baseURL}/barchart-comparison/${session1?.date}/${session2?.date}?player1_name=${player1?.name}&player2_name=${player2?.name}`;
      setBarchartURL(barchartShootsUrl);

      let heatmapP1Url = `${baseURL}/heatmap/${session1?.date}`;
      setHeatmapP1URL(heatmapP1Url);

      let heatmapP2Url = `${baseURL}/heatmap/${session2?.date}`;
      setHeatmapP2URL(heatmapP2Url);
    }
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
        speedL += Math.sqrt(data.LHandVelocity_x ** 2 + data.LHandVelocity_y ** 2 + data.LHandVelocity_z ** 2);
        speedR += Math.sqrt(data.RHandVelocity_x ** 2 + data.RHandVelocity_y ** 2 + data.RHandVelocity_z ** 2);
      });
      const numFrames = sessionTrackingP1.length;
      const avgSpeedL = numFrames > 0 && sessionDataP1?.session_time ? speedL / numFrames : 0;
      const avgSpeedR = numFrames > 0 && sessionDataP1?.session_time ? speedL / numFrames : 0;

      setLhandSpeedP1(avgSpeedL);
      setRhandSpeedP1(avgSpeedR);
    }
    if (sessionTrackingP2.length > 0 && session2) {
      let speedL = 0, speedR = 0;
      sessionTrackingP2.forEach(data => {
        speedL += Math.sqrt(data.LHandVelocity_x ** 2 + data.LHandVelocity_y ** 2 + data.LHandVelocity_z ** 2);
        speedR += Math.sqrt(data.RHandVelocity_x ** 2 + data.RHandVelocity_y ** 2 + data.RHandVelocity_z ** 2);
      });
      const numFrames = sessionTrackingP2.length;
      const avgSpeedL = numFrames > 0 ? speedL / numFrames : 0;
      const avgSpeedR = numFrames > 0 ? speedR / numFrames : 0;

      setLhandSpeedP2(avgSpeedL);
      setRhandSpeedP2(avgSpeedR);
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
          <Typography variant="h4" align="center" sx={{ flex: 1 }}>Players Comparison</Typography>
        </Box>

        <Container maxWidth="lg" sx={{ position: 'relative' }} >
          {/* Rectangle with session info */}
          <Box
            sx={{
              position: 'absolute',
              left: 24,
              zIndex: 10,
              minWidth: 250,
              bgcolor: 'background.paper',
              borderRadius: 2,
              boxShadow: 3,
              p: 2,
            }}
          >
            <Typography variant="subtitle2" color="text.secondary" align='center'>
              Sessions Details
            </Typography>

            <Typography variant="body2" sx={{ display: 'flex', alignItems: 'flex-start' }}>
              <Box component="span" sx={{ fontWeight: 'bold' }}>
                Mode:
              </Box>
              <Box component="span" sx={{ display: 'flex', flexDirection: 'column', ml: 1 }}>
                <span>{session1?.prestige_level ? session1.prestige_level : 'N/A'}</span>
              </Box>
            </Typography>

            <Typography variant="body2" sx={{ display: 'flex', alignItems: 'flex-start' }}>
              <Box component="span" sx={{ fontWeight: 'bold' }}>
                Dates:
              </Box>
              <Box component="span" sx={{ display: 'flex', flexDirection: 'column', ml: 1 }}>
                <span>{session1?.date ? formatDate(session1.date) : 'N/A'}</span>
                <span>{session2?.date ? formatDate(session2.date) : 'N/A'}</span>
              </Box>
            </Typography>
          </Box>
        </Container>

        <Box sx={{
          display: 'flex',
          justifyContent: "center",
          alignItems: "center",
          gap: 1,
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

          <Typography variant="h6" sx={{ mr: 2 }}>vs</Typography>

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
              <TableContainer component={Paper} sx={{ border: '1px solid black' }}>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell align="center" sx={{ borderBottom: '1px solid black', borderRight: '1px solid black' }}>Metric</TableCell>
                      <TableCell align="center" sx={{ borderBottom: '1px solid black', borderRight: '1px solid black' }}>{player1?.name}</TableCell>
                      <TableCell align="center" sx={{ borderBottom: '1px solid black' }}>{player2?.name}</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    <TableRow>
                      <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Session Duration</b></TableCell>
                      <TableCell align="center" sx={{ borderRight: '1px solid black' }}>{parseInt(sessionDataP1?.session_time || '0')}s</TableCell>
                      <TableCell align="center">{parseInt(sessionDataP2?.session_time || '0')}s</TableCell>
                    </TableRow>
                    {(session1?.prestige_level === "LightsReaction" || session1?.prestige_level === "LightsReaction2") && (
                      <>
                        <TableRow>
                          <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Nº of lights</b></TableCell>
                          <TableCell align="center" sx={{ borderRight: '1px solid black' }}>{sessionDataP1?.n_lights}</TableCell>
                          <TableCell align="center">{sessionDataP2?.n_lights}</TableCell>
                        </TableRow>
                      </>
                    )}
                    <TableRow>
                      <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Left Hand Speed</b></TableCell>
                        <TableCell align="center" sx={{ borderRight: '1px solid black' }}>{LhandSpeedP1 ? `${LhandSpeedP1.toFixed(3)} m/s` : 'N/A'}</TableCell>
                        <TableCell align="center">{LhandSpeedP2 ? `${LhandSpeedP2.toFixed(3)} m/s` : 'N/A'}</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Right Hand Speed</b></TableCell>
                        <TableCell align="center" sx={{ borderRight: '1px solid black' }}>{RhandSpeedP1 ? `${RhandSpeedP1.toFixed(3)} m/s` : 'N/A'}</TableCell>
                        <TableCell align="center">{RhandSpeedP2 ? `${RhandSpeedP2.toFixed(3)} m/s` : 'N/A'}</TableCell>
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

          {(session1?.prestige_level === "LightsReaction" || session1?.prestige_level === "LightsReaction2") && (
            <>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                  <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                    <Typography variant="subtitle2" gutterBottom align="center">
                      {player1?.name}
                    </Typography>
                    {plotReactionTimesP1URL ? (
                      <img id={`reaction-speed-${session1?.id}`} src={plotReactionTimesP1URL} alt="Plot of reaction speed" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                    ) : (
                      <Typography variant="body2" color="textSecondary">Loading plot...</Typography>
                    )}
                  </Paper>
                </Box>

                <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                  <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                    <Typography variant="subtitle2" gutterBottom align="center">
                      {player2?.name}
                    </Typography>
                    {plotReactionTimesP2URL ? (
                      <img id={`reaction-speed-${session2?.id}`} src={plotReactionTimesP2URL} alt="Plot of reaction speed" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                    ) : (
                      <Typography variant="body2" color="textSecondary">Loading plot...</Typography>
                    )}
                  </Paper>
                </Box>
              </Box>
            </>
          )}


        </Box>
      </Container >
    </Box >
  );
};

export default ComparisonDetails;
