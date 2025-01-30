import { Box, Container, Typography, Paper, Grid, IconButton, Avatar } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useLocation } from 'wouter';
import { useEffect, useState } from 'react';
import { Session } from '../../types/session';
import { User } from '../../types/user';
import { SessionData } from '../../types/sessionData';
import { SessionTracking } from '../../types/sessionTracking';
import Navbar from '../../components/navBar';
import { formatDate } from '../../components/utils';

const SessionDetails = (): JSX.Element => {
    const [, navigate] = useLocation();
    const [player, setPlayer] = useState<User | null>(null);
    const [session, setSession] = useState<Session | null>(null);
    const [sessionData, setSessionData] = useState<SessionData | null>(null);
    const [barchartShootsURL, setBarchartShootsURL] = useState<any>(null);
    const [sessionTracking, setSessionTracking] = useState<SessionTracking[]>([]);
    const [barchartSavesURL, setBarchartSavesURL] = useState<any>(null);
    const [heatmapURL, setHeatmapURL] = useState<any>(null);
    const [plotTimesURL, setPlotTimesURL] = useState<any>(null);
    const [scatterplotPositionsURL, setScatterplotPositionsURL] = useState<any>(null);
    const [LhandSpeed, setLhandSpeed] = useState<number>()
    const [RhandSpeed, setRhandSpeed] = useState<number>()
    const [savesPercentage, setSavesPercentage] = useState<number>()
    const [loggedUser, setLoggedUser] = useState<User | null>(null);

    const getSessionData = async () => {
        if (!session) return;
        try {
            const response = await fetch(`http://localhost:8000/api/sessionData/${session.date}`);
            if (response.ok) {
                console.log("Session Data ANSWER")
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
            const response = await fetch(`http://localhost:8000/api/sessionTracking/${session.date}`);
            if (response.ok) {
                console.log("Session Tracking ANSWER")
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
        let barchartShootsUrl = `http://localhost:8000/api/barchart-shoots/${session?.date}`;
        setBarchartShootsURL(barchartShootsUrl);

        let barchartSavesUrl = `http://localhost:8000/api/barchart-saves/${session?.date}`;
        setBarchartSavesURL(barchartSavesUrl);

        let heatmapUrl = `http://localhost:8000/api/heatmap/${session?.date}`;
        setHeatmapURL(heatmapUrl);

        let plotTimesUrl = `http://localhost:8000/api/plot-times/${session?.date}`;
        setPlotTimesURL(plotTimesUrl);

        let scatterplotPositionsUrl = `http://localhost:8000/api/scatterplot-positions/${session?.date}`;
        setScatterplotPositionsURL(scatterplotPositionsUrl);
    }

    useEffect(() => {
        if (sessionData && session) {
            getPlots();

            const saves = sessionData.n_saves || 0;
            const goals = sessionData.n_goals || 0;
            setSavesPercentage((saves / (saves + goals)) * 100 || 0);
        }
    }, [sessionData, session]);

    useEffect(() => {
        if (sessionTracking.length > 0 && session) {
            let speedL = 0, speedR = 0;
            sessionTracking.forEach(data => {
                speedL += Math.sqrt(data.LHandVelocity_x + data.LHandVelocity_y + data.LHandVelocity_z);
                speedR += Math.sqrt(data.RHandVelocity_x + data.RHandVelocity_y + data.RHandVelocity_z);
            });
            setLhandSpeed(speedL);
            setRhandSpeed(speedR);
        }
    }, [sessionTracking, session]);

    const goBack = () => {
        navigate("/player-sessions", {
            state: { player, user: loggedUser }
        });
    }

    return (
        <Box>
            <Navbar user={loggedUser} />
            <Container maxWidth="lg" sx={{ mt: 4 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 4 }}>
                    <IconButton
                        onClick={goBack}
                        sx={{ mr: 2 }}
                    >
                        <ArrowBackIcon />
                    </IconButton>
                    <Typography variant="h4" align='center'>Statistics</Typography>
                </Box>

                <Grid container spacing={4}>
                    {/* Left Column - Player Info and Session Details */}
                    <Grid item xs={12} md={3}>
                        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                                <Avatar sx={{ width: 60, height: 60, bgcolor: '#00CED1' }} />
                                <Typography variant="h6">{player?.name}</Typography>
                            </Box>

                            <Paper sx={{ p: 2 }}>
                                {session ? (
                                    <>
                                        <Typography variant="subtitle2" gutterBottom>Session Date: {formatDate(session.date)}</Typography>
                                        <Typography variant="subtitle2">• From: {sessionData?.shoots_initial_zone}</Typography>
                                        <Box sx={{ mt: 2 }}>
                                            <Typography>Configuration</Typography>
                                            <Typography variant="subtitle2">• Game mode: {session?.game_mode}</Typography>
                                            <Typography variant="subtitle2">• Difficulty: {session?.prestige_level}</Typography>
                                            {/*              <Typography variant="subtitle2">• Tamaño del modelo: {sessionData.configuration.modelSize}</Typography> */}
                                        </Box>
                                    </>
                                ) : (
                                    <Typography variant="body2" color="textSecondary">Loading data...</Typography>
                                )}
                            </Paper>
                        </Box>
                    </Grid>

                    <Grid item xs={12} md={9}>
                        <Box sx={{
                            display: 'flex',
                            flexDirection: 'column',
                            gap: 4,
                            width: '100%',
                        }}>
                            {/* Metric 1 - Bar Chart */}
                            <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                <Paper sx={{ flex: 1, p: 3, minWidth: { xs: '100%', md: '45%' } }}>
                                    <Typography variant="h6" gutterBottom>Bar Chart of Shoots</Typography>
                                    <Box sx={{
                                        width: '100%',
                                        display: 'flex',
                                        justifyContent: 'center',
                                        alignItems: 'center'
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
                                    <Typography variant="h6" gutterBottom>Bar Chart of Saves per Bodypart</Typography>
                                    <Box sx={{
                                        width: '100%',
                                        display: 'flex',
                                        justifyContent: 'center',
                                        alignItems: 'center'
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
                                    <Typography variant="h6" gutterBottom>Heat Map of Goal Zones</Typography>
                                    <Box sx={{
                                        position: 'relative',
                                        backgroundSize: 'contain',
                                        paddingBottom: '56.25%', // 16:9 aspect ratio
                                        backgroundPosition: 'center',
                                        backgroundRepeat: 'no-repeat',
                                        alignItems: 'center',
                                        justifyContent: 'center'
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
                                    <Typography variant="h6" gutterBottom>Summary of Data</Typography>
                                    <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr' }}>
                                        <Typography fontWeight="bold"> • Session Duration</Typography>
                                        <Typography>{sessionData?.session_time}s</Typography>
                                        <Typography fontWeight="bold">• Saves percentage</Typography>
                                        <Typography>{savesPercentage}%</Typography>
                                        <Typography fontWeight="bold">• Average Hand Speed</Typography>
                                        <Typography>Left: {LhandSpeed?.toFixed(3)} s  <br /> Right: {RhandSpeed?.toFixed(3)}s</Typography>
                                    </Box>
                                </Paper>
                            </Box>

                            {/* Metric 4 - Plot */}
                            <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                                    {plotTimesURL ? (
                                        <img src={plotTimesURL} alt="Scatter plot of reaction time" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                                    ) : (
                                        <Typography variant="body2" color="textSecondary">Loading plot...</Typography>
                                    )}
                                </Paper>
                            </Box>
                            {/* Metric 5 - Scatter Plot */}
                            <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                                <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                                    {scatterplotPositionsURL ? (
                                        <img src={scatterplotPositionsURL} alt="Scatter plot of positions" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                                    ) : (
                                        <Typography variant="body2" color="textSecondary">Loading scatter plot...</Typography>
                                    )}
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
