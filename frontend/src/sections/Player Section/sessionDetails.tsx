import {
    Box, Container, Typography, Paper, Grid, IconButton, Avatar,
    TableContainer, Table, TableCell, TableHead, TableRow, TableBody,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useLocation } from 'wouter';
import { useEffect, useState } from 'react';
import { Session } from '../../types/session';
import { User } from '../../types/user';
import { SessionData } from '../../types/sessionData';
import { SessionTracking } from '../../types/sessionTracking';
import Navbar from '../../components/navBar';
import { formatDate } from '../../components/utils';
import { baseURL } from '../../components/utils';

const SessionDetails = (): JSX.Element => {
    const [, navigate] = useLocation();
    const [player, setPlayer] = useState<User | null>(null);
    const [session, setSession] = useState<Session | null>(null);
    const [sessionData, setSessionData] = useState<SessionData | null>(null);
    const [barchartShootsURL, setBarchartShootsURL] = useState<any>(null);
    const [sessionTracking, setSessionTracking] = useState<SessionTracking[]>([]);
    const [barchartSavesURL, setBarchartSavesURL] = useState<any>(null);
    const [heatmapURL, setHeatmapURL] = useState<any>(null);
    const [scatterplot2DPositionsURL, set2DScatterplotPositionsURL] = useState<any>(null);
    const [scatterplot3DPositionsURL, set3DScatterplotPositionsURL] = useState<any>(null);
    const [plotReactionTimesURL, setPlotReactionTimesURL] = useState<any>(null);
    const [LhandSpeed, setLhandSpeed] = useState<number>()
    const [RhandSpeed, setRhandSpeed] = useState<number>()
    const [savesPercentage, setSavesPercentage] = useState<number>()
    const [loggedUser, setLoggedUser] = useState<User | null>(null);

    const getSessionData = async () => {
        if (!session) return;
        try {
            const response = await fetch(`${baseURL}/sessionData/${session.date}`);
            if (response.ok) {
                const data = await response.json();
                setSessionData(data);
            } else {
                console.error('Error fetching session data');
            }
        } catch (error) {
            console.error('Error fetching session data', error);
        }
    }

    const getSessionTracking = async () => {
        if (!session) return;
        try {
            const response = await fetch(`${baseURL}/sessionTracking/${session.date}`);
            if (response.ok) {
                const data = await response.json();
                setSessionTracking(data);
            } else {
                console.error('Error fetching session tracking');
            }
        } catch (error) {
            console.error('Error fetching session tracking', error);
        }
    }

    const getDataFromPreviousPage = () => {
        const state = window.history.state;
        if (state?.player) {
            setPlayer(state.player);
        }
        if (state?.session) {
            setSession(state.session);
        }
        if (state?.user) {
            setLoggedUser(state.user)
        }
    }

    useEffect(() => {
        getDataFromPreviousPage();
    }, []);

    useEffect(() => {
        getSessionData();
        getSessionTracking();
    }, [session]);

    const getPlots = () => {
        if (!session) return;
        let barchartShootsUrl = `${baseURL}/barchart-shoots/${session?.date}`;
        setBarchartShootsURL(barchartShootsUrl);

        let barchartSavesUrl = `${baseURL}/barchart-saves/${session?.date}`;
        setBarchartSavesURL(barchartSavesUrl);

        let heatmapUrl = `${baseURL}/heatmap/${session?.date}`;
        setHeatmapURL(heatmapUrl);

        let scatterplot2DPositionsUrl = `${baseURL}/2D-scatterplot-positions/${session?.date}`;
        set2DScatterplotPositionsURL(scatterplot2DPositionsUrl);

        let scatterplot3DPositionsUrl = `${baseURL}/3D-scatterplot-positions/${session?.date}`;
        set3DScatterplotPositionsURL(scatterplot3DPositionsUrl);

        let reactionplotUrl = `${baseURL}/reaction-speed/${session?.date}`;
        setPlotReactionTimesURL(reactionplotUrl);
    }

    useEffect(() => {
        if (sessionData && session) {
            getPlots();

            const saves = sessionData.n_saves || 0;
            const goals = sessionData.n_goals || 0;
            setSavesPercentage(parseFloat(((saves / (saves + goals)) * 100 || 0).toFixed(2)));
        }
    }, [sessionData, session]);

    useEffect(() => {
        if (sessionTracking.length > 0 && session && sessionData) {
            /* let speedL = 0, speedR = 0;
             sessionTracking.forEach(data => {
                 speedL += Math.sqrt(
                     Math.pow(data.LHandVelocity_x, 2) +
                     Math.pow(data.LHandVelocity_y, 2) +
                     Math.pow(data.LHandVelocity_z, 2)
                 );
                 speedR += Math.sqrt(
                     Math.pow(data.RHandVelocity_x, 2) +
                     Math.pow(data.RHandVelocity_y, 2) +
                     Math.pow(data.RHandVelocity_z, 2)
                 );
             });*/
            let speedL = 0, speedR = 0;
            sessionTracking.forEach(data => {
                speedL += Math.sqrt(data.LHandVelocity_x ** 2 + data.LHandVelocity_y ** 2 + data.LHandVelocity_z ** 2);
                speedR += Math.sqrt(data.RHandVelocity_x ** 2 + data.RHandVelocity_y ** 2 + data.RHandVelocity_z ** 2);
            });
            const numFrames = sessionTracking.length;
            const avgSpeedL = numFrames > 0 && sessionData?.session_time ? speedL / numFrames : 0;
            const avgSpeedR = numFrames > 0 && sessionData?.session_time ? speedL / numFrames : 0;

            setLhandSpeed(avgSpeedL);
            setRhandSpeed(avgSpeedR);
        }
    }, [sessionTracking, session]);

    const goBack = () => {
        navigate("/player-sessions", {
            state: { player, user: loggedUser }
        });
    }

    const getUniqueInitialZones = () => {
        if (!sessionData) return '';
        const uniqueZones = new Set(sessionData.shoots_initial_zone.split(','));
        return Array.from(uniqueZones).join(',');
    };

    return (
        <Box>
            <Navbar user={loggedUser} />
            <Container maxWidth="lg" sx={{ mt: 4 }}>
                <Box sx={{ display: 'flex', alignItems: "center", mb: 4 }}>
                    <IconButton
                        onClick={goBack}
                        sx={{ mr: 2 }}
                    >
                        <ArrowBackIcon />
                    </IconButton>
                    <Typography variant="h4" align="center" sx={{ width: '100%' }}>Statistics</Typography>
                </Box>

                <Grid container spacing={4}>
                    {/* Left Column - Player Info and Session Details */}
                    <Grid item xs={12} md={3}>
                        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
                            <Box sx={{ display: 'flex', alignItems: "center", gap: 2 }}>
                                <Avatar sx={{ width: 60, height: 60, bgcolor: '#00CED1' }} />
                                <Typography variant="h6">{player?.name}</Typography>
                            </Box>

                            <Paper sx={{ p: 2 }}>
                                {session ? (
                                    <>
                                        <Typography variant="subtitle2" gutterBottom>Session Date: {formatDate(session.date)}</Typography>
                                        {(session?.prestige_level !== "LightsReaction" && session?.prestige_level !== "LightsReaction2") && (
                                            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                                <Typography variant="subtitle2" sx={{ wordBreak: 'break-word' }}>• From: {(getUniqueInitialZones() as string).replace(/,/g, ', ')}</Typography>
                                            </Box>
                                        )}
                                        <Box sx={{ mt: 2 }}>
                                            <Typography>Configuration</Typography>
                                            <Typography variant="subtitle2">• Game mode: {session?.game_mode}</Typography>
                                            <Typography variant="subtitle2">• Difficulty: <strong>{session?.prestige_level}</strong></Typography>
                                            {/*              <Typography variant="subtitle2">• Model size: {sessionData.configuration.modelSize}</Typography> */}
                                        </Box>
                                    </>
                                ) : (
                                    <Typography variant="body2" color="textSecondary">Loading data...</Typography>
                                )}
                            </Paper>
                        </Box>
                    </Grid>
                    <Grid item xs={12} md={9}>
                        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, width: '100%', }}>

                            {(session?.prestige_level === "LightsReaction" || session?.prestige_level === "LightsReaction2") && (
                                <>
                                    {/* Metric 3 - Summary Table */}
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                        <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                                            <Typography variant="h6" gutterBottom align="center">Summary of Data</Typography>
                                            <TableContainer component={Paper} sx={{ border: '1px solid black' }}>
                                                <Table>
                                                    <TableHead>
                                                        <TableRow>
                                                            <TableCell align="center" sx={{ borderBottom: '1px solid black', borderRight: '1px solid black' }}>Metric</TableCell>
                                                            <TableCell align="center" sx={{ borderBottom: '1px solid black' }}>Data</TableCell>
                                                        </TableRow>
                                                    </TableHead>
                                                    <TableBody>
                                                        <TableRow>
                                                            <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Session Duration</b></TableCell>
                                                            <TableCell align="center">{parseInt(sessionData?.session_time || '0')}s</TableCell>
                                                        </TableRow>
                                                        <TableRow>
                                                            <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Nº of Lights</b></TableCell>
                                                            <TableCell align="center">{sessionData?.n_lights}</TableCell>
                                                        </TableRow>
                                                        <TableRow>
                                                            <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Left Hand Speed</b></TableCell>
                                                            <TableCell align="center">{LhandSpeed?.toFixed(3)}m/s</TableCell>
                                                        </TableRow>
                                                        <TableRow>
                                                            <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Right Hand Speed</b></TableCell>
                                                            <TableCell align="center">{RhandSpeed?.toFixed(3)}m/s</TableCell>
                                                        </TableRow>
                                                    </TableBody>
                                                </Table>
                                            </TableContainer>
                                        </Paper>
                                    </Box>
                                    {/* Metric - Lines Plot */}
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                        <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                                            {plotReactionTimesURL ? (
                                                <img id={`reaction-speed-${session?.id}`} src={plotReactionTimesURL} alt="Plot of reaction speed" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                                            ) : (
                                                <Typography variant="body2" color="textSecondary">Loading plot...</Typography>
                                            )}
                                        </Paper>
                                    </Box>
                                </>
                            )}
                            {(session?.prestige_level !== "LightsReaction" && session?.prestige_level !== "LightsReaction2") && (
                                <>
                                    {/* Metric 1 - Bar Chart */}
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                        <Paper sx={{ flex: 1, p: 3, minWidth: { xs: '100%', md: '45%' } }}>
                                            <Typography variant="h6" gutterBottom align="center">Bar Chart of Shoots</Typography>
                                            <Box sx={{
                                                width: '100%',
                                                display: 'flex',
                                                justifyContent: "center",
                                                alignItems: "center"
                                            }}>
                                                {barchartShootsURL ? (
                                                    <img src={barchartShootsURL} alt="Bar Chart" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                                                ) : (
                                                    <Typography variant="body2" color="textSecondary">Loading bar chart...</Typography>
                                                )}
                                            </Box>
                                        </Paper>
                                    </Box>
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                        <Paper sx={{ flex: 1, p: 3, minWidth: { xs: '100%', md: '45%' } }}>
                                            <Typography variant="h6" gutterBottom align="center">Bar Chart of Saves per Bodypart</Typography>
                                            <Box sx={{
                                                width: '100%',
                                                display: 'flex',
                                                justifyContent: "center",
                                                alignItems: "center"
                                            }}>
                                                {barchartSavesURL ? (
                                                    <img src={barchartSavesURL} alt="Bar Chart" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                                                ) : (
                                                    <Typography variant="body2" color="textSecondary">Loading bar chart...</Typography>
                                                )}
                                            </Box>
                                        </Paper>
                                    </Box>
                                    {/* Metric 2 - HeatMap */}
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                        <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                                            <Typography variant="h6" gutterBottom align="center">Heat Map of Goal Zones</Typography>
                                            <Box sx={{
                                                position: 'relative',
                                                backgroundSize: 'contain',
                                                paddingBottom: '56.25%', // 16:9 aspect ratio
                                                backgroundPosition: "center",
                                                backgroundRepeat: 'no-repeat',
                                                alignItems: "center",
                                                justifyContent: "center"
                                            }}>
                                                <img src="/porteria.png" alt="Portería" style={{ position: 'absolute', width: '100%', height: '86%', objectFit: 'contain', zIndex: 1 }} />
                                                {heatmapURL ? (
                                                    <img src={heatmapURL} alt="Heatmap" style={{ position: 'absolute', width: '100%', height: '67.8%', top: '5%', objectFit: 'contain', zIndex: 2 }} />
                                                ) : (
                                                    <Typography variant="body2" color="textSecondary">Loading heat map...</Typography>
                                                )}
                                            </Box>
                                        </Paper>
                                    </Box>

                                    {/* Metric 3 - Summary Table */}
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                        <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                                            <Typography variant="h6" gutterBottom align="center">Summary of Data</Typography>
                                            <TableContainer component={Paper} sx={{ border: '1px solid black' }}>
                                                <Table>
                                                    <TableHead>
                                                        <TableRow>
                                                            <TableCell align="center" sx={{ borderBottom: '1px solid black', borderRight: '1px solid black' }}>Metric</TableCell>
                                                            <TableCell align="center" sx={{ borderBottom: '1px solid black' }}>Data</TableCell>
                                                        </TableRow>
                                                    </TableHead>
                                                    <TableBody>
                                                        <TableRow>
                                                            <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Session Duration</b></TableCell>
                                                            <TableCell align="center">{parseInt(sessionData?.session_time ||'0')}s</TableCell>
                                                        </TableRow>
                                                        <TableRow>
                                                            <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Saves Percentage</b></TableCell>
                                                            <TableCell align="center">{savesPercentage}%</TableCell>
                                                        </TableRow>
                                                        <TableRow>
                                                            <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Left Hand Speed</b></TableCell>
                                                            <TableCell align="center">{LhandSpeed?.toFixed(3)}m/s</TableCell>
                                                        </TableRow>
                                                        <TableRow>
                                                            <TableCell component="th" scope="row" align="center" sx={{ borderRight: '1px solid black' }}><b>Right Hand Speed</b></TableCell>
                                                            <TableCell align="center">{RhandSpeed?.toFixed(3)}m/s</TableCell>
                                                        </TableRow>
                                                    </TableBody>
                                                </Table>
                                            </TableContainer>
                                        </Paper>
                                    </Box>

                                </>
                            )}
                            {/* Metric 4 - Scatter Plot */}
                            <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                <Paper sx={{ display: 'flex', flexDirection: 'row', gap: 2, p: 2, width: '100%' }}>
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                        {scatterplot2DPositionsURL ? (
                                            <img src={scatterplot2DPositionsURL} alt="Scatter plot of positions" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                                        ) : (
                                            <Typography variant="body2" color="textSecondary">Loading scatter plot...</Typography>
                                        )}
                                    </Box>
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                        {scatterplot3DPositionsURL ? (
                                            <img src={scatterplot3DPositionsURL} alt="Scatter plot of positions" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                                        ) : (
                                            <Typography variant="body2" color="textSecondary">Loading scatter plot...</Typography>
                                        )}
                                    </Box>
                                </Paper>
                            </Box>
                        </Box>
                    </Grid>
                </Grid>
            </Container >
        </Box >
    );
};

export default SessionDetails;
