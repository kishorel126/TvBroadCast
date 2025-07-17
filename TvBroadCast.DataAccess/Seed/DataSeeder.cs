using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TvBroadCast.DataAccess.DbContext;
using TvBroadCast.Domain.Entities;

namespace TvBroadCast.DataAccess.Seed
{
    public class DataSeeder
    {

        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        // Dependency injection constructor
        public DataSeeder(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
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
            var schedulerUser = new User { 
                UserName = "scheduler1",
                Email = "scheduler1@tvbroadcast.com"
            };

            var approverUser = new User
            {
                UserName = "approver1",
                Email = "approver1@tvbroadcast.com"
            };

            var adminUser = new User
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
            if (await _userManager.FindByEmailAsync(approverUser.UserName) == null)
            {
                await _userManager.CreateAsync(approverUser, "TBCApprover@123");
                await _userManager.AddToRoleAsync(approverUser, "Approver");
            }
            if (await _userManager.FindByEmailAsync(adminUser.UserName) == null)
            {
                await _userManager.CreateAsync(adminUser, "Admin@123");
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }


            //Seeding the sample broadcasts if it doesn't exist
            if (!_context.BroadCasts.Any())
            {
                var broadcast = new BroadCast
                {
                    Title = "BroadCast : Morning News",
                    Description = "Daily news broadcast in the morning time",
                    StartTime = DateTime.Now.AddHours(8),
                    EndTime = DateTime.Now.AddHours(9),
                    Status = BroadcastStatus.Approved,
                    SchedulerId = schedulerUser.Id,
                    ApproverId = approverUser.Id,
                    ApproverComment = "Scheduled and Approved for broadcast."
                };

                // Adding the broadcast to the context
                _context.BroadCasts.Add(broadcast);

                //Seeding the Approval History if it doesn't exist
                var history = new ApprovalHistory
                {
                    BroadcastId = broadcast.Id,
                    ApproverId = approverUser.Id,
                    Timestamp = DateTime.Now,
                    Status = BroadcastStatus.Approved,
                    Comment = "The morning news from 8 to 9, Looks gooda!"
                };

                // Adding the approval history to the context
                _context.ApprovalHistories.Add(history);


                //Saving all changes to the database
                _context.SaveChanges();
            }

        }
    }
}
