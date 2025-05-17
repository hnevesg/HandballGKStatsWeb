
import { Box, Avatar, Typography, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper } from '@mui/material';
import { Session } from '../types/session';
import html2canvas from "html2canvas";
import jsPDF from "jspdf";
import { useState } from 'react';

export const baseURL = 'https://gkstatsweb.duckdns.org:12345';//'https://192.168.43.173:12345';
export const streamingURL = 'wss://gkstatsweb.duckdns.org:12345/webrtc-signaling';

export const formatDate = (dateString: string) => {
  return dateString.replace('T', ' ');
};

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

export const PDFExporter = () => {
    const [exporting, setExporting] = useState(false);

    const exportPDF = async (element: HTMLElement, filename = 'export.pdf') => {
        if (!element) return;

        setExporting(true);
        try {
            const canvas = await html2canvas(element, {
                useCORS: true,
                allowTaint: true,
                scale: 2
            });

            const imgData = canvas.toDataURL('image/png');
            const pdf = new jsPDF('p', 'mm', 'a4');
            const pdfWidth = pdf.internal.pageSize.getWidth();
            const pdfHeight = pdf.internal.pageSize.getHeight();

            const canvasWidth = canvas.width;
            const canvasHeight = canvas.height;
            const imgHeight = (canvasHeight * pdfWidth) / canvasWidth;

            let heightLeft = imgHeight;
            let position = 0;

            pdf.addImage(imgData, 'PNG', 0, position, pdfWidth, imgHeight);
            heightLeft -= pdfHeight;

            while (heightLeft > 0) {
                position = heightLeft - imgHeight;
                pdf.addPage();
                pdf.addImage(imgData, 'PNG', 0, position, pdfWidth, imgHeight);
                heightLeft -= pdfHeight;
            }

            pdf.save(filename);
        } finally {
            setExporting(false);
        }
    };

    return { exporting, exportPDF };
};