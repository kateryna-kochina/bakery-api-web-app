using AutoMapper;
using Bakery.Dtos;
using Bakery.Endpoints;
using Bakery.Models;
using Bakery.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Bakery.Tests.Endpoints;

public class ProductsEndpointsTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateProductDto>> _mockCreateProductDtoValidator;

    public ProductsEndpointsTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateProductDtoValidator = new Mock<IValidator<CreateProductDto>>();
    }

    [Fact]
    public async Task ProductsEndpoints_GetProductsAsync_ReturnsOkResult()
    {
        // Arrange
        var expectedCategoryId = 1;
        var products = new List<Product> { new() { Id = 1, Title = "Banana Cake", Price = 120, Image = "link/to/img", CategoryId = expectedCategoryId, Description = "Best banana cake!" } };
        var productsDtos = new List<ProductDetailsDto> { new(1, "Banana Cake", 120, "link/to/img", 1, "Cakes", "Best banana cake!") };

        _mockProductRepository.Setup(repo => repo.GetProductsAsync(It.Is<int>(id => id == expectedCategoryId))).ReturnsAsync(products);
        _mockMapper.Setup(m => m.Map<List<ProductDetailsDto>>(products)).Returns(productsDtos);

        // Act
        var result = await ProductsEndpoints.GetProductsAsync(expectedCategoryId, _mockProductRepository.Object, _mockMapper.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Ok<List<ProductDetailsDto>>>();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(productsDtos);

        _mockProductRepository.Verify(repo => repo.GetProductsAsync(expectedCategoryId), Times.Once);
        _mockMapper.Verify(m => m.Map<List<ProductDetailsDto>>(products), Times.Once);
    }

    [Fact]
    public async Task ProductsEndpoints_GetProductByIdAsync_ReturnsOkResult()
    {
        // Arrange
        var expectedId = 1;
        var product = new Product { Id = expectedId, Title = "Banana Cake", Price = 120, Image = "link/to/img", CategoryId = 1, Description = "Best banana cake!" };
        var productDetailsDto = new ProductDetailsDto(expectedId, "Banana Cake", 120, "link/to/img", 1, "Cakes", "Best banana cake!");

        _mockProductRepository.Setup(repo =>
            repo.GetProductByIdAsync(It.Is<int>(id => id == product.Id))).ReturnsAsync(product);
        _mockMapper.Setup(m => m.Map<ProductDetailsDto>(product)).Returns(productDetailsDto);

        // Act
        var result = await ProductsEndpoints.GetProductByIdAsync(expectedId, _mockProductRepository.Object, _mockMapper.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<Ok<ProductDetailsDto>, NotFound<string>>>();

        var okResult = (Ok<ProductDetailsDto>)result.Result;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(productDetailsDto);

        _mockProductRepository.Verify(repo => repo.GetProductByIdAsync(expectedId), Times.Once);
        _mockMapper.Verify(m => m.Map<ProductDetailsDto>(product), Times.Once);
    }

    [Fact]
    public async Task ProductsEndpoints_GetProductByIdAsync_ReturnsNotFoundResult()
    {
        // Arrange
        var existingId = 1;
        var nonExistingProductId = 10;
        var product = new Product { Id = existingId, Title = "Banana Cake", Price = 120, Image = "link/to/img", CategoryId = 1, Description = "Best banana cake!" };
        var productDetailsDto = new ProductDetailsDto(existingId, "Banana Cake", 120, "link/to/img", 1, "Cakes", "Best banana cake!");

        _mockProductRepository.Setup(repo =>
            repo.GetProductByIdAsync(It.Is<int>(id => id == product.Id))).ReturnsAsync(product);
        _mockMapper.Setup(m => m.Map<ProductDetailsDto>(product)).Returns(productDetailsDto);

        // Act
        var result = await ProductsEndpoints.GetProductByIdAsync(nonExistingProductId, _mockProductRepository.Object, _mockMapper.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<Ok<ProductDetailsDto>, NotFound<string>>>();

        var notFoundResult = (NotFound<string>)result.Result;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be(EndpointsConstants.Product.NOT_FOUND_MESSAGE);

        _mockProductRepository.Verify(repo => repo.GetProductByIdAsync(nonExistingProductId), Times.Once);
        _mockMapper.Verify(m => m.Map<ProductDetailsDto>(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task ProductsEndpoints_CreateProductAsync_ReturnOkResult()
    {
        // Arrange
        var newProduct = new CreateProductDto("Banana Cake", 120, "link/to/img", 1, "Best banana cake!");
        var createdProduct = new Product { Id = 1, Title = "Banana Cake", Price = 120, Image = "link/to/img", CategoryId = 1, Description = "Best banana cake!" };
        var createdProductDetailsDto = new ProductDetailsDto(1, "Banana Cake", 120, "link/to/img", 1, "Cakes", "Best banana cake!");

        _mockProductRepository.Setup(repo =>
            repo.CreateProductAsync(newProduct)).ReturnsAsync(createdProduct);
        _mockMapper.Setup(m => m.Map<ProductDetailsDto>(createdProduct)).Returns(createdProductDetailsDto);
        _mockCreateProductDtoValidator.Setup(v => v.ValidateAsync(newProduct, default))
                .ReturnsAsync(new ValidationResult());

        // Act
        var result = await ProductsEndpoints.CreateProductAsync(
            newProduct, _mockProductRepository.Object, _mockMapper.Object, _mockCreateProductDtoValidator.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<Created<ProductDetailsDto>, BadRequest<string>>>();

        var createdResult = (Created<ProductDetailsDto>)result.Result;
        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.Value.Should().BeEquivalentTo(createdProductDetailsDto);

        _mockProductRepository.Verify(repo => repo.CreateProductAsync(newProduct), Times.Once);
        _mockMapper.Verify(m => m.Map<ProductDetailsDto>(createdProduct), Times.Once);
    }

    [Fact]
    public async Task ProductsEndpoints_UpdateProductAsync_ReturnsNoContentResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = true;
        var updatedProduct = new UpdateProductDto("Banana Cake", 120, "link/to/img", 1, "Best banana cake!");

        _mockProductRepository.Setup(repo =>
            repo.UpdateProductAsync(It.Is<int>(id => id == expectedId), updatedProduct)).ReturnsAsync(expectedResult);

        // Act
        var result = await ProductsEndpoints.UpdateProductAsync(expectedId, updatedProduct, _mockProductRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>>>();

        var noContentResult = (NoContent)result.Result;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _mockProductRepository.Verify(repo => repo.UpdateProductAsync(expectedId, updatedProduct), Times.Once);
    }

    [Fact]
    public async Task ProductsEndpoints_UpdateProductAsync_ReturnsNotFoundResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = false;
        var updatedProduct = new UpdateProductDto("Banana Cake", 120, "link/to/img", 1, "Best banana cake!");

        _mockProductRepository.Setup(repo =>
            repo.UpdateProductAsync(It.Is<int>(id => id == expectedId), updatedProduct)).ReturnsAsync(expectedResult);

        // Act
        var result = await ProductsEndpoints.UpdateProductAsync(expectedId, updatedProduct, _mockProductRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>>>();

        var notFoundResult = (NotFound<string>)result.Result;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be(EndpointsConstants.Product.NOT_FOUND_MESSAGE);

        _mockProductRepository.Verify(repo => repo.UpdateProductAsync(expectedId, updatedProduct), Times.Once);
    }

    [Fact]
    public async Task ProductsEndpoints_DeleteProductAsync_ReturnsNoContentResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = true;

        _mockProductRepository.Setup(repo =>
            repo.DeleteProductAsync(It.Is<int>(id => id == expectedId))).ReturnsAsync(expectedResult);

        // Act
        var result = await ProductsEndpoints.DeleteProductAsync(expectedId, _mockProductRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>>>();

        var noContentResult = (NoContent)result.Result;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _mockProductRepository.Verify(repo => repo.DeleteProductAsync(expectedId), Times.Once);
    }

    [Fact]
    public async Task ProductsEndpoints_DeleteProductAsync_ReturnsNotFoundResult()
    {
        // Arrange
        var expectedId = 1;
        var expectedResult = false;

        _mockProductRepository.Setup(repo =>
            repo.DeleteProductAsync(It.Is<int>(id => id == expectedId))).ReturnsAsync(expectedResult);

        // Act
        var result = await ProductsEndpoints.DeleteProductAsync(expectedId, _mockProductRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Results<NoContent, NotFound<string>>>();

        var notFoundResult = (NotFound<string>)result.Result;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be(EndpointsConstants.Product.NOT_FOUND_MESSAGE);

        _mockProductRepository.Verify(repo => repo.DeleteProductAsync(expectedId), Times.Once);
    }
}