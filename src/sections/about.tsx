import { 
    Typography, 
    Box, 
    Container,
    Paper,
  } from '@mui/material';
  import Navbar from '../components/navBar';

  const About = (): JSX.Element => {
    return (
        <Box>
            <Box>
                <Navbar/>
            </Box>  
            <Box
              sx={{
                backgroundSize: 'cover',
                backgroundPosition: 'center',
                height: '200px', 
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
              Sobre nosotros
            </Typography>
            <Typography variant="subtitle1">
            Aprende más información sobre nuestra plataforma de estadísticas de balonmano 
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
                Nuestra Misión
              </Typography>
              <Typography variant="body1">
              El propósito de esta app web es proporcionar estadísticas y análisis completos de balonmano para jugadores y entrenadores. Nuestra plataforma ayuda a los equipos a mejorar su rendimiento a través de información basada en datos.
              </Typography>
            </Box>
  
            <Box>
              <Typography variant="h5" gutterBottom>
              Características de la plataforma
              </Typography>
              <Box sx={{ display: 'flex', gap: 3, flexWrap: 'wrap' }}>
                <Paper sx={{ p: 2, flex: 1, minWidth: 250, bgcolor: '#00CED1CC' }}>
                  <Typography variant="h6">Área de jugadores</Typography>
                  <Typography>Acceso a estadísticas personales y datos de rendimiento</Typography>
                </Paper>
                <Paper sx={{ p: 2, flex: 1, minWidth: 250, bgcolor: '#00FF44CC' }}>
                  <Typography variant="h6">Área de entrenadores</Typography>
                  <Typography>Herramientas de gestión y análisis de equipos</Typography>
                </Paper>
              </Box>
            </Box>
  
            <Box>
              <Typography variant="h5" gutterBottom>
                Contáctanos
              </Typography>
              <Typography variant="body1">
              ¿Tienes dudas o sugerencias? Ponte en contacto con nosotros para obtener ayuda e información.
              </Typography>
            </Box>
          </Paper>
        </Container>
      </Box>
    );
  };
  
  export default About;
  