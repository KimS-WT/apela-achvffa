using System;
using System.Linq;
using System.Threading.Tasks;
using ChoctawGivingCircle.Data;
using ChoctawGivingCircle.Models;
using ChoctawGivingCircle.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ChoctawGivingCircle.Tests
{
    public class AllocationFlowTests
    {
        private ApplicationDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task ApprovingAllocation_AppliesAllocationAndCreatesEmailLogs()
        {
            using var db = CreateContext(Guid.NewGuid().ToString());

            // seed users
            var adminInitiator = new ApplicationUser { Id = "admin-init", UserName = "init@local", Email = "init@local", DisplayName = "Initiator" };
            var adminApprover = new ApplicationUser { Id = "admin-approver", UserName = "approver@local", Email = "approver@local", DisplayName = "Approver" };
            var requester = new ApplicationUser { Id = "requester", UserName = "requester@local", Email = "requester@local", DisplayName = "Requester" };
            await db.Users.AddRangeAsync(adminInitiator, adminApprover, requester);

            // seed a request owned by the requester
            var request = new AssistanceRequest
            {
                Title = "Test Request",
                Description = "Help needed",
                Category = AssistanceCategory.Other,
                Priority = AssistancePriority.Medium,
                EstimatedCost = 200m,
                Status = AssistanceStatus.Approved,
                UserId = requester.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await db.AssistanceRequests.AddAsync(request);

            // seed a general fund contribution
            var contribution = new Contribution
            {
                ContributionType = "Money",
                Amount = 150m,
                DonorUserId = adminInitiator.Id,
                IsGeneralFund = true,
                CreatedAt = DateTime.UtcNow
            };
            await db.Contributions.AddAsync(contribution);

            await db.SaveChangesAsync();

            // create pending allocation approval
            var approval = new FundAllocationApproval
            {
                ContributionId = contribution.Id,
                AssistanceRequestId = request.Id,
                RequestedByAdminId = adminInitiator.Id,
                Status = AllocationApprovalStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            await db.FundAllocationApprovals.AddAsync(approval);
            await db.SaveChangesAsync();

            // instantiate services
            var contributionService = new ContributionService(db);
            var emailService = new FakeEmailService(db, NullLogger<FakeEmailService>.Instance);

            // simulate approver applying the allocation (what the Approvals page does)
            await contributionService.AllocateToRequestAsync(approval.ContributionId, approval.AssistanceRequestId);

            approval.Status = AllocationApprovalStatus.Approved;
            approval.ApprovedByAdminId = adminApprover.Id;
            approval.ApprovedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            // send notification emails
            await emailService.SendAllocationApprovedAsync(requester.Email, approval);
            await emailService.SendAllocationApprovedAsync(adminInitiator.Email, approval);

            // assertions: contribution was allocated
            var updatedContribution = await db.Contributions.FindAsync(contribution.Id);
            Assert.Equal(request.Id, updatedContribution.AssistanceRequestId);

            // email logs created
            var emailLogs = await db.EmailLogs.ToListAsync();
            Assert.True(emailLogs.Count >= 2, "Expected at least two email logs (requester + initiator)");
            Assert.Contains(emailLogs, e => e.EmailType == "AllocationApproved" && e.RecipientEmail == requester.Email);
            Assert.Contains(emailLogs, e => e.EmailType == "AllocationApproved" && e.RecipientEmail == adminInitiator.Email);
        }

        [Fact]
        public async Task DenyingAllocation_CreatesDeniedEmailLog()
        {
            using var db = CreateContext(Guid.NewGuid().ToString());

            var adminInitiator = new ApplicationUser { Id = "admin-init-2", UserName = "init2@local", Email = "init2@local", DisplayName = "Initiator2" };
            await db.Users.AddAsync(adminInitiator);

            var request = new AssistanceRequest
            {
                Title = "Test Request 2",
                Description = "Help needed",
                Category = AssistanceCategory.Other,
                Priority = AssistancePriority.Low,
                EstimatedCost = 50m,
                Status = AssistanceStatus.Approved,
                UserId = adminInitiator.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await db.AssistanceRequests.AddAsync(request);

            var contribution = new Contribution
            {
                ContributionType = "Money",
                Amount = 25m,
                DonorUserId = adminInitiator.Id,
                IsGeneralFund = true,
                CreatedAt = DateTime.UtcNow
            };
            await db.Contributions.AddAsync(contribution);
            await db.SaveChangesAsync();

            var approval = new FundAllocationApproval
            {
                ContributionId = contribution.Id,
                AssistanceRequestId = request.Id,
                RequestedByAdminId = adminInitiator.Id,
                Status = AllocationApprovalStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            await db.FundAllocationApprovals.AddAsync(approval);
            await db.SaveChangesAsync();

            var emailService = new FakeEmailService(db, NullLogger<FakeEmailService>.Instance);

            // Simulate denial
            approval.Status = AllocationApprovalStatus.Denied;
            approval.ApprovedByAdminId = "some-approver";
            approval.ApprovedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            await emailService.SendAllocationDeniedAsync(adminInitiator.Email, approval);

            var emailLogs = await db.EmailLogs.ToListAsync();
            Assert.Contains(emailLogs, e => e.EmailType == "AllocationDenied" && e.RecipientEmail == adminInitiator.Email);
        }
    }
}
