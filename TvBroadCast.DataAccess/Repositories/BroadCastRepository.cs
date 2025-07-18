using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvBroadCast.DataAccess.DbContext;
using TvBroadCast.Domain.Entities;
using TvBroadCast.Domain.Interfaces.IBroadCast;

namespace TvBroadCast.DataAccess.Repositories
{
    public class BroadCastRepository : IBroadCastRepository
    {

        private readonly AppDbContext _context;

        public BroadCastRepository(AppDbContext context) { 
            _context = context;
        }

        public async Task<List<BroadCast>> GetAllAsync()
        {
            return  _context.BroadCasts.OrderBy(b => b.StartTime).ToList();
        }

        public async Task<BroadCast?> GetBroadCastByIdAsync(int id)
        {
            return await _context.BroadCasts.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(BroadCast broadCast)
        {
            _context.BroadCasts.Add(broadCast);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BroadCast broadCast)
        {
            _context.BroadCasts.Update(broadCast);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var broadcast = await _context.BroadCasts.FindAsync(id);
            if (broadcast != null) { 
                _context.BroadCasts.Remove(broadcast);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsOverlappingAsync(DateTime start , DateTime end , int? excludeId = null)
        {

            return await _context.BroadCasts.AnyAsync(b =>
                b.StartTime < end && b.EndTime > start && (excludeId == null || b.Id != excludeId)
            );

        }

        public async Task <List<BroadCast>> GetBroadCastForTimeWindowAsync(DateTime windowStart , DateTime windowEnd)
        {
            

            return   _context.BroadCasts
                .Where(b => (b.StartTime < windowEnd) && (b.EndTime > windowStart) && (b.Status == BroadCastStatus.BStatus.Approved))
                .OrderBy(b => b.StartTime)
                .ToList();
        }

    }
}
