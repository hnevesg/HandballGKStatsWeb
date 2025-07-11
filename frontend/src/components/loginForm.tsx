import { useState, useEffect } from 'react';
import {
    Button, Box, Avatar, Typography, Container, Paper, IconButton, Link
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { TextInput } from '@mantine/core';
import { useLocation } from 'wouter';

interface LoginFormProps {
    endpoint: string;
    avatarIcon: JSX.Element;
    avatarColor: string;
    showForgotPassword?: boolean;
}

export default function LoginForm({ endpoint, avatarIcon, avatarColor, showForgotPassword = true }: LoginFormProps) {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [emailError, setEmailError] = useState('');
    const [passwordError, setPasswordError] = useState('');
    const [loginError, setLoginError] = useState('');
    const [, setLocation] = useLocation();

    const handleLogin = async () => {
        setEmailError('');
        setPasswordError('');
        setLoginError('');

        if (!email) setEmailError('Email is required');
        if (!password) setPasswordError('Password is required');
        if (email && password) fetchData();
    };

    const fetchData = async () => {
        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password }),
            });
            const data = await response.json();
            if (response.ok) {
                setLocation('/home', { state: { mail: email } });
            } else {
                setLoginError(data.detail || 'Login failed');
            }
        } catch (error) {
            console.error('Error:', error);
            setLoginError('An error occurred');
        }
    };

    useEffect(() => {
        const handleKeyDown = (event: KeyboardEvent) => {
            if (event.key === 'Enter') handleLogin();
        };
        window.addEventListener('keydown', handleKeyDown);
        return () => window.removeEventListener('keydown', handleKeyDown);
    }, [email, password]);

    return (
        <Container maxWidth="sm">
            <Paper
                elevation={3}
                sx={{
                    mt: 8, p: 4, display: 'flex', flexDirection: 'column', alignItems: 'center',
                    backgroundColor: 'rgba(255, 255, 255, 0.9)', backdropFilter: 'blur(10px)', position: 'relative'
                }}
            >
                <IconButton
                    sx={{ position: 'absolute', left: 16, top: 16 }}
                    onClick={() => setLocation('/')}
                >
                    <ArrowBackIcon />
                </IconButton>

                <Avatar sx={{ m: 1, bgcolor: avatarColor, width: 56, height: 56 }}>
                    {avatarIcon}
                </Avatar>

                <Typography component="h1" variant="h5" sx={{ mb: 3 }}>
                    Sign In
                </Typography>

                <Box component="form" sx={{ mt: 1, width: '100%' }}>
                    <TextInput
                        required label="Email" name="email" autoComplete="email"
                        value={email} onChange={(e) => setEmail(e.target.value)}
                        error={!!emailError} placeholder="you@example.com" mb="md"
                    />
                    <TextInput
                        required name="password" label="Password" type="password" autoComplete="new-password"
                        value={password} onChange={(e) => setPassword(e.target.value)}
                        error={!!passwordError} placeholder="Your password" mb="md"
                    />

                    {showForgotPassword && ( // Conditionally render the link
                        <Typography variant="body2" sx={{ mt: 2 }}>
                            <Link component="button" onClick={() => setLocation('/forgot-password')}>
                                Forgot your password?
                            </Link>
                        </Typography>
                    )}

                    <Button
                        fullWidth variant="contained" onClick={handleLogin}
                        sx={{
                            mt: 3, mb: 2, bgcolor: avatarColor,
                            '&:hover': { bgcolor: '#40E0D0' }
                        }}
                    >
                        Continue
                    </Button>
                    {loginError && <p>{loginError}</p>}
                </Box>
            </Paper>
        </Container>
    );
}