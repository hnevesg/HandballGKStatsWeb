import React, { useEffect, useState, useRef } from "react";
import { Box, Container, Typography, Paper } from '@mui/material';
import Navbar from '../components/navBar';
import { User } from '../types/user';
import { baseURL, streamingURL } from '../components/utils';

const Streaming: React.FC = () => {
    const videoRef = useRef<HTMLVideoElement>(null);
    let ws: WebSocket;

    const [loggedUser, setLoggedUser] = useState<User | null>(null);

    const [bitrate, setBitrate] = useState<string>('');
    const [resolution, setResolution] = useState<string>('');
    const [frameRate, setFrameRate] = useState<string>('');
    const [jitter, setJitter] = useState<string>('');
    const [packetLoss, setPacketLoss] = useState<string>('');
    const [codec, setCodec] = useState<string>('');
    let prevBytesReceived = 0;
    let prevTimestamp = 0;

    useEffect(() => {
        getUser();
    }, []);

    const getUser = async () => {
        const state = window.history.state;
        const response = await fetch(`${baseURL}/user/${state?.mail}`);
        if (response.ok) {
            const data = await response.json();
            setLoggedUser(data);
        }
    };

    const logStats = async (peerConnection: RTCPeerConnection) => {
        setInterval(async () => {
            const stats = await peerConnection.getStats();
            stats.forEach(report => {
                if (report.type === "inbound-rtp" && report.kind === "video") {
                    console.log("RECEIVER - Inbound video report:", report);

                    if ("bytesReceived" in report && "timestamp" in report) {
                        if (prevTimestamp) {
                            const timeDiff = (report.timestamp - prevTimestamp) / 1000; // Convert ms to seconds
                            const bytesDiff = report.bytesReceived - prevBytesReceived;
                            const bitrateKbps = (bytesDiff * 8) / timeDiff / 1000; // Convert to kbps
                            setBitrate(`${bitrateKbps.toFixed(2)} kbps`);
                        }
                        prevBytesReceived = report.bytesReceived;
                        prevTimestamp = report.timestamp;
                    }

                    if ("frameWidth" in report && "frameHeight" in report) {
                        setResolution(`${report.frameWidth}x${report.frameHeight}`);
                    }
                    if ("framesPerSecond" in report) {
                        setFrameRate(`${report.framesPerSecond} fps`);
                    }
                    if ("jitter" in report) {
                        setJitter(`${report.jitter}`);
                    }
                    if ("packetsLost" in report && "packetsReceived" in report) {
                        const totalPackets = report.packetsLost + report.packetsReceived;
                        if (totalPackets > 0) {
                            const packetLossPercent = (report.packetsLost / totalPackets) * 100;
                            setPacketLoss(`${packetLossPercent.toFixed(2)} %`);
                        } else {
                            setPacketLoss("0 %");
                        }
                    }
                }
                else if (report.type === "codec" && report.mimeType) {
                    setCodec(report.mimeType);
                }
            });
        }, 2000); // updates every 2 seconds
    };

    useEffect(() => {
        const peerConnection = new RTCPeerConnection();

        ws = new WebSocket(streamingURL);
        let ID = '';

        ws.onopen = () => {
            console.log('RECEIVER - WebSocket connection opened');
            logStats(peerConnection);
        };

        peerConnection.addTransceiver("video", { direction: "recvonly" });

        ws.onmessage = async (message) => {
            const data = JSON.parse(message.data);

            if (data.type === "welcome") {
                ID = data.senderId;

                const helloMsg = {
                    type: "hello",
                    senderId: ID,
                    targetId: 1
                };
                ws.send(JSON.stringify(helloMsg));
            } else if (data.type === "offer") {
                const offer = new RTCSessionDescription({ sdp: data.sdp, type: "offer" });

                await peerConnection.setRemoteDescription(offer);
                const answer = await peerConnection.createAnswer();
                await peerConnection.setLocalDescription(answer);

                peerConnection.onicegatheringstatechange = async () => {
                    if (peerConnection.iceGatheringState === "complete") {
                        console.log("RECEIVER - ICE gathering complete");
                        const localDescription = peerConnection.localDescription;
                        if (localDescription) {
                            const answerMsg = {
                                type: "answer",
                                sdp: localDescription.sdp,
                                senderId: ID,
                                targetId: 1
                            };
                            ws.send(JSON.stringify(answerMsg));
                            console.log("RECEIVER - Answer and ICE candidates sent together");
                        }
                    }
                };
            } else if (data.type === "candidate" && data.candidate) {
                const candidate = new RTCIceCandidate(data.candidate);
                await peerConnection.addIceCandidate(candidate);
            }
        };

        peerConnection.ontrack = (event) => {
            let streamToUse;

            if (event.streams.length === 0) {
                // new MediaStream with the received track
                streamToUse = new MediaStream([event.track]);
                console.log("RECEIVER - Created new MediaStream with received video track");
            } else {
                streamToUse = event.streams[0];
                console.log("RECEIVER - Using existing stream");
            }

            const video = videoRef.current;
            if (!video) {
                return;
            }
            video.srcObject = streamToUse;
            video.muted = true;
            video.autoplay = true;
            video.playsInline = true;

            video.onloadedmetadata = () => {
                video.play().then(() => {
                    console.log("RECEIVER - Video is playing");
                }).catch((err) => {
                    console.error("Video play error:", err);
                });
            };
        };

        return () => {
            ws.close();
            peerConnection.close();
        };
    }, []);

    return (
        <Box>
            <Navbar user={loggedUser} />
            <Container maxWidth="lg" sx={{ mt: 4 }}>
                <Box sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    '@media (orientation: landscape)': {
                        flexDirection: 'row',
                    },
                    gap: 4
                }}>

                    {/* Technical Data container */}
                    <Paper
                        elevation={2}
                        sx={{
                            p: { xs: 1, sm: 2 },
                            width: { xs: '100%', sm: 220, md: 260 },
                            maxWidth: { xs: '90%', sm: 220, md: 260 },
                            fontSize: { xs: '0.85rem', sm: '0.95rem', md: '1rem' },
                            textAlign: 'center',
                            alignSelf: 'center',
                            ml: 0
                        }}
                    >
                        <Typography variant="h6" gutterBottom sx={{ fontSize: { xs: '1rem', sm: '1.1rem', md: '1.25rem', fontWeight: 'bold' } }}>
                            Technical Information
                        </Typography>
                        <Box sx={{ display: 'grid', gridTemplateColumns: 'auto 1fr', gap: 2 }}>
                            <Typography variant="body1" fontWeight="bold" sx={{ fontSize: 'inherit' }}>Resolution:</Typography>
                            <Typography variant="body1">{resolution}</Typography>

                            <Typography variant="body1" fontWeight="bold" sx={{ fontSize: 'inherit' }}>Codec:</Typography>
                            <Typography variant="body1">{codec}</Typography>

                            <Typography variant="body1" fontWeight="bold" sx={{ fontSize: 'inherit' }}>Bitrate:</Typography>
                            <Typography variant="body1">{bitrate}</Typography>

                            <Typography variant="body1" fontWeight="bold" sx={{ fontSize: 'inherit' }}>Frame Rate:</Typography>
                            <Typography variant="body1">{frameRate}</Typography>

                            <Typography variant="body1" fontWeight="bold" sx={{ fontSize: 'inherit' }}>Jitter:</Typography>
                            <Typography variant="body1">{jitter}</Typography>

                            <Typography variant="body1" fontWeight="bold" sx={{ fontSize: 'inherit' }}>Packet Loss:</Typography>
                            <Typography variant="body1">{packetLoss}</Typography>
                        </Box>
                    </Paper>
                    {/* Video container */}
                    <Paper
                        elevation={3}
                        sx={{
                            flex: 1,
                            bgcolor: 'black',
                            aspectRatio: '16/9',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            minWidth: 0,
                            width: '100%',
                            maxWidth: '100%',
                        }}
                    >
                        <video
                            ref={videoRef}
                            controls
                            autoPlay
                            playsInline
                            style={{
                                width: '100%',
                                height: '100%',
                                objectFit: 'contain',
                                background: 'black',
                            }}
                        >
                            Your browser does not support the video tag.
                        </video>
                    </Paper>

                </Box>
            </Container>

        </Box>
    );
};
export default Streaming;