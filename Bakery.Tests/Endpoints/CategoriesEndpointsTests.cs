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

public class CategoriesEndpointsTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateCategoryDto>> _mockCreateCategoryDtoValidator;

    public CategoriesEndpointsTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateCategoryDtoValidator = new Mock<IValidator<CreateCategoryDto>>();
    }

    [Fact]
    public async Task CategoriesEndpoints_GetCategoriesAsync_ReturnsOkResult()
    {
        // Arrange
        var categories = new List<Category> { new() { Id = 1, CategoryName = "Bread" } };
        var categoriesDtos = new List<CategoryDetailsDto> { new(1, "Bread") };

        _mockCategoryRepository.Setup(repo => repo.GetCategoriesAsync()).ReturnsAsync(categories);
        _mockMapper.Setup(m => m.Map<List<CategoryDetailsDto>>(categories)).Returns(categoriesDtos);

        // Act
        var result = await CategoriesEndpoints.GetCategoriesAsync(_mockCategoryRepository.Object, _mockMapper.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Ok<List<CategoryDetailsDto>>>();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(categoriesDtos);

        _mockCategoryRepository.Verify(repo => repo.GetCategoriesAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<List<CategoryDetailsDto>>(categories), Times.Once);
    }

    [Fact]
    public async Task CategoriesEndpoints_GetCategoryByIdAsync_ReturnsOkResult()
    {
        // Arrange
        var expectedId = 1;
        var category = new Category { Id = expectedId, CategoryName = "Bread" };
        var categoryDetailsDto = new CategoryDetailsDto(expectedId, "Bread");

        _mockCategoryRepository.Setup(repo =>
            repo.GetCategoryByIdAsync(It.Is<int>(id => id == category.Id))).ReturnsAsync(category);
        _mockMapper.Setup(m => m.Map<CategoryDetailsDto>(category)).Returns(categoryDetailsDto);

        // Act
        var result = await CategoriesEndpoints.GetCategoryByIdAsync(expectedId, _mockCategoryRepository.Object, _mockMapper.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<Ok<CategoryDetailsDto>, NotFound<string>>>();

        var okResult = (Ok<CategoryDetailsDto>)result.Result;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(categoryDetailsDto);

        _mockCategoryRepository.Verify(repo => repo.GetCategoryByIdAsync(expectedId), Times.Once);
        _mockMapper.Verify(m => m.Map<CategoryDetailsDto>(category), Times.Once);
    }

    [Fact]
    public async Task CategoriesEndpoints_GetCategoryByIdAsync_ReturnsNotFoundResult()
    {
        // Arrange
        var existingId = 1;
        var nonExistingCategoryId = 10;
        var category = new Category { Id = existingId, CategoryName = "Bread" };
        var categoryDetailsDto = new CategoryDetailsDto(existingId, "Bread");

        _mockCategoryRepository.Setup(repo =>
            repo.GetCategoryByIdAsync(It.Is<int>(id => id == category.Id))).ReturnsAsync(category);
        _mockMapper.Setup(m => m.Map<CategoryDetailsDto>(category)).Returns(categoryDetailsDto);

        // Act
        var result = await CategoriesEndpoints.GetCategoryByIdAsync(nonExistingCategoryId, _mockCategoryRepository.Object, _mockMapper.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<Ok<CategoryDetailsDto>, NotFound<string>>>();

        var notFoundResult = (NotFound<string>)result.Result;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be(EndpointsConstants.Category.NOT_FOUND_MESSAGE);

        _mockCategoryRepository.Verify(repo => repo.GetCategoryByIdAsync(nonExistingCategoryId), Times.Once);
        _mockMapper.Verify(m => m.Map<CategoryDetailsDto>(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task CategoriesEndpoints_CreateCategoryAsync_ReturnOkResult()
    {
        // Arrange
        var newCategory = new CreateCategoryDto("Bread");
        var createdCategory = new Category { Id = 1, CategoryName = "Bread" };
        var createdCategoryDetailsDto = new CategoryDetailsDto(1, "Bread");

        _mockCategoryRepository.Setup(repo =>
            repo.CreateCategoryAsync(newCategory)).ReturnsAsync(createdCategory);
        _mockMapper.Setup(m => m.Map<CategoryDetailsDto>(createdCategory)).Returns(createdCategoryDetailsDto);
        _mockCreateCategoryDtoValidator.Setup(v => v.ValidateAsync(newCategory, default))
                .ReturnsAsync(new ValidationResult());

        // Act
        var result = await CategoriesEndpoints.CreateCategoryAsync(
            newCategory, _mockCategoryRepository.Object, _mockMapper.Object, _mockCreateCategoryDtoValidator.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<Created<CategoryDetailsDto>, BadRequest<string>>>();

        var createdResult = (Created<CategoryDetailsDto>)result.Result;
        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.Value.Should().BeEquivalentTo(createdCategoryDetailsDto);

        _mockCategoryRepository.Verify(repo => repo.CreateCategoryAsync(newCategory), Times.Once);
        _mockMapper.Verify(m => m.Map<CategoryDetailsDto>(createdCategory), Times.Once);
    }

    [Fact]
    public async Task CategoriesEndpoints_UpdateCategoryAsync_ReturnsNoContentResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = true;
        var updatedCategory = new UpdateCategoryDto("Bread");

        _mockCategoryRepository.Setup(repo =>
            repo.UpdateCategoryAsync(It.Is<int>(id => id == expectedId), updatedCategory)).ReturnsAsync(expectedResult);

        // Act
        var result = await CategoriesEndpoints.UpdateCategoryAsync(expectedId, updatedCategory, _mockCategoryRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>>>();

        var noContentResult = (NoContent)result.Result;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _mockCategoryRepository.Verify(repo => repo.UpdateCategoryAsync(expectedId, updatedCategory), Times.Once);
    }

    [Fact]
    public async Task CategoriesEndpoints_UpdateCategoryAsync_ReturnsNotFoundResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = false;
        var updatedCategory = new UpdateCategoryDto("Bread");

        _mockCategoryRepository.Setup(repo =>
            repo.UpdateCategoryAsync(It.Is<int>(id => id == expectedId), updatedCategory)).ReturnsAsync(expectedResult);

        // Act
        var result = await CategoriesEndpoints.UpdateCategoryAsync(expectedId, updatedCategory, _mockCategoryRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>>>();

        var notFoundResult = (NotFound<string>)result.Result;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be(EndpointsConstants.Category.NOT_FOUND_MESSAGE);

        _mockCategoryRepository.Verify(repo => repo.UpdateCategoryAsync(expectedId, updatedCategory), Times.Once);
    }

    [Fact]
    public async Task CategoriesEndpoints_DeleteCategoryAsync_ReturnsNoContentResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = true;

        _mockCategoryRepository.Setup(repo =>
            repo.DeleteCategoryAsync(It.Is<int>(id => id == expectedId))).ReturnsAsync(expectedResult);

        // Act
        var result = await CategoriesEndpoints.DeleteCategoryAsync(expectedId, _mockCategoryRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>>>();

        var noContentResult = (NoContent)result.Result;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _mockCategoryRepository.Verify(repo => repo.DeleteCategoryAsync(expectedId), Times.Once);
    }

    [Fact]
    public async Task CategoriesEndpoints_DeleteCategoryAsync_ReturnsNotFoundResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = false;

        _mockCategoryRepository.Setup(repo =>
            repo.DeleteCategoryAsync(It.Is<int>(id => id == expectedId))).ReturnsAsync(expectedResult);

        // Act
        var result = await CategoriesEndpoints.DeleteCategoryAsync(expectedId, _mockCategoryRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>>>();

        var notFoundResult = (NotFound<string>)result.Result;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be(EndpointsConstants.Category.NOT_FOUND_MESSAGE);

        _mockCategoryRepository.Verify(repo => repo.DeleteCategoryAsync(expectedId), Times.Once);
    }
}