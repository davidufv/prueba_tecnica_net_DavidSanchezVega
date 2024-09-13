using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using prueba_tecnica_net.Models;

public class BalancePriceControllerTests
{
    private readonly Mock<BalancePriceService> _mockBalancePriceService;
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly BalancePriceController _controller;

    public BalancePriceControllerTests()
    {
        _mockBalancePriceService = new Mock<BalancePriceService>();
        _mockContext = new Mock<ApplicationDbContext>(
            new DbContextOptions<ApplicationDbContext>()
        );

        _controller = new BalancePriceController(_mockContext.Object, _mockBalancePriceService.Object);
    }

    [Fact]
    public async Task GetBalancePrice_ReturnsNotFound_WhenBalancePriceDoesNotExist()
    {
        // Arrange
        int testId = 1;
        // Configura el mock para que devuelva null cuando se busca un BalancePrice con el ID de prueba.

        _mockContext.Setup(c => c.BalancePricesBd.FindAsync(testId))
                    .ReturnsAsync((BalancePriceBd)null);

        // Act
        var result = await _controller.GetBalancePrice(testId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetBalancePrice_ReturnsOkResult_WithBalancePrice()
    {
        // Arrange
        int testId = 1;
        var testBalancePrice = new BalancePriceBd { Id = testId };
        _mockContext.Setup(c => c.BalancePricesBd.FindAsync(testId))
                    .ReturnsAsync(testBalancePrice);

        // Act
        var result = await _controller.GetBalancePrice(testId);
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<BalancePriceBd>(actionResult.Value);

        // Assert
        Assert.Equal(testId, returnValue.Id);
    }
    [Fact]
    public async Task FetchAndReturnData_ReturnsOkResult_WithFetchedData()
    {
        // Arrange
        var externalPrices = new List<BalancePrice>
    {
        new BalancePrice
        {
            DownRegPrice = 100.0m,
            DownRegPriceFrrA = 50.0m,
            ImblPurchasePrice = 75.0m,
            ImblSalesPrice = 80.0m,
            ImblSpotDifferencePrice = 10.0m,
            IncentivisingComponent = 5.0m,
            MainDirRegPowerPerMBA = 200.0m,
            Mba = "MBA1",
            Timestamp = "2024-01-01T00:00:00",
            TimestampUTC = "2024-01-01T00:00:00",
            UpRegPrice = 120.0m,
            UpRegPriceFrrA = 60.0m,
            ValueOfAvoidedActivation = 15.0m
        }
    };
        // Configura el mock para que devuelva la lista de BalancePrice de prueba.

        _mockBalancePriceService
            .Setup(service => service.GetBalancePrices(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
            .ReturnsAsync(externalPrices);


        // Act: Llama al método del controlador que se está probando.
        var result = await _controller.FetchAndReturnData("start", "end", new List<string> { "MBA1", "MBA2" });
        var actionResult = Assert.IsType<OkObjectResult>(result); // Verifica que el resultado sea un OkObjectResult (200).
        var returnValue = Assert.IsType<List<BalancePriceBd>>(actionResult.Value); // Verifica que el valor devuelto sea una lista de BalancePriceBd.

        // Assert: Verifica que la lista devuelta contiene un solo elemento y que los valores coinciden con los datos de prueba.
        Assert.Single(returnValue);
        Assert.Equal(externalPrices[0].DownRegPrice, returnValue[0].DownRegPrice);
        Assert.Equal(externalPrices[0].DownRegPriceFrrA, returnValue[0].DownRegPriceFrrA);
        Assert.Equal(externalPrices[0].ImblPurchasePrice, returnValue[0].ImblPurchasePrice);

        // Agrega más afirmaciones según sea necesario para verificar la correcta conversión y retorno de datos
    }
    [Fact]
    public async Task FetchAndStoreData_ReturnsOkResult_WithStoredData()
    {
        // Arrange: Configura el escenario de prueba.
        var externalPrices = new List<BalancePrice>
        {
            new BalancePrice { DownRegPrice = 100.0m, /* otras propiedades */ }
        };

        // Configura el mock para que devuelva la lista de BalancePrice de prueba.
        _mockBalancePriceService
            .Setup(service => service.GetBalancePrices(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
            .ReturnsAsync(externalPrices);

        // Configura el mock para verificar que AddRange y SaveChangesAsync sean llamados durante la prueba.
        _mockContext.Setup(m => m.BalancePricesBd.AddRange(It.IsAny<IEnumerable<BalancePriceBd>>()))
                    .Verifiable();
        _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1)
                    .Verifiable();

        // Act: Llama al método del controlador que se está probando.
        var result = await _controller.FetchAndStoreData("start", "end", new List<string> { "MBA1", "MBA2" });
        var actionResult = Assert.IsType<OkObjectResult>(result); // Verifica que el resultado sea un OkObjectResult (200).
        var returnValue = Assert.IsType<List<BalancePriceBd>>(actionResult.Value); // Verifica que el valor devuelto sea una lista de BalancePriceBd.

        // Assert: Verifica que la lista devuelta contiene un solo elemento y que se han llamado AddRange y SaveChangesAsync.
        Assert.Single(returnValue);
        _mockContext.Verify(m => m.BalancePricesBd.AddRange(It.IsAny<IEnumerable<BalancePriceBd>>()), Times.Once);
        _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}
