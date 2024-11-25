﻿namespace SportSpot.V1.Session.Chat.Entities
{
    public record MessageEntity
    {
        public required Guid Id { get; set; }
        public required Guid SessionId { get; set; }
        public required Guid CreatorId { get; set; }
        public required string Message { get; set; }
        public required DateTime CreatedAt { get; set; }
        public Guid? ParentMessageId { get; set; }
    }
}
