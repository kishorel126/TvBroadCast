
using TvBroadCast.Domain.Entities;
using TvBroadCast.Domain.Interfaces.IBroadCast;

namespace TvBroadCast.Services
{
    public class BroadCastService : IBroadCastService
    {

        private readonly IBroadCastRepository _broadCastRepository;

        //Injecting the dependencies using the constructor
        public BroadCastService(IBroadCastRepository broadCastRepository)
        {
            _broadCastRepository = broadCastRepository;
        }

        public async Task<List<BroadCast>> GetAllAsync()
        {
            return await _broadCastRepository.GetAllAsync();
        }

        public async Task<BroadCast?> GetBroadCastByIdAsync(int id)
        {
            return await _broadCastRepository.GetBroadCastByIdAsync(id);
        }

        public async Task<(bool Success , string Error)> AddBroadCastAsync(BroadCast broadCast)
        {

            //Validating the start and end time --- no overlap
            if(broadCast.StartTime >= broadCast.EndTime)
            {
                return (false , "Start time must be before the end time!");
            }
            if (broadCast.EndTime <= broadCast.StartTime)
            {
                return (false, "End time must be after the start time");
            }

            // Check if overlapping with existing broadcast
            if (await _broadCastRepository.IsOverlappingAsync(broadCast.StartTime , broadCast.EndTime))
            {
                return (false , "Broadcast time overlaps with existing schedule!");
            }

            broadCast.Status = BroadCastStatus.BStatus.Pending;

            await _broadCastRepository.AddAsync(broadCast);

            return (true , "BroadCast added successfully!");

        }


        public async Task<(bool Success , string Error)> UpdateBroadCastAsync(BroadCast broadCast)
        {

            //Validating the start time and end time 
            if(broadCast.StartTime >= broadCast.EndTime)
            {
                return (false , "Start time must be before the end time!");
            }
            if(broadCast.EndTime <= broadCast.StartTime)
            {
                return (false , "End time must be after the start time!");
            }

            //Overlap check for the time
            if(await _broadCastRepository.IsOverlappingAsync(broadCast.StartTime, broadCast.EndTime))
            {
                return (false, "Broadcast time overlaps with existing schedule!");
            }

            //Changing the status
            broadCast.Status = BroadCastStatus.BStatus.Pending;

            await _broadCastRepository.UpdateAsync(broadCast);

            return (true , "Broadcast schedule updated successfully!");

        }

        public async Task<(bool Success , string Error)> DeleteBroadCastAsync(int id)
        {

            var broadcast = _broadCastRepository.GetBroadCastByIdAsync(id);

            if (broadcast != null)
            {
                await _broadCastRepository.DeleteAsync(id);
                return (true , "Broadcast deleted successfully!");
            }

            return (false , "Couldn't delete the Broadcast, please try again!");

        }


        public async Task<List<BroadCast>> GetBroadCastForTimeWindowAsync(DateTime windowStart , DateTime windowEnd)
        {

            //Validating the start time and end time 
            if (windowStart >= windowEnd)
            {
                return null;
            }
            if (windowEnd <= windowStart)
            {
                return null;
            }

            return await _broadCastRepository.GetBroadCastForTimeWindowAsync(windowStart, windowEnd);
        }
    }
}
