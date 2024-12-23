import {
  Typography,
  Box,
  Button,
  Container,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper
} from '@mui/material';
import Navbar from '../components/navBar';

const Home = (): JSX.Element => {
  const sessions = [
    { id: 57, usuario: 'ttt', fecha: '19/08/23' },
    { id: 56, usuario: 'zzz', fecha: '01/07/23' },
    { id: 55, usuario: 'aaa', fecha: '22/06/23' },
    { id: 54, usuario: 'bbb', fecha: '17/04/23' },
    { id: 53, usuario: 'aaa', fecha: '01/10/23' }
  ];

  return (
    <Box>
      <Box>
        <Navbar />
      </Box>
      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Box sx={{
          backgroundImage: 'url("/cat.jpg")',
          backgroundSize: 'cover',
          backgroundPosition: 'center',
          height: '400px',
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          alignItems: 'center',
          color: 'white',
          textAlign: 'center',
          mb: 6
        }}
        >
          <Typography variant="h3" gutterBottom>
            Bienvenid@ a Handball GK Stats Web!
          </Typography>
          <Typography variant="subtitle1" gutterBottom>
            Te gustaría saber más sobre el juego o los creadores?
          </Typography>
          <Button
            variant="contained"
            sx={{
              backgroundColor: 'black',
              color: 'white',
              mt: 2
            }}
          >
            Learn more
          </Button>
        </Box>

        <Container maxWidth="md" sx={{ mb: 6 }}>
          <TableContainer
            component={Paper}
            sx={{
              width: '100%',
              maxWidth: 600,
              mx: 'auto',
              boxShadow: 2
            }}
          >
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>ID Session</TableCell>
                  <TableCell>Usuario</TableCell>
                  <TableCell>Fecha</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {sessions.map((session) => (
                  <TableRow key={session.id}>
                    <TableCell>{session.id}</TableCell>
                    <TableCell>{session.usuario}</TableCell>
                    <TableCell>{session.fecha}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Container>

      </Container>
    </Box>
  );
};

export default Home;
