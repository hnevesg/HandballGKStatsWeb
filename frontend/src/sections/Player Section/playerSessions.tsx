import {
    Box, Container, Typography, Table, TableBody, TableCell, TableContainer,
    TableHead, TableRow, Paper, Avatar, IconButton
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
    const baseURL = 'http://localhost:8000/api';

    const getSessions = async () => {
        if (!player) return;
        try {
            const response = await fetch(`${baseURL}/sessions/${player.id}`, {
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

                <TableContainer component={Paper} sx={{ maxHeight: '50vh', overflow: 'auto' }}>
                    <Table stickyHeader>
                        <TableHead>
                            <TableRow>
                                <TableCell>Date</TableCell>
                                <TableCell>Game Mode</TableCell>
                                <TableCell>Difficulty</TableCell>
                                <TableCell>Details</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {sessions.map((session) => (
                                <TableRow key={session.id}>
                                    <TableCell>{formatDate(session.date.toString())}</TableCell>
                                    <TableCell>{session.game_mode}</TableCell>
                                    <TableCell>{session.prestige_level}</TableCell>
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
        </Box >
    );
};

export default PlayerSessions;
/*
107
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

*/