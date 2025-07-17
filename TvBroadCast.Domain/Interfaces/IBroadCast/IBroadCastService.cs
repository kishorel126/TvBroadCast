using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvBroadCast.Domain.Entities;

namespace TvBroadCast.Domain.Interfaces.IBroadCast
{
    public interface IBroadCastService
    {

        Task<List<BroadCast>> GetAllAsync();

        Task<BroadCast?> GetBroadCastByIdAsync(int id);

        Task<(bool Success, string Error)> AddBroadCastAsync(BroadCast broadCast);

        Task<(bool Success, string Error)> UpdateBroadCastAsync(BroadCast broadCast);

        Task<(bool Success, string Error)> DeleteBroadCastAsync(int id);

        Task<List<BroadCast>> GetBroadCastForTimeWindowAsync(DateTime windowStart, DateTime windowEnd);


    }
}
