using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvBroadCast.Domain.Entities;

namespace TvBroadCast.Domain.Interfaces.IBroadCast
{
    public interface IBroadCastRepository
    {

        Task<List<BroadCast>> GetAllAsync();
        Task<BroadCast?> GetBroadCastByIdAsync(int id);
        Task AddAsync(BroadCast broadcast);
        Task UpdateAsync(BroadCast broadcast);
        Task DeleteAsync(int id);
        Task<bool> IsOverlappingAsync(DateTime start, DateTime end, int? excludeId = null);
        Task<List<BroadCast>> GetBroadCastForTimeWindowAsync(DateTime windowStart, DateTime windowEnd);

    }
}
