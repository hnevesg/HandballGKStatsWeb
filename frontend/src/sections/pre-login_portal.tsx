import { Container, Grid, Card, CardActionArea, Typography } from '@mui/material';
// Cambiar wouter por react-router-dom
import PersonAddIcon from '@mui/icons-material/PersonAdd';
import GroupsIcon from '@mui/icons-material/Groups';
import SportsHandballIcon from '@mui/icons-material/SportsHandball';
import TuneIcon from '@mui/icons-material/Tune';

const Portal = (): JSX.Element => {
  const cardData = [
    {
      title: 'Create Account',
      color: '#FFD700CC',
      icon: <PersonAddIcon sx={{ fontSize: 40, color: 'white' }} />,
      path: '/register'
    },
    {
      title: 'Players\' Private Area',
      color: '#00CED1CC',
      icon: <SportsHandballIcon sx={{ fontSize: 40, color: 'white' }} />,
      path: '/loginplayers'
    },
    {
      title: 'Staff Private Area',
      color: '#00FF44CC',
      icon: <GroupsIcon sx={{ fontSize: 40, color: 'white' }} />,
      path: '/loginstaff'
    },
    {
      title: 'Admin Area',
      color: '#A9A9A9CC',
      icon: <TuneIcon sx={{ fontSize: 40, color: 'white' }} />,
      path: '/loginadmin'
    }
  ];

  return (
    <Container maxWidth="lg"
      sx={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center'
      }}
    >
      <Grid container spacing={3} justifyContent="center" sx={{ py: 4 }}>
        {cardData.map((card, index) => (
          <Grid item xs={12} sm={6} md={5} key={index}>
            <Card
              sx={{
                height: 200,
                backgroundColor: card.color,
                color: 'white',
                borderRadius: 2,
                position: 'relative',
                overflow: 'hidden',
                cursor: 'pointer',
                transition: 'all 0.3s ease',
                '&:hover': {
                  transform: 'scale(1.05)',
                  boxShadow: '0 8px 16px rgba(0,0,0,0.2)'
                }
              }}
            >
              <CardActionArea
                onClick={() => window.location.href = card.path}
                sx={{
                  height: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  justifyContent: 'center',
                  alignItems: 'center',
                  gap: 2
                }}
              >
                {card.icon}
                <Typography
                  variant="h6"
                  component="div"
                  sx={{
                    textAlign: 'center',
                    px: 2
                  }}
                >
                  {card.title}
                </Typography>
              </CardActionArea>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Container>
  );
};

export default Portal;
