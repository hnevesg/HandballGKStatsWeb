import { Box, Container, Typography, TextField, Button, Avatar, IconButton, Tooltip } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import { useState, useMemo, useEffect } from 'react';
import { useLocation } from 'wouter'
import { Player } from '../../types/player';
import ArrowBackIosNewIcon from '@mui/icons-material/ArrowBackIosNew';
import ArrowForwardIosIcon from '@mui/icons-material/ArrowForwardIos';
import Navbar from '../../components/navBar';

const PlayerSection = (): JSX.Element => {
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedPlayer, setSelectedPlayer] = useState<Player | null>(null);
  const [, navigate] = useLocation();
  const [currentPage, setCurrentPage] = useState(0);
  const [players, setPlayers] = useState<Player[]>([]);
  const playersPerPage = 3;

  const filteredPlayers = useMemo(() => {
    return players.filter(player =>
      player.name.toLowerCase().includes(searchQuery.toLowerCase())
    );
  }, [players, searchQuery]);

  const handlePlayerSelect = (player: Player) => {
    setSelectedPlayer(player);
  };

  const SetLocationToSessions = () => {
    if (selectedPlayer) {
      navigate("/player-sessions", {
        state: { player: selectedPlayer }
      });
    }
  };

  const SetLocationToProgress = () => {
    if (selectedPlayer) {
      navigate("/player-progress", {
        state: { player: selectedPlayer }
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

  const fetchData = async () => {
    try {
      const response = await fetch('http://localhost:8000/api/players', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      const data = await response.json();

      if (response.ok) {
        const playersWithAvatars = data.map((player: Player, index: number) => {
          const [firstName = '', lastName = ''] = player.name.split(' ');
          const avatar = `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
          return { ...player, id: index + 1, avatar };
        });

        setPlayers(playersWithAvatars);
      } else {
        console.error('Failed to fetch players:', data.message || response.statusText);
      }
    } catch (error) {
      console.error('Error fetching players:', error);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <Box>
      <Navbar />
      <Container maxWidth="md" sx={{ mt: 8 }}>
        <Typography variant="h4" sx={{ textAlign: 'center', mb: 4 }}>
          Selecciona un jugador
        </Typography>

        <Box sx={{ display: 'flex', justifyContent: 'center', mb: 6 }}>
          <TextField
            placeholder="Buscar jugadores..."
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
            Ver estadísticas
          </Button>
          <Button
            variant="outlined"
            disabled={!selectedPlayer}
            onClick={SetLocationToProgress}
          >
            Ver progreso
          </Button>
        </Box>
      </Container>
    </Box>
  );
};

export default PlayerSection;
