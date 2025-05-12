import TuneIcon from '@mui/icons-material/Tune';
import LoginForm from '../components/loginForm';
import { baseURL } from '../components/utils';

export default function LoginAdmin() {
  return (
    <LoginForm
      endpoint={`${baseURL}/login-admin`}
      avatarIcon={<TuneIcon />}
      avatarColor="#A9A9A9CC"
      showForgotPassword={false}
    />
  );
}