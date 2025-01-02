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
import { useLocation } from 'wouter';

interface Player {
  id: number;
  name: string;
  avatar: string;
}

const PlayersComparison = (): JSX.Element => {
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedPlayers, setSelectedPlayers] = useState<(Player | null)[]>([null, null]);
  const [activeField, setActiveField] = useState<number | null>(null);
  const [, setLocation] = useLocation();

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
        state: { player1: selectedPlayers[0], player2: selectedPlayers[1] }
      });
    }
  };

  return (
    <Box>
      <Navbar />
      <Container maxWidth="md" sx={{ mt: 8 }}>
        <Typography variant="h4" sx={{ textAlign: 'center', mb: 4 }}>
          Comparación de Jugadores
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

        <Box sx={{
          display: 'flex',
          justifyContent: 'space-around',
          mb: 4
        }}>
          <TextField
            label="Jugador 1"
            value={selectedPlayers[0]?.name || ''}
            onClick={() => setActiveField(0)}
            sx={{
              '& .MuiOutlinedInput-root': {
                borderColor: activeField === 0 ? '#00CED1' : 'inherit'
              }
            }}
          />
          <TextField
            label="Jugador 2"
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
            Continuar
          </Button>
        </Box>
      </Container>
    </Box>
  );
};

export default PlayersComparison;
