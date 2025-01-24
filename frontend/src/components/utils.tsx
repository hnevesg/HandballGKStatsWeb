export const formatDate = (dateString: string) => {
  return dateString.replace('T', ' ');
};

import { Box, Avatar, Typography, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper } from '@mui/material';
import { Session } from '../types/session';

export const PlayerTable = ({
  playerName,
  playerAvatar,
  playerSessions,
  selectedSession,
  setSelectedSession
}: {
  playerName: string;
  playerAvatar: string;
  playerSessions: Session[];
  selectedSession: number | null;
  setSelectedSession: (sessionId: number) => void;
}) => {
  return (
    <Box sx={{ flex: 1 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
        <Avatar sx={{ mr: 2, width: 45, height: 45, bgcolor: '#00CED1' }}>{playerAvatar}</Avatar>
        <Typography variant="h6">{playerName}</Typography>
      </Box>

      <TableContainer component={Paper} sx={{ maxHeight: '28vh', overflow: 'auto' }}>
        <Table stickyHeader>
          <TableHead>
            <TableRow>
              <TableCell>Date</TableCell>
              <TableCell>Game Mode</TableCell>
              <TableCell>Difficulty</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {playerSessions.map((session) => (
              <TableRow
                key={session.id}
                onClick={() => setSelectedSession(session.id)}
                sx={{
                  cursor: 'pointer',
                  backgroundColor: selectedSession === session.id ? 'rgb(0, 206, 209)' : 'inherit',
                  '&:hover': {
                    backgroundColor: selectedSession === session.id ? 'rgb(0, 206, 209)' : 'inherit',
                  }
                }}
              >
                <TableCell>{formatDate(session.date)}</TableCell>
                <TableCell>{session.game_mode}</TableCell>
                <TableCell>{session.prestige_level}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
};
