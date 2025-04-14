import { JSX, useState } from 'react';
import { useLocation } from 'wouter';
import { Button, Container, Paper, Typography } from '@mui/material';
import { TextInput } from '@mantine/core';
import { baseURL } from '../components/utils';

const ResetPassword = (): JSX.Element => {
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [, setLocation] = useLocation();

  const handleSubmit = async () => {
    setError('');

    if (await userExists()) {
      if (password.length < 8) {
        setError('Password must be at least 8 characters');
        return;
      }

      if (password !== confirmPassword) {
        setError('Passwords do not match');
        return;
      }

      createNewPassword()
    }
  };

  const userExists = async () => {
    try {
      const response = await fetch(`${baseURL}/user/${email}`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
      });
      const data = await response.json();
      if (response.ok) {
        return true;
      } else {
        setError(data.detail || 'Email not found');
        return false;
      }
    } catch (err) {
      setError('Failed to check email existence');
      return false;
    }
  }

  const createNewPassword = async () => {
    try {
      const salt = CryptoJS.lib.WordArray.random(16).toString();
      const hashedPassword = CryptoJS.SHA256(password + salt).toString();

      const response = await fetch(`${baseURL}/reset-password`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email,
          password: hashedPassword,
          salt
        })
      });

      const data = await response.json();

      if (response.ok) {
        alert('Password reset successfully!');
        setLocation('/home', {
          state: { mail: email }
        });
      } else {
        alert(data.detail || 'Error resetting password');
      }
    } catch (err) {
      setError('Failed to reset password');
    }
  }

  return (
    <Container maxWidth="xs">
      <Paper elevation={3} sx={{ p: 3, mt: 8 }}>
        <Typography variant="h5" gutterBottom align="center" sx={{ mb: 2 }}>
          Set the new password
        </Typography>

        <TextInput
          required
          label="Email"
          name='email'
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          error={!!error}
          placeholder="you@example.com"
          mb="md"
        />

        <TextInput
          required
          label="New Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <TextInput
          required
          label="Confirm Password"
          type="password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          error={!!error}
        />

        <div style={{ marginBottom: '16px' }}></div>
        <Button
          fullWidth
          variant="contained"
          onClick={handleSubmit}
          sx={{ mb: 2 }}
        >
          Reset
        </Button>

        <Button
          fullWidth
          onClick={() => setLocation('/')}
        >
          Back to Initial Page
        </Button>
      </Paper>
    </Container>
  );
};

export default ResetPassword;
