import TuneIcon from '@mui/icons-material/Tune';
import LoginForm from '../components/loginForm';
import { baseURL } from '../components/utils';

export default function LoginAdmin() {
  return (
    <LoginForm
      endpoint={`${baseURL}/login-admin`}
      avatarIcon={<TuneIcon />}
      avatarColor="#333333"
      showForgotPassword={false}
    />
  );
}