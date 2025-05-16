// pages/LoginPlayers.tsx
import SportsHandballIcon from '@mui/icons-material/SportsHandball';
import LoginForm from '../components/loginForm';
import { baseURL } from '../components/utils';

export default function LoginPlayers() {
  return (
    <LoginForm
      endpoint={`${baseURL}/login-players`}
      avatarIcon={<SportsHandballIcon />}
      avatarColor="#FFD54F"
    />
  );
}