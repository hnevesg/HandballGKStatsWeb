import { 
    Box, 
    Container, 
    Typography, 
    Avatar,
    IconButton,
    Paper,
  } from '@mui/material';
  import ArrowBackIcon from '@mui/icons-material/ArrowBack';
  import { useLocation } from 'wouter';
import Navbar from '../../components/navBar';
import { useEffect, useState } from 'react';
import { Player } from '../../types/player';
  
  const ComparisonDetails = (): JSX.Element => {
    const [, navigate] = useLocation();
    const [player1, setPlayer1] = useState<Player | null>(null);
    const [player2, setPlayer2] = useState<Player | null>(null);
  
    useEffect(() => {
        const state = window.history.state;
        if (state?.player1) {
          setPlayer1(state.player1);
        }
        if (state?.player2) {
          setPlayer2(state.player2);
        }
      }, []);
      
    return (
      <Box>
        <Navbar />
        <Container maxWidth="lg" sx={{ mt: 4 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', mb: 6 }}>
            <IconButton 
              onClick={() => navigate('/players-comparison')}
              sx={{ mr: 2 }}
            >
              <ArrowBackIcon />
            </IconButton>
            <Typography variant="h4">Players Comparison</Typography>
          </Box>
  
          <Box sx={{ 
            display: 'flex', 
            justifyContent: 'center', 
            alignItems: 'center',
            gap: 2,
            mb: 6 
          }}>
            <Box sx={{ textAlign: 'center' }}>
              <Avatar 
                sx={{ 
                  width: 60, 
                  height: 60,
                  bgcolor: '#00CED1',
                  mb: 1 
                }}
              >
                {player1?.avatar}
              </Avatar>
            {/*  <Typography>{player1?.name}</Typography>*/}
            </Box>
  
            <Typography variant="h6" sx={{ mx: 2 }}>vs</Typography>
  
            <Box sx={{ textAlign: 'center' }}>
              <Avatar 
                sx={{ 
                  width: 60, 
                  height: 60,
                  bgcolor: '#00CED1',
                  mb: 1 
                }}
              >
                {player2?.avatar}
              </Avatar>
             {/* <Typography>{player2?.name}</Typography>*/}
            </Box>
          </Box>
  
          <Box sx={{ 
            display: 'flex',
            justifyContent: 'space-between',
            gap: 4
          }}>
            {/* Metric 1 */}
            <Paper sx={{ flex: 1, p: 3 }}>
              <Typography variant="h6" gutterBottom>
                Metric 1
              </Typography>
              <Box sx={{ 
                height: 300,
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center'
              }}>
                {/* Add your chart component here */}
              </Box>
            </Paper>
  
            {/* Metric 2 */}
            <Paper sx={{ flex: 1, p: 3 }}>
              <Typography variant="h6" gutterBottom>
                Metric 2
              </Typography>
              <Box sx={{ 
                height: 300,
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center'
              }}>
                {/* Add your chart component here */}
              </Box>
            </Paper>
          </Box>
        </Container>
      </Box>
    );
  };
  
  export default ComparisonDetails;
  