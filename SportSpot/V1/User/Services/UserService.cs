﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportSpot.V1.Exceptions.User;
using SportSpot.V1.Media.Entities;
using SportSpot.V1.Media.Services;
using SportSpot.V1.User.Context;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.User.Services
{
    public class UserService(UserManager<AuthUserEntity> _userManager, AuthContext _context, IMediaService _mediaService) : IUserService
    {
        public async Task DeleteAllUser()
        {
            List<AuthUserEntity> users = await _context.Users.ToListAsync();
            foreach (AuthUserEntity user in users)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task<byte[]> GetAvatar(AuthUserEntity user)
        {
            if (user.AvatarId == null)
                return []; //TODO: return default avatar
            MediaEntity media = await _mediaService.GetMedia(user.AvatarId.Value);
            return await _mediaService.GetMediaAsBytes(media);
        }
        public async Task<AuthUserEntity> GetUser(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException();
        }
    }
}
