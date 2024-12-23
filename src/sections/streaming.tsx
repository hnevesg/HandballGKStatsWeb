import {
  Box,
  Container,
  Typography,
  Paper
} from '@mui/material';
import Navbar from '../components/navBar';

const Streaming = (): JSX.Element => {
  return (
    <Box>
      <Navbar />

      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Box sx={{
          display: 'flex',
          flexDirection: 'row',
          gap: 4
        }}>

          {/* Technical Data container */}
          <Paper
            elevation={2}
            sx={{
              p: 3,
              maxWidth: 300,
              width: '100%',
              textAlign: 'center',
              alignSelf: 'flex-start',
              ml: 0
            }}
          >
            <Typography variant="h6" gutterBottom>
              Información Técnica
            </Typography>
            <Box sx={{ display: 'grid', gridTemplateColumns: 'auto 1fr', gap: 2 }}>
              <Typography variant="body1" fontWeight="bold">Bitrate:</Typography>
              <Typography variant="body1">3000 kbps</Typography>

              <Typography variant="body1" fontWeight="bold">Resolución:</Typography>
              <Typography variant="body1">1920x1080</Typography>

              <Typography variant="body1" fontWeight="bold">Codec:</Typography>
              <Typography variant="body1">H.264</Typography>

              <Typography variant="body1" fontWeight="bold">Frame Rate:</Typography>
              <Typography variant="body1">30 fps</Typography>
            </Box>
          </Paper>

          {/* Video container */}
          <Paper
            elevation={3}
            sx={{
              width: '100%',
              bgcolor: 'black',
              aspectRatio: '16/9',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              flex: 8
            }}
          >
            <video
              controls
              style={{ width: '100%', height: '100%' }}
            >
              <source src="your-video-source.mp4" type="video/mp4" />
              Your browser does not support the video tag.
            </video>
          </Paper>

        </Box>
      </Container>

    </Box>
  );
};

export default Streaming;
