using AutoMapper;
using Bakery.Dtos;
using Bakery.Endpoints;
using Bakery.Models;
using Bakery.Repositories.Contracts;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Bakery.Tests.Endpoints;

public class OptionsEndpointsTests
{
    private readonly Mock<IOptionRepository> _mockOptionRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateOptionDto>> _mockCreateOptionDtoValidator;
    private readonly Mock<IValidator<UpdateOptionDto>> _mockUpdateOptionDtoValidator;

    public OptionsEndpointsTests()
    {
        _mockOptionRepository = new Mock<IOptionRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateOptionDtoValidator = new Mock<IValidator<CreateOptionDto>>();
        _mockUpdateOptionDtoValidator = new Mock<IValidator<UpdateOptionDto>>();
    }

    [Fact]
    public async Task OptionsEndpoints_GetOptionsAsync_ReturnsOkResult()
    {
        // Arrange
        var options = new List<Option> { new() { Id = 1, OptionName = "Dozen", Coefficient = 12.0 } };
        var optionsDtos = new List<OptionDetailsDto> { new(1, "Dozen", 12.0) };

        _mockOptionRepository.Setup(repo => repo.GetOptionsAsync()).ReturnsAsync(options);
        _mockMapper.Setup(m => m.Map<List<OptionDetailsDto>>(options)).Returns(optionsDtos);

        // Act
        var result = await OptionsEndpoints.GetOptionsAsync(_mockOptionRepository.Object, _mockMapper.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Ok<List<OptionDetailsDto>>>();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(optionsDtos);

        _mockOptionRepository.Verify(repo => repo.GetOptionsAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<List<OptionDetailsDto>>(options), Times.Once);
    }

    [Fact]
    public async Task OptionsEndpoints_GetOptionByIdAsync_ReturnsOkResult()
    {
        // Arrange
        var expectedId = 1;
        var option = new Option { Id = expectedId, OptionName = "Dozen", Coefficient = 12.0 };
        var optionDetailsDto = new OptionDetailsDto(expectedId, "Dozen", 12.0);

        _mockOptionRepository.Setup(repo =>
            repo.GetOptionByIdAsync(It.Is<int>(id => id == option.Id))).ReturnsAsync(option);
        _mockMapper.Setup(m => m.Map<OptionDetailsDto>(option)).Returns(optionDetailsDto);

        // Act
        var result = await OptionsEndpoints.GetOptionByIdAsync(expectedId, _mockOptionRepository.Object, _mockMapper.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<Ok<OptionDetailsDto>, NotFound<string>>>();

        var okResult = (Ok<OptionDetailsDto>)result.Result;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(optionDetailsDto);

        _mockOptionRepository.Verify(repo => repo.GetOptionByIdAsync(expectedId), Times.Once);
        _mockMapper.Verify(m => m.Map<OptionDetailsDto>(option), Times.Once);
    }

    [Fact]
    public async Task OptionsEndpoints_GetOptionByIdAsync_ReturnsNotFoundResult()
    {
        // Arrange
        var existingId = 1;
        var nonExistingOptionId = 10;
        var option = new Option { Id = existingId, OptionName = "Dozen", Coefficient = 12.0 };
        var optionDetailsDto = new OptionDetailsDto(existingId, "Dozen", 12.0);

        _mockOptionRepository.Setup(repo =>
            repo.GetOptionByIdAsync(It.Is<int>(id => id == option.Id))).ReturnsAsync(option);
        _mockMapper.Setup(m => m.Map<OptionDetailsDto>(option)).Returns(optionDetailsDto);

        // Act
        var result = await OptionsEndpoints.GetOptionByIdAsync(nonExistingOptionId, _mockOptionRepository.Object, _mockMapper.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<Ok<OptionDetailsDto>, NotFound<string>>>();

        var notFoundResult = (NotFound<string>)result.Result;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be(EndpointsConstants.Option.NOT_FOUND_MESSAGE);

        _mockOptionRepository.Verify(repo => repo.GetOptionByIdAsync(nonExistingOptionId), Times.Once);
        _mockMapper.Verify(m => m.Map<OptionDetailsDto>(It.IsAny<Option>()), Times.Never);
    }

    [Fact]
    public async Task OptionsEndpoints_CreateOptionAsync_ReturnOkResult()
    {
        // Arrange
        var newOption = new CreateOptionDto("Dozen", 12.0);
        var createdOption = new Option { Id = 1, OptionName = "Dozen", Coefficient = 12.0 };
        var createdOptionDetailsDto = new OptionDetailsDto(1, "Dozen", 12.0);

        _mockOptionRepository.Setup(repo =>
            repo.CreateOptionAsync(newOption)).ReturnsAsync(createdOption);
        _mockMapper.Setup(m => m.Map<OptionDetailsDto>(createdOption)).Returns(createdOptionDetailsDto);
        _mockCreateOptionDtoValidator.Setup(v => v.ValidateAsync(newOption, default))
                .ReturnsAsync(new ValidationResult());

        // Act
        var result = await OptionsEndpoints.CreateOptionAsync(
            newOption, _mockOptionRepository.Object, _mockMapper.Object, _mockCreateOptionDtoValidator.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<Created<OptionDetailsDto>, BadRequest<string>>>();

        var createdResult = (Created<OptionDetailsDto>)result.Result;
        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.Value.Should().BeEquivalentTo(createdOptionDetailsDto);

        _mockOptionRepository.Verify(repo => repo.CreateOptionAsync(newOption), Times.Once);
        _mockMapper.Verify(m => m.Map<OptionDetailsDto>(createdOption), Times.Once);
    }

    [Fact]
    public async Task OptionsEndpoints_UpdateOptionAsync_ReturnsNoContentResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = true;
        var updatedOption = new UpdateOptionDto("Dozen", 12.0);

        _mockOptionRepository.Setup(repo =>
            repo.UpdateOptionAsync(It.Is<int>(id => id == expectedId), updatedOption)).ReturnsAsync(expectedResult);
        _mockUpdateOptionDtoValidator.Setup(v => v.ValidateAsync(updatedOption, default))
               .ReturnsAsync(new ValidationResult());


        // Act
        var result = await OptionsEndpoints.UpdateOptionAsync(
            expectedId, updatedOption, _mockOptionRepository.Object, _mockUpdateOptionDtoValidator.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>, BadRequest<string>>>();

        var noContentResult = (NoContent)result.Result;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _mockOptionRepository.Verify(repo => repo.UpdateOptionAsync(expectedId, updatedOption), Times.Once);
    }

    [Fact]
    public async Task OptionsEndpoints_UpdateOptionAsync_ReturnsNotFoundResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = false;
        var updatedOption = new UpdateOptionDto("Dozen", 12.0);

        _mockOptionRepository.Setup(repo =>
            repo.UpdateOptionAsync(It.Is<int>(id => id == expectedId), updatedOption)).ReturnsAsync(expectedResult);
        _mockUpdateOptionDtoValidator.Setup(v => v.ValidateAsync(updatedOption, default))
               .ReturnsAsync(new ValidationResult());

        // Act
        var result = await OptionsEndpoints.UpdateOptionAsync(
            expectedId, updatedOption, _mockOptionRepository.Object, _mockUpdateOptionDtoValidator.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>, BadRequest<string>>>();

        var notFoundResult = (NotFound<string>)result.Result;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be(EndpointsConstants.Option.NOT_FOUND_MESSAGE);

        _mockOptionRepository.Verify(repo => repo.UpdateOptionAsync(expectedId, updatedOption), Times.Once);
    }

    [Fact]
    public async Task OptionsEndpoints_DeleteOptionAsync_ReturnsNoContentResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = true;

        _mockOptionRepository.Setup(repo =>
            repo.DeleteOptionAsync(It.Is<int>(id => id == expectedId))).ReturnsAsync(expectedResult);

        // Act
        var result = await OptionsEndpoints.DeleteOptionAsync(expectedId, _mockOptionRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>>>();

        var noContentResult = (NoContent)result.Result;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _mockOptionRepository.Verify(repo => repo.DeleteOptionAsync(expectedId), Times.Once);
    }

    [Fact]
    public async Task OptionsEndpoints_DeleteOptionAsync_ReturnsNotFoundResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = false;

        _mockOptionRepository.Setup(repo =>
            repo.DeleteOptionAsync(It.Is<int>(id => id == expectedId))).ReturnsAsync(expectedResult);

        // Act
        var result = await OptionsEndpoints.DeleteOptionAsync(expectedId, _mockOptionRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>>>();

        var notFoundResult = (NotFound<string>)result.Result;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be(EndpointsConstants.Option.NOT_FOUND_MESSAGE);

        _mockOptionRepository.Verify(repo => repo.DeleteOptionAsync(expectedId), Times.Once);
    }
}