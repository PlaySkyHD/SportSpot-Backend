﻿
using SportSpot.Exceptions.User;
using SportSpot.V1.User.Mapper;
using SportSpot.V1.User.Repositories;

namespace SportSpot.V1.User.Services
{
    public class ClubService(IClubRepositoriy _clubRepositoriy) : IClubService
    {
        public async Task<ClubEntity> CreateClub(Guid id, ClubRegisterRequestDto request)
        {
            ClubEntity entity = new()
            {
                Id = id,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address?.Convert()
            };
            await _clubRepositoriy.CreateClub(entity);
            return entity;
        }

        public async Task DeleteClub(Guid id) => await _clubRepositoriy.DeleteClub(id);

        public Task DeleteClub(ClubEntity club) => DeleteClub(club.Id);

        public async Task<ClubEntity?> GetClubById(Guid id) => await _clubRepositoriy.GetClubById(id) ?? throw new ClubNotFoundException();
    }
}
