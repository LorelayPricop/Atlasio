using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using backend.Presentation.Controllers;
using backend.Application.DTOs;
using backend.Domain.Entities;
using backend.Application.Commands;
using backend.Application.Queries;
using MediatR;
using backend.Tests.Helpers;
using Microsoft.Extensions.Logging;

namespace backend.Tests.Controllers
{
    /// <summary>
    /// Tests unitarios para DestinationsController
    /// Prueba todos los endpoints del controlador de destinos
    /// </summary>
    public class DestinationsControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<DestinationsController>> _mockLogger;
        private readonly DestinationsController _controller;

        public DestinationsControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<DestinationsController>>();
            _controller = new DestinationsController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetDestinations_WithValidFilter_ReturnsOkResult()
        {
            // Arrange
            var filter = TestDataHelper.CreateTestDestinationFilter();
            var expectedResult = new PagedResultDto<DestinationDto>
            {
                Items = TestDataHelper.CreateTestDestinations().Select(d => new DestinationDto
                {
                    ID = d.ID,
                    Name = d.Name,
                    Description = d.Description,
                    CountryCode = d.CountryCode,
                    DestinationTypeId = d.DestinationTypeId,
                    LastModif = d.LastModif,
                    CreatedDate = d.CreatedDate
                }).ToList(),
                TotalCount = 3,
                Page = 1,
                PageSize = 10
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetDestinationsQuery>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetDestinations(filter);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetDestinations_WhenServiceThrowsException_PropagatesException()
        {
            // Arrange
            var filter = TestDataHelper.CreateTestDestinationFilter();
            _mockMediator.Setup(m => m.Send(It.IsAny<GetDestinationsQuery>(), It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new Exception("Error de base de datos"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.GetDestinations(filter));
            exception.Message.Should().Be("Error de base de datos");
        }

        [Fact]
        public async Task GetDestination_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var destinationId = 1;
            var expectedDestination = new DestinationDto
            {
                ID = destinationId,
                Name = "Cancún",
                Description = "Hermosa playa en el Caribe mexicano",
                CountryCode = "MEX",
                DestinationTypeId = TestDataHelper.BeachTypeId,
                LastModif = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetDestinationByIdQuery>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedDestination);

            // Act
            var result = await _controller.GetDestination(destinationId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedDestination);
        }

        [Fact]
        public async Task GetDestination_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var destinationId = TestConstants.NonExistentDestinationId;
            _mockMediator.Setup(m => m.Send(It.IsAny<GetDestinationByIdQuery>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync((DestinationDto?)null);

            // Act
            var result = await _controller.GetDestination(destinationId);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult!.Value.Should().NotBeNull();
        }



        [Fact]
        public async Task CreateDestination_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var createDto = TestDataHelper.CreateTestCreateDestinationDto();
            var createdDestination = new DestinationDto
            {
                ID = 1,
                Name = createDto.Name,
                Description = createDto.Description,
                CountryCode = createDto.CountryCode,
                DestinationTypeId = createDto.DestinationTypeId,
                LastModif = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateDestinationCommand>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(createdDestination);

            // Act
            var result = await _controller.CreateDestination(createDto);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult!.Value.Should().BeEquivalentTo(createdDestination);
            createdResult.ActionName.Should().Be(nameof(_controller.GetDestination));
        }

        [Fact]
        public async Task CreateDestination_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var createDto = TestDataHelper.CreateTestCreateDestinationDto();
            _controller.ModelState.AddModelError("Name", "El nombre es requerido");

            // Act
            var result = await _controller.CreateDestination(createDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }



        [Fact]
        public async Task UpdateDestination_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var destinationId = 1;
            var updateDto = TestDataHelper.CreateTestUpdateDestinationDto();
            var updatedDestination = new DestinationDto
            {
                ID = destinationId,
                Name = updateDto.Name,
                Description = updateDto.Description,
                CountryCode = updateDto.CountryCode,
                DestinationTypeId = updateDto.DestinationTypeId,
                LastModif = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateDestinationCommand>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(updatedDestination);

            // Act
            var result = await _controller.UpdateDestination(destinationId, updateDto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(updatedDestination);
        }

        [Fact]
        public async Task UpdateDestination_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var destinationId = TestConstants.NonExistentDestinationId;
            var updateDto = TestDataHelper.CreateTestUpdateDestinationDto();
            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateDestinationCommand>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync((DestinationDto?)null);

            // Act
            var result = await _controller.UpdateDestination(destinationId, updateDto);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateDestination_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var destinationId = 1;
            var updateDto = TestDataHelper.CreateTestUpdateDestinationDto();
            _controller.ModelState.AddModelError("Name", "El nombre es requerido");

            // Act
            var result = await _controller.UpdateDestination(destinationId, updateDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }



        [Fact]
        public async Task DeleteDestination_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var destinationId = 1;
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteDestinationCommand>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteDestination(destinationId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteDestination_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var destinationId = TestConstants.NonExistentDestinationId;
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteDestinationCommand>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteDestination(destinationId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }



        [Fact]
        public async Task GetCountries_ReturnsOkResult()
        {
            // Arrange
            var expectedCountries = new List<string> { "FRA", "JPN", "MEX" };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCountriesQuery>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedCountries);

            // Act
            var result = await _controller.GetCountries();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedCountries);
        }



        [Fact]
        public async Task GetDestinationTypes_ReturnsOkResult()
        {
            // Arrange
            var expectedTypes = new List<DestinationTypeDto> 
            { 
                new() { Id = 1, Code = "BEACH", Name = "Beach", DisplayOrder = 1, IsActive = true },
                new() { Id = 2, Code = "MOUNTAIN", Name = "Mountain", DisplayOrder = 2, IsActive = true },
                new() { Id = 3, Code = "CITY", Name = "City", DisplayOrder = 3, IsActive = true },
                new() { Id = 4, Code = "CULTURAL", Name = "Cultural", DisplayOrder = 4, IsActive = true },
                new() { Id = 5, Code = "ADVENTURE", Name = "Adventure", DisplayOrder = 5, IsActive = true },
                new() { Id = 6, Code = "RELAX", Name = "Relax", DisplayOrder = 6, IsActive = true }
            };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetDestinationTypesQuery>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedTypes);

            // Act
            var result = await _controller.GetDestinationTypes();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var types = okResult!.Value as List<DestinationTypeDto>;
            types.Should().NotBeNull();
            types.Should().HaveCount(6);
            types.Should().Contain(t => t.Code == "BEACH");
            types.Should().Contain(t => t.Code == "MOUNTAIN");
            types.Should().Contain(t => t.Code == "CITY");
            types.Should().Contain(t => t.Code == "CULTURAL");
            types.Should().Contain(t => t.Code == "ADVENTURE");
            types.Should().Contain(t => t.Code == "RELAX");
        }

        [Fact]
        public async Task GetDestinationTypes_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<GetDestinationTypesQuery>(), It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new Exception("Error de base de datos"));
            
            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.GetDestinationTypes());
            exception.Message.Should().Be("Error de base de datos");
        }
    }
}
