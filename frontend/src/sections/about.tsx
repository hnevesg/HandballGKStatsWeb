import {
  Typography,
  Box,
  Container,
  Paper,
} from '@mui/material';
import { useEffect, useState } from 'react';
import Navbar from '../components/navBar';
import { User } from '../types/user';

const About = (): JSX.Element => {
  const [loggedUser, setLoggedUser] = useState<User | null>(null);

  useEffect(() => {
    getUser();
  }, []);

  const getUser = async () => {
    const state = window.history.state;
    const response = await fetch(`http://localhost:8000/api/user/${state?.mail}`);
    if (response.ok) {
      const data = await response.json();
      setLoggedUser(data);
    }
  }

  return (
    <Box>
      <Box>
        <Navbar user={loggedUser} />
      </Box>
      <Box
        sx={{
          backgroundSize: 'cover',
          backgroundPosition: 'center',
          height: '140px',
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          alignItems: 'center',
          color: 'black',
          textAlign: 'center',
          mb: 3
        }}
      >
        <Typography variant="h3" gutterBottom>
          About us
        </Typography>
        <Typography variant="h5">
          Learn more about our handball statistics platform:
        </Typography>
      </Box>

      <Container maxWidth="md" sx={{ mb: 6 }}>
        <Paper
          elevation={3}
          sx={{
            p: 4,
            display: 'flex',
            flexDirection: 'column',
            gap: 3
          }}
        >

          <Box>
            <Typography variant="h5" gutterBottom>
              ● Our mission
            </Typography>
            <Typography variant="body1">
              The purpose of this web app is to provide statistics and complete handball analisys for goalkeepers and staff. We believe that this information, based on the data from sessions, will help improve the temas' performance.
            </Typography>
          </Box>

          <Box>
            <Typography variant="h5" gutterBottom>
              ● Platform Characteristics
            </Typography>
            <Box sx={{ display: 'flex', gap: 3, flexWrap: 'wrap' }}>
              <Paper sx={{ p: 2, flex: 1, bgcolor: '#00CED1CC' }}>
                <Typography variant="h6" align='center'>Player's Area</Typography>
                <Typography align='center'>Access to personal statistics & performance data</Typography>
              </Paper>
              <Paper sx={{ p: 2, flex: 1, bgcolor: '#00FF44CC' }}>
                <Typography variant="h6" align='center'>Staff Area</Typography>
                <Typography align='center'>Access to club's goalkeepers statistics, comparisons & performance data</Typography>
              </Paper>
            </Box>
          </Box>

          <Box>
            <Typography variant="h5" gutterBottom>
              ● Contact Us
            </Typography>
            <Typography variant="body1">
              Any question or tip? Contact us through our <a href="mailto:  @helena.neves@alu.uclm.es">e-mail address </a> 
            </Typography>
          </Box>
        </Paper>
      </Container>
    </Box>
  );
};

export default About;
