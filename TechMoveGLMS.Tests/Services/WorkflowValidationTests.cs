using Microsoft.EntityFrameworkCore;
using System;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Patterns;
using TechMoveGLMS.Services;
using Xunit;

namespace TechMoveGLMS.Tests.Services
{
    public class WorkflowValidationTests
    {
        private ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateRequest_ActiveContract_Succeeds()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new ServiceRequestService(
                context,
                new StandardFreightPricing(),
                new StandardRequestFactory());

            var contract = new Contract
            {
                ContractId = 1,
                ClientId = 1,
                Status = "Active",
                ServiceLevel = "Premium",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(6)
            };
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();

            var request = new ServiceRequest
            {
                ContractId = 1,
                Description = "Test Request",
                Status = "Pending",
                CostZAR = 18.50m
            };

            // Act
            var (success, message) = await service.CreateRequestAsync(request, 100);

            // Assert
            Assert.True(success);
            Assert.Equal("Service Request created successfully.", message);
        }

        [Fact]
        public async Task CreateRequest_ExpiredContract_Fails()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new ServiceRequestService(
                context,
                new StandardFreightPricing(),
                new StandardRequestFactory());

            var contract = new Contract
            {
                ContractId = 1,
                ClientId = 1,
                Status = "Expired",
                ServiceLevel = "Standard",
                StartDate = DateTime.Now.AddMonths(-6),
                EndDate = DateTime.Now.AddMonths(-1)
            };
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();

            var request = new ServiceRequest
            {
                ContractId = 1,
                Description = "Test Request",
                Status = "Pending",
                CostZAR = 18.50m
            };

            // Act
            var (success, message) = await service.CreateRequestAsync(request, 100);

            // Assert
            Assert.False(success);
            Assert.Contains("Expired", message);
        }

        [Fact]
        public async Task CreateRequest_OnHoldContract_Fails()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new ServiceRequestService(
                context,
                new StandardFreightPricing(),
                new StandardRequestFactory());

            var contract = new Contract
            {
                ContractId = 1,
                ClientId = 1,
                Status = "On Hold",
                ServiceLevel = "Standard",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3)
            };
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();

            var request = new ServiceRequest
            {
                ContractId = 1,
                Description = "Test Request",
                Status = "Pending",
                CostZAR = 18.50m
            };

            // Act
            var (success, message) = await service.CreateRequestAsync(request, 100);

            // Assert
            Assert.False(success);
            Assert.Contains("On Hold", message);
        }

        [Fact]
        public async Task CreateRequest_DraftContract_Succeeds()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new ServiceRequestService(
                context,
                new StandardFreightPricing(),
                new StandardRequestFactory());

            var contract = new Contract
            {
                ContractId = 1,
                ClientId = 1,
                Status = "Draft",
                ServiceLevel = "Standard",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3)
            };
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();

            var request = new ServiceRequest
            {
                ContractId = 1,
                Description = "Test Request",
                Status = "Pending",
                CostZAR = 18.50m
            };

            // Act
            var (success, message) = await service.CreateRequestAsync(request, 100);

            // Assert — Draft contracts CAN have service requests
            Assert.True(success);
        }

        [Fact]
        public async Task CreateRequest_ContractNotFound_Fails()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new ServiceRequestService(
                context,
                new StandardFreightPricing(),
                new StandardRequestFactory());

            var request = new ServiceRequest
            {
                ContractId = 999,
                Description = "Test Request",
                Status = "Pending",
                CostZAR = 18.50m
            };

            // Act
            var (success, message) = await service.CreateRequestAsync(request, 100);

            // Assert
            Assert.False(success);
            Assert.Equal("Contract not found.", message);
        }
    }
}