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

const Register = () => {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleRegister = async () => {
    // Registration logic
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
          Register
        </Title>
      </Box>

      <Box>
        <TextInput
          label="Name"
          placeholder="Your name"
          required
          value={name}
          onChange={(e) => setName(e.target.value)}
          mb="md"
        />

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

        <Button
          fullWidth
          onClick={handleRegister}
          color="blue"
          size="md"
          mb="sm"
        >
          Register
        </Button>

        <Group >
          <Text size="sm">
            Already have an account? <Link to="/login">Login</Link>
          </Text>
        </Group>
      </Box>
    </Container>
  );
};

export default Register;
