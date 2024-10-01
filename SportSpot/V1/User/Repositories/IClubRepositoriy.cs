﻿namespace SportSpot.V1.User
{
    public interface IClubRepositoriy
    {
        Task<ClubEntity?> GetClubById(Guid id);
        Task<ClubEntity> CreateClub(ClubEntity club);
        Task<ClubEntity> UpdateClub(ClubEntity club);
        Task DeleteClub(Guid id);
    }
}