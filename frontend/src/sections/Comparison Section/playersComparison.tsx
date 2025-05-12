import {
  Box,
  Container,
  Typography,
  TextField,
  Button,
  Avatar,
  IconButton
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import { useState, useMemo, useEffect } from 'react';
import Navbar from '../../components/navBar';
import { useLocation } from 'wouter';
import ArrowBackIosNewIcon from '@mui/icons-material/ArrowBackIosNew';
import ArrowForwardIosIcon from '@mui/icons-material/ArrowForwardIos';
import { User } from '../../types/user';
import { Rol } from '../../types/rol';
import { baseURL } from '../../components/utils';


const PlayersComparison = (): JSX.Element => {
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedPlayers, setSelectedPlayers] = useState<(User | null)[]>([null, null]);
  const [activeField, setActiveField] = useState<number | null>(null);
  const [, setLocation] = useLocation();
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
    if (selectedPlayers.some(p => p?.id === player.id)) return;

    if (activeField !== null) {
      const newPlayers = [...selectedPlayers];
      newPlayers[activeField] = player;
      setSelectedPlayers(newPlayers);
    } else {
      if (!selectedPlayers[0]) {
        setSelectedPlayers([player, selectedPlayers[1]]);
      } else if (!selectedPlayers[1]) {
        setSelectedPlayers([selectedPlayers[0], player]);
      }
    }
  };

  const SetLocationToSessions = () => {
    if (selectedPlayers[0] && selectedPlayers[1]) {
      setLocation("/players-sessions", {
        state: { player1: selectedPlayers[0], player2: selectedPlayers[1], user: loggedUser }
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
    if (loggedUser?.role != Rol.ENTRENADOR && loggedUser?.role != Rol.ADMINISTRADOR) return;
    try {
      if (loggedUser?.role == Rol.ENTRENADOR) {
        const response = await fetch(`${baseURL}/players/${loggedUser?.teamID}`);
        const data = await response.json();

        if (response.ok) {
          const playersWithAvatars = data.map((player: User) => {
            const [firstName = '', lastName = ''] = player.name.split(' ');
            const avatar = `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
            return { ...player, avatar };
          });

          setPlayers(playersWithAvatars);
        } else {
          console.error('Failed to fetch players:', data.message || response.statusText);
        }
      } else {
        const response = await fetch(`${baseURL}/players`);
        const data = await response.json();

        if (response.ok) {
          const playersWithAvatars = data.map((player: User) => {
            const [firstName = '', lastName = ''] = player.name.split(' ');
            const avatar = `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
            return { ...player, avatar };
          });

          setPlayers(playersWithAvatars);
        } else {
          console.error('Failed to fetch players:', data.message || response.statusText);
        }
      }
    } catch (error) {
      console.error('Error fetching players:', error);
    }
  };

  useEffect(() => {
    getPlayers();
  }, [loggedUser]);

  return (
    <Box>
      <Navbar user={loggedUser} />
      <Container maxWidth="md" sx={{ mt: 8 }}>
        <Typography variant="h4" sx={{ textAlign: 'center', mb: 4 }}>
          Players Comparison
        </Typography>

        {loggedUser?.role === Rol.PORTERO && (
          <Typography variant="body1" align='center' color="error" sx={{
            backgroundColor: '#fce4ec',
            border: '1px solid #f8bbd0',
            borderRadius: '8px',
            padding: '12px 16px',
            marginBottom: '24px',
            fontWeight: 600,
            boxShadow: '0px 4px 6px rgba(0, 0, 0, 0.1)',
          }}>Since you are a player, you don't have access to this section.</Typography>
        )}

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
              <IconButton
                key={`${player.id}-${currentPage}-${index}`}
                onClick={() => handlePlayerSelect(player)}
                sx={{
                  width: 80,
                  height: 80,
                  border: selectedPlayers[0]?.id === player.id ? '2px solid #00CED1' : selectedPlayers[1]?.id === player.id ? '2px solid #00CED1' : '2px solid #e0e0e0',
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

        <Box sx={{
          display: 'flex',
          justifyContent: 'space-around',
          mb: 4
        }}>
          <TextField
            label="Player 1"
            value={selectedPlayers[0]?.name || ''}
            onClick={() => setActiveField(0)}
            sx={{
              '& .MuiOutlinedInput-root': {
                borderColor: activeField === 0 ? '#00CED1' : 'inherit'
              }
            }}
          />
          <TextField
            label="Player 2"
            value={selectedPlayers[1]?.name || ''}
            onClick={() => setActiveField(1)}
            sx={{
              '& .MuiOutlinedInput-root': {
                borderColor: activeField === 1 ? '#00CED1' : 'inherit'
              }
            }}
          />
        </Box>

        <Box sx={{ display: 'flex', justifyContent: 'center' }}>
          <Button
            variant="contained"
            disabled={!selectedPlayers[0] || !selectedPlayers[1]}
            onClick={SetLocationToSessions}
          >
            Continue
          </Button>
        </Box>
      </Container>
    </Box>
  );
};

export default PlayersComparison;
