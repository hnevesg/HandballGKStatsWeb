// pages/LoginStaff.tsx
import GroupsIcon from '@mui/icons-material/Groups';
import LoginForm from '../components/loginForm';
import { baseURL } from '../components/utils';

export default function LoginStaff() {
  return (
    <LoginForm
      endpoint={`${baseURL}/login-staff`}
      avatarIcon={<GroupsIcon />}
      avatarColor="#00FF44CC"
    />
  );
}