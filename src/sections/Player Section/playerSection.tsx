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
import { useState, useMemo } from 'react';
import Navbar from '../../components/navBar';
import { useLocation } from 'wouter'
import { Player } from '../../types/player';

const PlayerSection = (): JSX.Element => {
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedPlayer, setSelectedPlayer] = useState<Player | null>(null);
  const [, navigate] = useLocation();

  const players: Player[] = [
    { id: 1, name: 'Alex Smith', avatar: 'AS' },
    { id: 2, name: 'John Doe', avatar: 'JD' },
    { id: 3, name: 'Mike Johnson', avatar: 'MJ' },
    { id: 4, name: 'Sarah Wilson', avatar: 'SW' }
  ];

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
          flexWrap: 'wrap',
          gap: 3,
          mb: 6
        }}>
          {filteredPlayers.map((player) => (
            <IconButton
              key={player.id}
              onClick={() => handlePlayerSelect(player)}
              sx={{
                width: 80,
                height: 80,
                border: '2px solid #e0e0e0',
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
