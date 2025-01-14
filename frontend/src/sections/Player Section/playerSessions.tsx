import {
    Box,
    Container,
    Typography,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    Avatar,
    IconButton,
    Grid
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import LaunchIcon from '@mui/icons-material/Launch';
import NavBar from '../../components/navBar';
import { Player } from '../../types/player';
import { Session } from '../../types/session';
import { useEffect, useState } from 'react';
import { useLocation } from 'wouter';

const PlayerSessions = (): JSX.Element => {
    const [player, setPlayer] = useState<Player | null>(null);
    const [, navigate] = useLocation();

    const [sessions] = useState<Session[]>([
        { date: '22/01/23', mode: 'Expert', details: 'View' },
        { date: '01/10/23', mode: 'Fixed Position', details: 'View' },
        { date: '14/09/23', mode: 'Progressive I', details: 'View' }
    ]);

    useEffect(() => {
        const state = window.history.state;
        if (state?.player) {
            setPlayer(state.player);
        }
    }, []);

    const handleSessionSelect = (session: Session) => {
        //setSession(session);
        navigate("/statistics-details", {
            state: { player: window.history.state.player, session: session }
        });
    }

    return (
        <Box>
            <NavBar />
            <Container maxWidth="md" sx={{ mt: 4 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 4 }}>
                    <IconButton
                        onClick={() => window.location.href = '/player-section'}
                        sx={{ mr: 2 }}
                    >
                        <ArrowBackIcon />
                    </IconButton>
                    <Typography variant="h4" align='center'>Estadísticas</Typography>
                </Box>

                <Box sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    mb: 4
                }}>
                    <Avatar
                        sx={{
                            width: 80,
                            height: 80,
                            mb: 2,
                            bgcolor: '#00CED1'
                        }}
                    />
                    <Typography variant="h6">{player?.name}</Typography>
                </Box>

                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Fecha</TableCell>
                                <TableCell>Modo</TableCell>
                                <TableCell>Detalles</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {sessions.map((session, index) => (
                                <TableRow key={index}>
                                    <TableCell>{session.date}</TableCell>
                                    <TableCell>{session.mode}</TableCell>
                                    <TableCell>
                                        <IconButton size="small"
                                            onClick={() => handleSessionSelect(session)}
                                            sx={{ mr: 2 }}
                                        >
                                            <LaunchIcon />
                                        </IconButton>
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Container>
        </Box>
    );
};

export default PlayerSessions;
