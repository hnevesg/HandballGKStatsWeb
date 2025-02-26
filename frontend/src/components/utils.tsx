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
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', mb: 2 }}>
        <Avatar sx={{ mr: 2, width: 45, height: 45, bgcolor: '#00CED1' }}>{playerAvatar}</Avatar>
        <Typography variant="h6">{playerName}</Typography>
      </Box>

      {playerSessions.length > 0 ? (
        <TableContainer component={Paper} sx={{ maxHeight: '28vh', overflow: 'auto', border: '1px solid black', alignContent: 'center', margin: 'auto' }}>
          <Table stickyHeader>
            <TableHead>
              <TableRow>
                <TableCell sx={{ borderBottom: '1px solid black', textAlign: 'center' }}>Date</TableCell>
                <TableCell sx={{ borderBottom: '1px solid black', textAlign: 'center' }}>Session ID</TableCell>
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
                  <TableCell sx={{ textAlign: 'center' }}>{formatDate(session.date)}</TableCell>
                  <TableCell sx={{ textAlign: 'center' }}>{session.id}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      ) : (
        <Typography variant="body1" align='center' color="error" sx={{
          backgroundColor: '#fce4ec',
          border: '1px solid #f8bbd0',
          borderRadius: '8px',
          padding: '12px 16px',
          marginBottom: '24px',
          fontWeight: 600,
          boxShadow: '0px 4px 6px rgba(0, 0, 0, 0.1)',
        }}>No sessions available</Typography>
      )}
    </Box>
  );
};
