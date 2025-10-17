using Application.Common;
using Application.Common.CacheModels;
using Domain.Repositories;
using Domain.Services;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ActiveDepartmentService : IActiveDepartmentService
    {
        private readonly ICacheService _cacheService;
        private readonly IUserRepository _userRepository;

        public ActiveDepartmentService(ICacheService cacheService, IUserRepository userRepository)
        {
            _cacheService = cacheService;
            _userRepository = userRepository;
        }

        public async Task<Guid?> GetActiveDepartmentIdAsync(Guid userId)
        {
            var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);
            
            // Try to get from cache first
            var cachedData = await _cacheService.GetAsync<ActiveDepartmentCacheModel>(cacheKey);
            if (cachedData != null)
            {
                return cachedData.DepartmentId;
            }

            // If not in cache, get from database
            var user = await _userRepository.GetbyIdAsync(userId);
            if (user?.ActiveDepartmentId != null)
            {
                // Cache the result for future use
                var cacheModel = new ActiveDepartmentCacheModel { DepartmentId = user.ActiveDepartmentId.Value };
                await _cacheService.SetAsync(cacheKey, cacheModel, CacheHelper.Expiration.User);
                return user.ActiveDepartmentId;
            }

            return null;
        }

        public async Task<bool> SetActiveDepartmentIdAsync(Guid userId, Guid departmentId)
        {
            try
            {
                var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);
                
                // Update in database
                var user = await _userRepository.GetbyIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                var result = user.SetActiveDepartment(departmentId);
                if (!result.IsSuccess)
                {
                    return false;
                }

                // Update in cache
                var cacheModel = new ActiveDepartmentCacheModel { DepartmentId = departmentId };
                await _cacheService.SetAsync(cacheKey, cacheModel, CacheHelper.Expiration.User);
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveActiveDepartmentIdAsync(Guid userId)
        {
            try
            {
                var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);
                
                // Remove from cache
                await _cacheService.RemoveAsync(cacheKey);
                
                // Update in database
                var user = await _userRepository.GetbyIdAsync(userId);
                if (user != null)
                {
                    user.ActiveDepartmentId = null;
                    user.Update();
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HasActiveDepartmentAsync(Guid userId)
        {
            var cacheKey = CacheHelper.UserActiveDepartmentKey(userId);
            return await _cacheService.ExistsAsync(cacheKey);
        }
    }
}
