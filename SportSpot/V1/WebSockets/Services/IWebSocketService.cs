﻿using SportSpot.V1.User.Entities;
using SportSpot.V1.WebSockets.Dtos;
using System.Net.WebSockets;

namespace SportSpot.V1.WebSockets.Services
{
    public interface IWebSocketService
    {
        Task<bool> OnConnect(WebSocket webSocket, string authorization);
        Task OnDisconnect(WebSocket webSocket);
        Task OnReceive(WebSocket webSocket, string payload);
        Task<bool> SendMessage(AuthUserEntity user, AbstractWebSocketMessageDto message);
    }
}
