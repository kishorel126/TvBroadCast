using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using TvBroadCast.DataAccess.DbContext;
using TvBroadCast.Domain.Entities;

namespace TvBroadCast.DataAccess.Seed
{
    public class DataSeeder
    {

        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        // Dependency injection constructor
        public DataSeeder(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task SeedDataAsync()
        {

            //Ensure the database is created
            await _context.Database.MigrateAsync();

            //Seeding Roles
            string[] roles = {"Scheduler" , "Approver" , "Admin" };

            foreach (var role in roles)
            {
                // Check if the role exists
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    // If it doesn't exist, create the role
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            //Seeding Test Users
            var schedulerUser = new IdentityUser { 
                UserName = "scheduler1",
                Email = "scheduler1@tvbroadcast.com"
            };

            var approverUser = new IdentityUser
            {
                UserName = "approver1",
                Email = "approver1@tvbroadcast.com"
            };

            var adminUser = new IdentityUser
            {
                UserName = "admin1",
                Email = "admin1@tvbroadcast.com"
            };


            //CHeck if the test users already exist or else creating them 
            if (await _userManager.FindByEmailAsync(schedulerUser.Email) == null)
            {
                await _userManager.CreateAsync(schedulerUser, "TBCScheduler@123");
                await _userManager.AddToRoleAsync(schedulerUser, "Scheduler");
            }
            if (await _userManager.FindByEmailAsync(approverUser.Email) == null)
            {
                await _userManager.CreateAsync(approverUser, "TBCApprover@123");
                await _userManager.AddToRoleAsync(approverUser, "Approver");
            }
            if (await _userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                await _userManager.CreateAsync(adminUser, "Admin@123");
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }


            //Seeding the sample broadcasts if it doesn't exist
            if (!_context.BroadCasts.Any())
            {
                var utcNow = DateTime.UtcNow;

                var broadcasts = new List<BroadCast>
{
    // Within current slot
    new BroadCast
    {
        Title = "Headline Update",
        Description = "Midday news featuring global updates.",
        StartTime = utcNow.AddMinutes(-15),
        EndTime = utcNow.AddMinutes(15),
        Status = BroadCastStatus.BStatus.Pending,
        SchedulerId = schedulerUser.Id,
        ApproverId = approverUser.Id,
        ApproverComment = ""
    },

    // Fully before window
    new BroadCast
    {
        Title = "Early Morning Recap",
        Description = "Highlights from 6AM - 7AM.",
        StartTime = utcNow.AddHours(-6),
        EndTime = utcNow.AddHours(-5),
        Status = BroadCastStatus.BStatus.Approved,
        SchedulerId = schedulerUser.Id,
        ApproverId = approverUser.Id,
        ApproverComment = "Approved"
    },

    // 2 hours before current slot
    new BroadCast
    {
        Title = "Pre-Prime Analysis",
        Description = "Show discusses trends before major prime time.",
        StartTime = utcNow.AddHours(-2),
        EndTime = utcNow.AddHours(-1.5),
        Status = BroadCastStatus.BStatus.Pending,
        SchedulerId = schedulerUser.Id,
        ApproverId = approverUser.Id,
        ApproverComment = ""
    },

    // 2 hours after current slot
    new BroadCast
    {
        Title = "Late Bulletin",
        Description = "Wrap-up of the day’s top stories.",
        StartTime = utcNow.AddHours(2),
        EndTime = utcNow.AddHours(2.5),
        Status = BroadCastStatus.BStatus.Approved,
        SchedulerId = schedulerUser.Id,
        ApproverId = approverUser.Id,
        ApproverComment = "Scheduled"
    },

    // Starts inside, ends after
    new BroadCast
    {
        Title = "Live Interview Segment",
        Description = "Conversation with industry leaders.",
        StartTime = utcNow.AddMinutes(20),
        EndTime = utcNow.AddMinutes(60),
        Status = BroadCastStatus.BStatus.Pending,
        SchedulerId = schedulerUser.Id,
        ApproverId = approverUser.Id,
        ApproverComment = ""
    },

    // Starts before, ends inside
    new BroadCast
    {
        Title = "Trend Watch",
        Description = "Market movements and upcoming tech.",
        StartTime = utcNow.AddMinutes(-45),
        EndTime = utcNow.AddMinutes(-5),
        Status = BroadCastStatus.BStatus.Approved,
        SchedulerId = schedulerUser.Id,
        ApproverId = approverUser.Id,
        ApproverComment = "Good timing"
    },

    // Fully overlapping 4.5-hour window
    new BroadCast
    {
        Title = "Extended Feature",
        Description = "In-depth documentary on climate change.",
        StartTime = utcNow.AddHours(-2),
        EndTime = utcNow.AddHours(2),
        Status = BroadCastStatus.BStatus.Rejected,
        SchedulerId = schedulerUser.Id,
        ApproverId = approverUser.Id,
        ApproverComment = "Too long for slot"
    },

    // Exactly at window edges
    new BroadCast
    {
        Title = "Slot Edge Start",
        Description = "Starts at exact -2 hour mark.",
        StartTime = utcNow.AddHours(-2),
        EndTime = utcNow.AddHours(-1.5),
        Status = BroadCastStatus.BStatus.Approved,
        SchedulerId = schedulerUser.Id,
        ApproverId = approverUser.Id,
        ApproverComment = "Fits well"
    },
    new BroadCast
    {
        Title = "Slot Edge End",
        Description = "Ends at exact +2 hour mark.",
        StartTime = utcNow.AddHours(1.5),
        EndTime = utcNow.AddHours(2),
        Status = BroadCastStatus.BStatus.Approved,
        SchedulerId = schedulerUser.Id,
        ApproverId = approverUser.Id,
        ApproverComment = "Sharp timing"
    },

    // Outside future window
    new BroadCast
    {
        Title = "Future Forecast",
        Description = "Tomorrow’s predictions.",
        StartTime = utcNow.AddHours(6),
        EndTime = utcNow.AddHours(7),
        Status = BroadCastStatus.BStatus.Pending,
        SchedulerId = schedulerUser.Id,
        ApproverId = approverUser.Id,
        ApproverComment = ""
    }
};


                //// Adding the broadcast to the context
                //await _context.BroadCasts.AddRangeAsync(broadcasts);
                //await _context.SaveChangesAsync();

                foreach (var b in broadcasts)
                {
                    try
                    {
                        _context.BroadCasts.Add(b);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to seed broadcast '{b.Title}': {ex.Message}");
                    }
                }



                //Seeding the Approval History if it doesn't exist
                var histories = broadcasts.Select(b => new ApprovalHistory
                {
                    BroadcastId = b.Id,
                    ApproverId = approverUser.Id,
                    Timestamp = DateTime.Now,
                    Status = BroadCastStatus.BStatus.Approved,
                    Comment = $"Approved: {b.Title}"
                }).ToList();

                // Adding the approval history to the context
                await _context.ApprovalHistories.AddRangeAsync(histories);


                //Saving all changes to the database
                await _context.SaveChangesAsync();
            }

        }
    }
}
