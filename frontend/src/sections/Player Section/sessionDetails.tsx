import {
    Box,
    Container,
    Typography,
    Paper,
    Grid,
    IconButton,
    Avatar
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useLocation } from 'wouter';
import NavBar from '../../components/navBar';
import { useEffect, useState } from 'react';
import { Session } from '../../types/session';
import { Player } from '../../types/player';

const SessionDetails = (): JSX.Element => {
    const [, navigate] = useLocation();
    const [player, setPlayer] = useState<Player | null>(null);
    const [session, setSession] = useState<Session | null>(null);

    const sessionData = {
        date: '22/11/23',
        timeSpent: '1:30mins',
        goals: 2,
        saves: 8,
        position: 'pivote, extremo izquierdo',
        configuration: {
            mode: 'Expert',
            modelSize: 'Small'
        }
    };

    useEffect(() => {
        const state = window.history.state;
        if (state?.player) {
            setPlayer(state.player);
        }
        if (state?.session) {
            setSession(state.session);
        }
    }, []);

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
                                <Typography variant="subtitle2">• Duración: {sessionData.timeSpent}</Typography>
                                <Typography variant="subtitle2">• Nº goles: {sessionData.goals}</Typography>
                                <Typography variant="subtitle2">• Nº paradas: {sessionData.saves}</Typography>
                                <Typography variant="subtitle2">• Desde: {sessionData.position}</Typography>
                                <Box sx={{ mt: 2 }}>
                                    <Typography>Configuración</Typography>
                                    <Typography variant="subtitle2">• Modo: {session?.mode}</Typography>
                                    <Typography variant="subtitle2">• Tamaño del modelo: {sessionData.configuration.modelSize}</Typography>
                                </Box>
                            </Paper>
                        </Box>
                    </Grid>

                    {/* Right Column - Metrics */}
                    <Grid item xs={12} md={9}>
                        <Grid container spacing={3}>
                            {/* Metric 1 - Scatter Plot */}
                            <Grid item xs={12} md={6}>
                                <Paper sx={{ p: 2, height: '300px' }}>
                                    <Typography variant="h6" gutterBottom>Métrica 1</Typography>
                                    {/* Scatter plot visualization would go here */}
                                </Paper>
                            </Grid>

                            {/* Metric 2 - Bar Chart */}
                            <Grid item xs={12} md={6}>
                                <Paper sx={{ p: 2, height: '300px' }}>
                                    <Typography variant="h6" gutterBottom>Métrica 2</Typography>
                                    {/* Bar chart visualization would go here */}
                                </Paper>
                            </Grid>

                            {/* Metric 3 - Table */}
                            <Grid item xs={12} md={6}>
                                <Paper sx={{ p: 2 }}>
                                    <Typography variant="h6" gutterBottom>Métrica 3</Typography>
                                    <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr' }}>
                                        <Typography fontWeight="bold">Velocidad media de las manos</Typography>
                                        <Typography>10ms</Typography>
                                    </Box>
                                </Paper>
                            </Grid>

                            {/* Metric 4 - Goal Visualization */}
                            <Grid item xs={12} md={6}>
                                <Paper sx={{ p: 2, height: '300px' }}>
                                    <Typography variant="h6" gutterBottom>Métrica 4</Typography>
                                    {/* Goal visualization would go here */}
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
