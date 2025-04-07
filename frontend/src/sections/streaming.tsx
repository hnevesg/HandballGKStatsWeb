import React, { useEffect, useState, useRef } from "react";
//import { RTCPeerConnection, RTCSessionDescription, RTCIceCandidate } from "wrtc";
import {
    Box,
    Container,
    Typography,
    Paper
} from '@mui/material';
import Navbar from '../components/navBar';
import { User } from '../types/user';

const Streaming: React.FC = () => {
    const videoRef = useRef<HTMLVideoElement>(null);
    const signalingServerUrl = 'ws://192.168.43.173:12345/webrtc-signaling';
    let ws: WebSocket;

    const [loggedUser, setLoggedUser] = useState<User | null>(null);
    const baseURL = 'http://192.168.43.173:12345';

    const [bitrate, setBitrate] = useState<string>('');
    const [resolution, setResolution] = useState<string>('');
    const [frameRate, setFrameRate] = useState<string>('');
    const [jitter, setJitter] = useState<string>('');
    const [packetLoss, setPacketLoss] = useState<string>('');
    const [rtt, setRoundTripTime] = useState<string>('');
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

                    /* if ("bitrate" in report) {
                       setBitrate(`${(report.bytesReceived / 1000).toFixed(2)} kbps`);
                     }*/
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
                else if (report.type === "remote-inbound-rtp" && "roundTripTime" in report) {
                    setRoundTripTime(`${report.roundTripTime} s`);
                }
            });
        }, 2000); // updates every 2 seconds
    };

    useEffect(() => {
        const peerConnection = new RTCPeerConnection();

        ws = new WebSocket(signalingServerUrl);
        ws.onopen = () => {
            console.log('WebSocket connection established');
            logStats(peerConnection);
        };

        ws.onmessage = async (message) => {
            const data = JSON.parse(message.data);

            if (data.type === "offer") {
                console.log("RECEIVER - Received offer:" + data.sdp);
                const offer = new RTCSessionDescription({ sdp: data.sdp, type: "offer" });
                await peerConnection.setRemoteDescription(offer);

                const answer = await peerConnection.createAnswer();
                await peerConnection.setLocalDescription(answer);

                ws.send(JSON.stringify({ type: "answer", sdp: answer.sdp }));
            } else if (data.type === "candidate" && data.candidate) {
                console.log("RECEIVER - Received ICE candidate");
                const candidate = new RTCIceCandidate(data.candidate);
                await peerConnection.addIceCandidate(candidate);
            }
        };

        peerConnection.onicecandidate = (event) => {
            if (event.candidate) {
                console.log("RECEIVER - Sending ICE candidate");
                ws.send(JSON.stringify({ type: "candidate", candidate: event.candidate }));
            }
        };

        peerConnection.ontrack = (event) => {
            console.log("RECEIVER - Received remote stream track");
            console.log("videoRef.current exists:", !!videoRef.current);
            console.log("event.streams length:", event.streams.length);

            let streamToUse;

            if (event.streams.length === 0) {
                // new MediaStream with the received track
                streamToUse = new MediaStream([event.track]);
                console.log("Created new MediaStream with track");
            } else {
                streamToUse = event.streams[0];
                console.log("Using existing stream");
            }
            if (videoRef.current) {
                videoRef.current.srcObject = streamToUse;
                videoRef.current.muted = true;
                videoRef.current.play()
                    .catch(error => console.error("Error playing video:", error));
            }
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
                    flexDirection: 'row',
                    gap: 4
                }}>

                    {/* Technical Data container */}
                    <Paper
                        elevation={2}
                        sx={{
                            p: 3,
                            maxWidth: 300,
                            width: '100%',
                            textAlign: 'center',
                            alignSelf: 'flex-start',
                            ml: 0
                        }}
                    >
                        <Typography variant="h6" gutterBottom>
                            Technical Information
                        </Typography>
                        <Box sx={{ display: 'grid', gridTemplateColumns: 'auto 1fr', gap: 2 }}>
                            <Typography variant="body1" fontWeight="bold">Resolution:</Typography>
                            <Typography variant="body1">{resolution}</Typography>

                            <Typography variant="body1" fontWeight="bold">Codec:</Typography>
                            <Typography variant="body1">{codec}</Typography>

                            <Typography variant="body1" fontWeight="bold">Bitrate:</Typography>
                            <Typography variant="body1">{bitrate}</Typography>

                            <Typography variant="body1" fontWeight="bold">Frame Rate:</Typography>
                            <Typography variant="body1">{frameRate}</Typography>

                            <Typography variant="body1" fontWeight="bold">Jitter:</Typography>
                            <Typography variant="body1">{jitter}</Typography>

                            {/* 
                                <Typography variant="body1" fontWeight="bold">Round Trip Time:</Typography>
                                <Typography variant="body1">{rtt}</Typography> 
                            */}

                            <Typography variant="body1" fontWeight="bold">Packet Loss:</Typography>
                            <Typography variant="body1">{packetLoss}</Typography>
                        </Box>
                    </Paper>

                    {/* Video container */}
                    <Paper
                        elevation={3}
                        sx={{
                            width: '100%',
                            bgcolor: 'black',
                            aspectRatio: '16/9',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            flex: 8
                        }}
                    >
                        <video
                            ref={videoRef}
                            controls
                            autoPlay
                            playsInline
                            style={{ width: '100%', height: '100%' }}
                        >
                            Your browser does not support the video tag.
                        </video>
                    </Paper>

                </Box>
            </Container>

        </Box>
    );
};
export default Streaming; //WebRTCReceiver;
