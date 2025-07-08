using UnityEngine;
using Unity.WebRTC;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System;
using System.Text;
using System.Threading;
//using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

[Serializable]
public class SDPMessage
{
    public string type;
    public string sdp;
    public string senderId;
    public string targetId;
}

[Serializable]
public class GenericMessage
{
    public string type;
    public string senderId;
}

[Serializable]
public class IceCandidateMessage
{
    public string type;
    public RTCIceCandidateInit candidate;
}


public class video_sender : MonoBehaviour
{
    string currentClientId = "";
    Dictionary<string, RTCPeerConnection> peerConnections = new();

    private VideoStreamTrack sharedVideoStreamTrack;
    public Camera captureCamera;
    private RenderTexture renderTexture;
    private Texture2D frameTexture;

    private ClientWebSocket webSocket;

    private string signalingServerUrl = "wss://gkstatsweb.duckdns.org:12345/webrtc-signaling?role=sender";

    async void Start()
    {
        StartCoroutine(WebRTC.Update());
        await ConnectToSignalingServer();
        GetVideoSource().ConfigureAwait(false);
    }

    async Task ConnectToSignalingServer()
    {
        webSocket = new ClientWebSocket();
        await webSocket.ConnectAsync(new Uri(signalingServerUrl), CancellationToken.None);
        Debug.Log("Connected to signaling server");

        // comenzar a escuchar mensajes del servidor de señalización
        _ = Task.Run(async () =>
        {
            byte[] buffer = new byte[8192];
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Debug.Log("SENDER - Received message");

                try
                {
                    var genericMsg = JsonUtility.FromJson<GenericMessage>(message);

                    if( genericMsg.type == "hello"){
                        string clientId = genericMsg.senderId;
                        Debug.Log($"New client connected: {clientId}");
                        await StartWebRTCConnection(clientId);
                    }
                    else if (genericMsg.type == "answer")
                    {
                        var data = JsonUtility.FromJson<SDPMessage>(message);
                  
                        if (!peerConnections.ContainsKey(data.senderId))
                        {
                            Debug.LogError($"Answer from unknonwn client {data.senderId}");
                            return;
                        }

                        if (data.targetId != "1")
                        {
                            Debug.LogError($"Answer from client {data.senderId} is not for this sender. Expected targetId '1', got '{data.targetId}'");
                            return;
                        }

                        if (!data.sdp.EndsWith("\r\n"))
                        {
                            data.sdp += "\r\n";
                        }
                        Debug.Log("SENDER - Received answer from client: " + data.senderId);
                        var answer = new RTCSessionDescription
                        {
                            type = RTCSdpType.Answer,
                            sdp = data.sdp
                        };

                        if (peerConnections[data.senderId].SignalingState == RTCSignalingState.Stable){
                            Debug.LogError($"Peer connection for client {data.senderId} is already stable, not setting remote description.");
                            return;
                        }

                        var operation = peerConnections[data.senderId].SetRemoteDescription(ref answer);
                        while (!operation.IsDone)
                            await Task.Yield();

                        if (operation.IsError)
                            Debug.LogError($"Failed to set remote description: {operation.Error.message}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error processing message: {e.Message}");
                }
            }
        });
    }

    async Task StartWebRTCConnection(string clientId)
    {
        if (peerConnections.ContainsKey(clientId))
        {
            Debug.Log($"Peer connection for client {clientId} already exists. Recreating...");
        peerConnections[clientId].Close();
            peerConnections[clientId].Dispose();
            peerConnections.Remove(clientId);
        }

        Debug.Log($"Starting WebRTC connection for client: {clientId}");

        RTCConfiguration config = new RTCConfiguration
        {
            iceServers = new RTCIceServer[] { }
        };

        RTCPeerConnection peerConnection = new RTCPeerConnection(ref config);
        peerConnections[clientId] = peerConnection;

        // manejador de eventos para candidatos ICE
        peerConnection.OnIceCandidate = candidate =>
        {
            if (candidate != null)
            {
                var candidateMessage = JsonUtility.ToJson(new IceCandidateMessage
                {
                    type = "candidate",
                    candidate = new RTCIceCandidateInit
                    {
                        candidate = candidate.Candidate,
                        sdpMid = candidate.SdpMid,
                        sdpMLineIndex = candidate.SdpMLineIndex
                    }
                });
                Debug.Log($"SERIALIZED ICE Candidate: " + candidateMessage.ToString());
            }
        };

        peerConnection.OnIceConnectionChange = state =>
        {
            Debug.Log($"SENDER - Connection state: {state.ToString()}");
        };

        if (sharedVideoStreamTrack != null)
        {
            Debug.Log("Adding shared video stream track to peer connection");
            peerConnection.AddTrack(sharedVideoStreamTrack);
        }

        peerConnection.OnIceGatheringStateChange = state =>
        {
            Debug.Log($"ICE gathering state changed to: {state}");

            if (state == RTCIceGatheringState.Complete)
            {
                // una vez se han reunido todos los candidatos, enviar la oferta
                string offerMessage = JsonUtility.ToJson(new SDPMessage
                {
                    type = "offer",
                    sdp = peerConnection.LocalDescription.sdp,
                    senderId = "1",
                    targetId = clientId
                });

                Debug.Log("Sending complete offer with all ICE candidates included");
                SendWebSocketMessage(offerMessage).ConfigureAwait(false);
            }
        };

        var createOfferOperation = peerConnection.CreateOffer();
        while (!createOfferOperation.IsDone)
            await Task.Yield();

        if (createOfferOperation.IsError)
        {
            Debug.LogError($"Failed to create offer: {createOfferOperation.Error.message}");
            return;
        }

        var offer = createOfferOperation.Desc;
        var setLocalDescriptionOperation = peerConnection.SetLocalDescription(ref offer);

        while (!setLocalDescriptionOperation.IsDone)
            await Task.Yield();

        if (setLocalDescriptionOperation.IsError)
        {
            Debug.LogError($"Failed to set local description: {setLocalDescriptionOperation.Error.message}");
            return;
        }

        string offerMessage = JsonUtility.ToJson(new SDPMessage
        {
            type = "offer",
            sdp = offer.sdp,
            senderId = "1",
            targetId = clientId
        });
    }

    async Task<VideoStreamTrack> GetVideoSource()
    {
        await Task.Yield();

        if (!renderTexture)
        {
            renderTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
            captureCamera.targetTexture = renderTexture;
        }

        // Texture2D para leer los pixels
        frameTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);

        // capturar frames
        sharedVideoStreamTrack = new VideoStreamTrack(renderTexture);
        StartCoroutine(CaptureFrames());

        return sharedVideoStreamTrack;
    }

    private System.Collections.IEnumerator CaptureFrames()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (Time.frameCount % 50 == 0)
            {
                Debug.Log("SENDER - Capturing frame");
            }
            RenderTexture.active = renderTexture;
            frameTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            frameTexture.Apply();
            RenderTexture.active = null;
        }
    }

    async Task SendWebSocketMessage(string message)
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        else
        {
            Debug.LogError("WebSocket is not connected. Unable to send message.");
        }
    }

    void OnDestroy()
    {
        if (peerConnections != null)
        {
            foreach (var connnection in peerConnections)
            {
                var peerConnection = connnection.Value;
                if (peerConnection != null)
                {
                    peerConnection.Close();
                    peerConnection.Dispose();
                    Debug.Log($"Closed and disposed peer connection for client {connnection.Key}");
                }
            }
        }

        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Application closing", CancellationToken.None).Wait();
        }
    }

    async void Update()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            await Task.Delay(1000);
        }
    }
}
