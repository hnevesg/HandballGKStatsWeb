import {
  Typography, Box, Button, Container, Table, TableBody,
  TableCell, TableContainer, TableHead, TableRow, Paper
} from '@mui/material';
import Navbar from '../components/navBar';
import { useLocation } from 'wouter';
import { User } from '../types/user';
import { Session } from '../types/session';
import { useEffect, useState } from 'react';
import { Rol } from '../types/rol';
import { formatDate } from '../components/utils';
import { baseURL } from '../components/utils';

const Home = (): JSX.Element => {
  const [, navigate] = useLocation();
  const [sessions, setSessions] = useState<Session[]>([]);
  const [loggedUser, setLoggedUser] = useState<User | null>(null);
  const [players, setPlayers] = useState<User[]>([]);

  useEffect(() => {
    getUser();
  }, []);

  const getUser = async () => {
    const state = window.history.state;
    const response = await fetch(`${baseURL}/user/${state?.mail}`);
    if (response.ok) {
      const data = await response.json();
      setLoggedUser(data);
    }
  }

  const getPlayers = async () => {
    const url =
      loggedUser?.role === Rol.ENTRENADOR
        ? `${baseURL}/players?team_id=${loggedUser.teamID}`
        : `${baseURL}/players`;

    const response = await fetch(url);
    if (response.ok) {
      const data = await response.json();
      setPlayers(data);
    }
  };

  useEffect(() => {
    if (!loggedUser) return;

    getSessions();

    if (loggedUser?.role === Rol.ENTRENADOR) {
      getPlayers();
    }
    else if (loggedUser?.role === Rol.ADMINISTRADOR) {
      getPlayers();
    }
  }, [loggedUser]);

  const getSessions = async () => {
    if (loggedUser?.role == Rol.PORTERO) {
      const response = await fetch(`${baseURL}/sessions/${loggedUser?.id}`);
      if (response.ok) {
        const data = await response.json();
        const sortedSessions = data.sort((a: Session, b: Session) => new Date(b.date).getTime() - new Date(a.date).getTime());
        setSessions(sortedSessions.slice(0, 5));
      }
    } else {
      let response;
      if (loggedUser?.role == Rol.ENTRENADOR) {
        response = await fetch(`${baseURL}/team-sessions?team_id=${loggedUser?.teamID}`);
      } else if (loggedUser?.role == Rol.ADMINISTRADOR) {
        response = await fetch(`${baseURL}/team-sessions`);
      }
      if (response?.ok) {
        const data = await response.json();
        const sortedSessions = data.sort((a: Session, b: Session) => new Date(b.date).getTime() - new Date(a.date).getTime());
        setSessions(sortedSessions.slice(0, 5));
      }
    }
  }

  const getPlayerName = (playerId: number) => {
    const player = players.find(p => p.id === playerId);
    return player ? player.name : 'Unknown Player';
  };

  return (
    <Box>
      <Box>
        <Navbar user={loggedUser} />
      </Box>
      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Box
          sx={{
            backgroundImage: 'url("/home_img.png")',
            backgroundSize: 'cover',
            backgroundPosition: 'center',
            height: { xs: 220, sm: 300, md: 400 },
            width: '90%',
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'center',
            alignItems: 'center',
            color: 'white',
            textAlign: 'center',
            mb: 6,
          }}
        >
          <Typography
            variant="h3"
            gutterBottom
            sx={{
              fontSize: { xs: '2rem', sm: '2.5rem', md: '3rem' },
              backgroundColor: 'rgba(0, 0, 0, 0.5)',
              padding: { xs: '4px', sm: '8px' },
              borderRadius: '4px',
            }}
          >
            Welcome to Handball GK Stats Web!
          </Typography>
          <Typography
            variant="subtitle1"
            gutterBottom
            sx={{
              fontSize: { xs: '1rem', sm: '1.25rem' },
              backgroundColor: 'rgba(0, 0, 0, 0.5)',
              padding: { xs: '4px', sm: '8px' },
              borderRadius: '4px',
            }}
          >
            Want to know more about the game or creators?
          </Typography>
          <Button
            onClick={() => navigate('/about', {
              state: { mail: loggedUser?.email }
            })}
            variant="contained"
            sx={{
              border: '1px solid white',
              backgroundColor: 'black',
              color: 'white',
              mt: 2
            }}
          >
            Click here
          </Button>
        </Box>

         <Container maxWidth="md" sx={{ mb: 6 }}>
          <Paper sx={{ p: 2, height: '100%', width: '100%' }}>
            <Typography variant="h5" gutterBottom align="center">Last Sessions</Typography>
            <TableContainer
              component={Paper}
              sx={{
          width: { xs: '100%', sm: '90%', md: '70%' },
          mx: 'auto',
          border: '1px solid black',
          overflowX: 'auto',
              }}
            >
              <Table size="small" sx={{ minWidth: 320 }}>
          <TableHead>
            <TableRow>
              {(loggedUser?.role !== Rol.PORTERO) &&
                <TableCell sx={{ borderBottom: '1px solid black', borderRight: '1px solid black', textAlign: 'center', fontSize: { xs: '0.9rem', sm: '1rem' } }} >User</TableCell>
              }
              <TableCell sx={{ borderBottom: '1px solid black', textAlign: 'center', fontSize: { xs: '0.9rem', sm: '1rem' } }}>Date</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {sessions.map((session) => (
              <TableRow key={session.id}>
                {loggedUser?.role !== Rol.PORTERO && (
            <TableCell sx={{ borderRight: '1px solid black', textAlign: 'center', fontSize: { xs: '0.9rem', sm: '1rem' } }}>{getPlayerName(session.player_id)}</TableCell>
                )}
                <TableCell sx={{ textAlign: 'center', fontSize: { xs: '0.9rem', sm: '1rem' } }}>{formatDate(session.date).toString()}</TableCell>
              </TableRow>
            ))}
          </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Container>
  </Container >
    </Box >
  );
};

export default Home;
