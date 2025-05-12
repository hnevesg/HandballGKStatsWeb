import { Box, Container, Typography, TextField, Button, Avatar, IconButton, Tooltip } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import { useState, useMemo, useEffect } from 'react';
import { useLocation } from 'wouter'
import { User } from '../../types/user';
import ArrowBackIosNewIcon from '@mui/icons-material/ArrowBackIosNew';
import ArrowForwardIosIcon from '@mui/icons-material/ArrowForwardIos';
import Navbar from '../../components/navBar';
import { Rol } from '../../types/rol';
import { baseURL } from '../../components/utils';

const PlayerSection = (): JSX.Element => {
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedPlayer, setSelectedPlayer] = useState<User | null>(null);
  const [, navigate] = useLocation();
  const [currentPage, setCurrentPage] = useState(0);
  const [loggedUser, setLoggedUser] = useState<User | null>(null);
  const [players, setPlayers] = useState<User[]>([]);
  const playersPerPage = 3;

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

  const filteredPlayers = useMemo(() => {
    return players.filter(player =>
      player.name.toLowerCase().includes(searchQuery.toLowerCase())
    );
  }, [players, searchQuery]);

  const handlePlayerSelect = (player: User) => {
    setSelectedPlayer(player);
  };

  const SetLocationToSessions = () => {
    if (selectedPlayer) {
      navigate("/player-sessions", {
        state: { player: selectedPlayer, user: loggedUser }
      });
    }
  };

  const SetLocationToProgress = () => {
    if (selectedPlayer) {
      navigate("/player-progress", {
        state: { player: selectedPlayer, user: loggedUser }
      });
    }
  };

  const getCurrentPagePlayers = useMemo(() => {
    const start = currentPage * playersPerPage;
    const end = start + playersPerPage;
    if (filteredPlayers.length <= playersPerPage) {
      return filteredPlayers;
    }
    return [...filteredPlayers].slice(start, end);
  }, [filteredPlayers, currentPage]);

  const handlePrevPage = () => {
    setCurrentPage(prev => {
      const newPage = Math.max(0, prev - 1);
      return newPage;
    });
  };

  const totalPages = Math.ceil(filteredPlayers.length / playersPerPage);

  const handleNextPage = () => {
    setCurrentPage(prev => {
      const maxPage = totalPages - 1;
      const newPage = Math.min(maxPage, prev + 1);
      return newPage;
    });
  };

  const getPlayers = async () => {
    if (!loggedUser) return;
    let response;
    if (loggedUser?.role == Rol.PORTERO) {
      response = await fetch(`${baseURL}/user/${loggedUser?.email}`);
      const data = await response?.json();
      if (response?.ok) {
        const [firstName = '', lastName = ''] = data.name.split(' ');
        const avatar = `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
        const playerWithAvatar = { ...data, avatar };

        setPlayers([playerWithAvatar]);
      } else {
        console.error('Failed to fetch player:', data.message);
      }
    } else if (loggedUser?.role == Rol.ENTRENADOR) {
      response = await fetch(`${baseURL}/players/${loggedUser?.teamID}`);
      const data = await response?.json();

      if (response?.ok) {
        const playersWithAvatars = data.map((player: User) => {
          const [firstName = '', lastName = ''] = player.name.split(' ');
          const avatar = `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
          return { ...player, avatar };
        });

        setPlayers(playersWithAvatars);
      }
      else {
        console.error('Failed to fetch players:', data.message);
      }
    } else if (loggedUser?.role == Rol.ADMINISTRADOR) {
      response = await fetch(`${baseURL}/players`);
      const data = await response?.json();

      if (response?.ok) {
        const playersWithAvatars = data.map((player: User) => {
          const [firstName = '', lastName = ''] = player.name.split(' ');
          const avatar = `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
          return { ...player, avatar };
        });

        setPlayers(playersWithAvatars);
      }
      else {
        console.error('Failed to fetch players:', data.message);
      }
    }
  }

  useEffect(() => {
    getPlayers();
  }, [loggedUser]);

  return (
    <Box>
      <Navbar user={loggedUser} />
      <Container maxWidth="md" sx={{ mt: 8 }}>
        <Typography variant="h4" sx={{ textAlign: 'center', mb: 4 }}>
          Select a player
        </Typography>

        <Box sx={{ display: 'flex', justifyContent: 'center', mb: 6 }}>
          <TextField
            placeholder="Search player..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            variant="outlined"
            InputProps={{
              startAdornment: <SearchIcon sx={{ mr: 1, color: 'gray' }} />
            }}
            sx={{ width: '100%', maxWidth: 500 }}
          />
        </Box>

        <Box sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          gap: 3,
          mb: 6,
          position: 'relative',
          width: '100%'
        }}>
          {filteredPlayers.length > playersPerPage && (
            <IconButton
              onClick={handlePrevPage}
              disabled={currentPage === 0}
              sx={{ position: 'absolute', left: 0 }}
            >
              <ArrowBackIosNewIcon />
            </IconButton>
          )}

          <Box
            key={`page-${currentPage}`}
            sx={{
              display: 'flex',
              justifyContent: 'center',
              gap: 3,
              width: '100%',
              maxWidth: '800px',
              margin: '0 40px'
            }}
          >
            {getCurrentPagePlayers.map((player, index) => (
              <Tooltip title={player.name} arrow key={`${player.id}-${currentPage}-${index}`}>
                <IconButton
                  key={`${player.id}-${currentPage}-${index}`}
                  onClick={() => handlePlayerSelect(player)}
                  sx={{
                    width: 80,
                    height: 80,
                    border: selectedPlayer?.id === player.id ? '2px solid #00CED1' : '2px solid #e0e0e0',
                    borderRadius: '50%',
                    '&:hover': {
                      backgroundColor: 'rgba(0, 0, 0, 0.04)',
                      transform: 'scale(1.1)'
                    }

                  }}
                >
                  <Avatar
                    sx={{
                      width: 60,
                      height: 60,
                      bgcolor: '#00CED1'
                    }}
                  >
                    {player.avatar}
                  </Avatar>
                </IconButton>
              </Tooltip>
            ))}
          </Box>

          {filteredPlayers.length > playersPerPage && (
            <IconButton
              onClick={handleNextPage}
              disabled={currentPage >= Math.ceil(filteredPlayers.length / playersPerPage) - 1}
              sx={{ position: 'absolute', right: 0 }}
            >
              <ArrowForwardIosIcon />
            </IconButton>
          )}
        </Box>

        <Box sx={{ display: 'flex', justifyContent: 'center', gap: 4 }}>
          <Button
            variant="outlined"
            disabled={!selectedPlayer}
            onClick={SetLocationToSessions}
          >
            View Statistics
          </Button>
          <Button
            variant="outlined"
            disabled={!selectedPlayer}
            onClick={SetLocationToProgress}
          >
            View progress
          </Button>
        </Box>
      </Container>
    </Box>
  );
};

export default PlayerSection;
