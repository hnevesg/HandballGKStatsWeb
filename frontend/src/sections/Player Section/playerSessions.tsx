import {
    Box, Container, Typography, Table, TableBody, TableCell, TableContainer,
    TableHead, TableRow, Paper, Avatar, IconButton, MenuItem, Select
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import LaunchIcon from '@mui/icons-material/Launch';
import { User } from '../../types/user';
import { Session } from '../../types/session';
import { useEffect, useState } from 'react';
import { useLocation } from 'wouter';
import Navbar from '../../components/navBar';
import { formatDate } from '../../components/utils';

const PlayerSessions = (): JSX.Element => {
    const [player, setPlayer] = useState<User>();
    const [, navigate] = useLocation();
    const [sessions, setSessions] = useState<Session[]>([]);
    const [loggedUser, setLoggedUser] = useState<User | null>(null);
    const [level, setLevel] = useState('Beginner');
    const baseURL = 'http://localhost:8000/api';

    const getSessions = async () => {
        if (!player) return;
        try {
            const response = await fetch(`${baseURL}/sessions/${player.id}?level=${level}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            if (response.ok) {
                const data = await response.json();
                setSessions(data);
            } else {
                console.error('Error fetching session data');
            }
        } catch (error) {
            console.error('Error fetching session data');
        }
    }

    useEffect(() => {
        const state = window.history.state;
        if (state?.player) {
            setPlayer(state.player);
        }
        if (state?.user) {
            setLoggedUser(state.user)
        }
    }, []);

    useEffect(() => {
        getSessions();
    }, [player]);

    const handleSessionSelect = (session: any) => {
        navigate("/statistics-details", {
            state: { player: player, session: session, user: loggedUser }
        });
    }

    const goBack = () => {
        navigate("/player-section", {
            state: { mail: loggedUser?.email }
        });
    }

    // Para cuando se cambia el nivel de dificultad
    useEffect(() => {
        getSessions();
    }, [level]);

    return (
        <Box>
            <Navbar user={loggedUser} />
            <Container maxWidth="md" sx={{ mt: 4 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 4 }}>
                    <IconButton
                        onClick={goBack}
                        sx={{ mr: 2 }}
                    >
                        <ArrowBackIcon />
                    </IconButton>
                    <Typography variant="h4" align='center'>Statistics</Typography>
                </Box>
                <Box sx={{
                    display: 'flex',
                    justifyContent: 'center',
                    mb: 4,
                    gap: 13
                }}>
                    <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', mb: 4 }}>
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

                    <Box sx={{ width: 200 }}>
                        <Typography variant="subtitle2" gutterBottom>Difficulty</Typography>
                        <Select
                            fullWidth
                            value={level}
                            onChange={(e) => setLevel(e.target.value)}
                        >
                            <MenuItem value="Beginner">Beginner</MenuItem>
                            <MenuItem value="Intermediate">Intermediate</MenuItem>
                            <MenuItem value="Expert">Expert</MenuItem>
                            <MenuItem value="Progressive">Progressive I</MenuItem>
                            <MenuItem value="Progressive2">Progressive II</MenuItem>
                            <MenuItem value="PerTime">PerTime</MenuItem>
                            <MenuItem value="LightsReaction">LightsReaction I</MenuItem>
                            <MenuItem value="LightsReaction2">LightsReaction II</MenuItem>
                        </Select>
                    </Box>
                </Box>

                {sessions.length > 0 ? (
                    <TableContainer component={Paper} sx={{ width: '80%', maxHeight: '50vh', overflow: 'auto', border: '1px solid black', alignContent: 'center', margin: 'auto' }}>
                        <Table stickyHeader>
                            <TableHead>
                                <TableRow>
                                    <TableCell sx={{ textAlign: 'center', borderBottom: '1px solid black' }}>Date</TableCell>
                                    <TableCell sx={{ textAlign: 'center', borderBottom: '1px solid black' }}>Game Mode</TableCell>
                                    <TableCell sx={{ textAlign: 'center', borderBottom: '1px solid black' }}>Details</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {sessions.map((session) => (
                                    <TableRow key={session.id}>
                                        <TableCell sx={{ textAlign: 'center' }}>{formatDate(session.date.toString())}</TableCell>
                                        <TableCell sx={{ textAlign: 'center' }}>{session.game_mode}</TableCell>
                                        <TableCell sx={{ textAlign: 'center' }}>
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
                ) : (
                    <Typography variant="body1" align='center' color="error" sx={{
                        backgroundColor: '#fce4ec',
                        border: '1px solid #f8bbd0',
                        borderRadius: '8px',
                        padding: '12px 16px',
                        marginBottom: '24px',
                        fontWeight: 600,
                        boxShadow: '0px 4px 6px rgba(0, 0, 0, 0.1)',
                    }}>No sessions available</Typography>
                )}
            </Container>
        </Box >
    );
};

export default PlayerSessions;