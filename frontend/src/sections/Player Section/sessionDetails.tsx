import { Box, Container, Typography, Paper, Grid, IconButton, Avatar } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useLocation } from 'wouter';
import NavBar from '../../components/navBar';
import { useEffect, useState } from 'react';
import { Session } from '../../types/session';
import { Player } from '../../types/player';
import { SessionData } from '../../types/sessionData';
import { SessionTracking } from '../../types/sessionTracking';

const SessionDetails = (): JSX.Element => {
    const [, navigate] = useLocation();
    const [player, setPlayer] = useState<Player>();
    const [session, setSession] = useState<Session>();
    const [sessionData, setSessionData] = useState<SessionData[]>([]);
    const [barchartShootsURL, setBarchartShootsURL] = useState<any>(null);
    const [sessionTracking, setSessionTracking] = useState<SessionTracking[]>([]);
    const [barchartSavesURL, setBarchartSavesURL] = useState<any>(null);
    const [heatmapURL, setHeatmapURL] = useState<any>(null);
    const [scatterplotURL, setScatterplotURL] = useState<any>(null);
    const [LhandSpeed, setLhandSpeed] = useState<number>()
    const [RhandSpeed, setRhandSpeed] = useState<number>()
    const [savesPercentage, setSavesPercentage] = useState<number>()

    const getSessionData = async () => {
        if (!session) return;
        try {
            const response = await fetch(`http://localhost:8000/api/sessionData/${session.id}`);
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
            const response = await fetch(`http://localhost:8000/api/sessionTracking/${session.id}`);
            if (response.ok) {
                console.log("Session Tracking ANSWER")
                const data = await response.json();
                setSessionTracking(data);
            } else {
                console.error('Error fetching session data');
            }
        } catch (error) {
            console.error('Error fetching session data', error);
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
    }

    useEffect(() => {
        getDataFromPreviousPage();
    }, []);

    useEffect(() => {
        getSessionData();
        getSessionTracking();
    }, [session]);

    useEffect(() => {
        if (sessionData && session) {
            let barchartShootsURL = `http://localhost:8000/api/barchart-shoots/${session.id}`;
            setBarchartShootsURL(barchartShootsURL);

            let barchartSavesURL = `http://localhost:8000/api/barchart-saves/${session.id}`;
            setBarchartSavesURL(barchartSavesURL);

            let heatmapUrl = `http://localhost:8000/api/heatmap/${session.id}`;
            setHeatmapURL(heatmapUrl);

            let saves = sessionData[0].n_saves;
            setSavesPercentage((saves / (saves + sessionData[0].n_goals)) * 100);
        }
    }, [sessionData]);

    useEffect(() => {
        if (sessionTracking && session) {
            //FOR EACH ENTRY -> SUMATORIO
            let speedL = Math.sqrt(sessionTracking[0].handL_speed_x + sessionTracking[0].handL_speed_y + sessionTracking[0].handL_speed_z);
            setLhandSpeed(speedL);
            let speedR = Math.sqrt(sessionTracking[0].handR_speed_x + sessionTracking[0].handR_speed_y + sessionTracking[0].handR_speed_z);
            setRhandSpeed(speedR);

            let scatterplotUrl = `http://localhost:8000/api/scatteplot/${session.id}`;
            setScatterplotURL(scatterplotUrl);

        }
    }, [sessionTracking]);

    const goBack = () => {
        navigate("/player-sessions", {
            state: { player: window.history.state.player }
        });
    }

    return (
        <Box>
            <NavBar />
            <Container maxWidth="lg" sx={{ mt: 4 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 4 }}>
                    <IconButton
                        onClick={goBack}
                        sx={{ mr: 2 }}
                    >
                        <ArrowBackIcon />
                    </IconButton>
                    <Typography variant="h4" align='center'>Estadísticas</Typography>
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
                                <Typography variant="subtitle2" gutterBottom>Fecha de la sesión: {session?.date}</Typography>
                                <Typography variant="subtitle2">• Duración: {/*sessionData[0].shoots_time*/}</Typography>
                                <Typography variant="subtitle2">• Nº goles: {sessionData[0]?.n_goals}</Typography>
                                <Typography variant="subtitle2">• Nº paradas: {sessionData[0]?.n_saves}</Typography>
                                <Typography variant="subtitle2">• Desde: {sessionData[0]?.shoots_final_zone}</Typography>
                                <Box sx={{ mt: 2 }}>
                                    <Typography>Configuración</Typography>
                                    <Typography variant="subtitle2">• Modo: {session?.game_mode}</Typography>
                                    <Typography variant="subtitle2">• Dificultad: {session?.prestige_level}</Typography>
                                    {/*              <Typography variant="subtitle2">• Tamaño del modelo: {sessionData.configuration.modelSize}</Typography> */}
                                </Box>
                            </Paper>
                        </Box>
                    </Grid>

                    <Grid item xs={12} md={9}>
                        <Grid container spacing={5}>
                            {/* Metric 1 - Bar Chart */}
                            <Grid item xs={12} md={6}>
                                <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                                    <Typography variant="h6" gutterBottom>Métrica 1: Gráfico de Barras de Lanzamientos</Typography>
                                    {barchartShootsURL && (
                                        <img src={barchartShootsURL} alt="Bar Chart" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                                    )}
                                </Paper>
                            </Grid>

                            <Grid item xs={12} md={6}>
                                <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                                    <Typography variant="h6" gutterBottom>Gráfico de Barras de Paradas</Typography>
                                    {barchartSavesURL && (
                                        <img src={barchartSavesURL} alt="Bar Chart" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                                    )}
                                </Paper>
                            </Grid>

                            {/* Metric 2 - HeatMap */}
                            <Grid item xs={12} md={6} >
                                <Paper sx={{ p: 2, height: '120%', width: '100%' }}>
                                    <Typography variant="h6" gutterBottom>Métrica 2: Mapa de Calor</Typography>
                                    <Box sx={{
                                        position: 'relative',
                                        height: '90%',
                                        width: '90%',
                                        backgroundSize: 'contain',
                                        backgroundPosition: 'center',
                                        backgroundRepeat: 'no-repeat',
                                        alignItems: 'center',
                                        justifyContent: 'center'
                                    }}>
                                        <img src="/porteria.png" alt="Portería" style={{ position: 'absolute', width: '100%', height: '86%', objectFit: 'contain', zIndex: 1, top: '15px' }} />
                                        {heatmapURL && (
                                            <img src={heatmapURL} alt="Heatmap" style={{ position: 'absolute', width: '118.4%', height: '75.2%', top: '18px', objectFit: 'contain', zIndex: 2 }} />
                                        )}
                                    </Box>
                                </Paper>
                            </Grid>

                            {/* Metric 3 - Summary Table */}
                            <Grid item xs={12} md={6}>
                                <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                                    <Typography variant="h6" gutterBottom>Métrica 3: Datos Resumen</Typography>
                                    <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr' }}>
                                        <Typography fontWeight="bold">Velocidad media de las manos</Typography>
                                        <Typography>Izquierda: {LhandSpeed?.toFixed(3)} s  <br /> Derecha: {RhandSpeed?.toFixed(3)}s</Typography>
                                        <Typography fontWeight="bold">Porcentaje de paradas</Typography>
                                        <Typography>{savesPercentage}%</Typography>
                                    </Box>
                                </Paper>
                            </Grid>

                            {/* Metric 4 - Scatter Plot */}
                            <Grid item xs={12} md={6}>
                                <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
                                    <Typography variant="h6" gutterBottom>Métrica 4: Gráfico de Dispersión</Typography>
                                    {/* Scatter plot visualization would go here */}
                                    {scatterplotURL && (
                                        <img src={scatterplotURL} alt="Scatter plot" style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                                    )};
                                </Paper>
                            </Grid>
                        </Grid>
                    </Grid>
                </Grid>
            </Container>
        </Box>
    );
};

export default SessionDetails;
