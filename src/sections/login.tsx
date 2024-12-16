import { useState } from 'react';
import {
  TextInput,
  PasswordInput,
  Button,
  Box,
  Avatar,
  Title,
  Container,
  Group,
  Text,
} from '@mantine/core';
import { Lock } from 'lucide-react';
import { Link } from 'wouter';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = async () => {
    // Login logic
  };

  return (
    <Container size={420} my={40}>
      <Box
        style={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        <Avatar size={60} radius="xl" color="blue" mb={10}>
          <Lock size={30} />
        </Avatar>
        <Title order={2} mb="md">
          Login
        </Title>
      </Box>

      <Box>
        <TextInput
          label="Email Address"
          placeholder="you@example.com"
          required
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          mb="md"
        />

        <PasswordInput
          label="Password"
          placeholder="Your password"
          required
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          mb="lg"
        />

        <Button fullWidth onClick={handleLogin} color="blue" size="md" mb="sm">
          Login
        </Button>

        <Group>
          <Text size="sm">
            Don't have an account? <Link to="/register">Register</Link>
          </Text>
        </Group>
      </Box>
    </Container>
  );
};

export default Login;
