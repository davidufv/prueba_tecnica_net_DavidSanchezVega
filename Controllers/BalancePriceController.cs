using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prueba_tecnica_net.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class BalancePriceController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly BalancePriceService _balancePriceService;


    //ATENCION, estos metodos tienen sus respectivas pruebasa con Moq en la carpeta TEST
    public BalancePriceController(ApplicationDbContext context, BalancePriceService balancePriceService)
    {
        _context = context;
        _balancePriceService = balancePriceService;
    }

    // Obtener BalancePrice por ID (Primary Key)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBalancePrice(int id)
    {
        // Buscar en la base de datos por la clave primaria
        var balancePrice = await _context.BalancePricesBd.FindAsync(id);

        if (balancePrice == null)
        {
            return NotFound();
        }

        return Ok(balancePrice);
    }

    [HttpGet("fetch")]
    public async Task<IActionResult> FetchAndReturnData(string start, string end, [FromQuery] List<string> mba)
    {
        // Llamada al servicio para obtener los datos desde la API externa
        var externalPrices = await _balancePriceService.GetBalancePrices(start, end, mba);

        // Mapeo: BalancePrice (API externa) a BalancePriceBd (base de datos)
        var balancePricesBd = externalPrices.Select(price => new BalancePriceBd
        {
            DownRegPrice = price.DownRegPrice,
            DownRegPriceFrrA = price.DownRegPriceFrrA,
            ImblPurchasePrice = price.ImblPurchasePrice,
            ImblSalesPrice = price.ImblSalesPrice,
            ImblSpotDifferencePrice = price.ImblSpotDifferencePrice,
            IncentivisingComponent = price.IncentivisingComponent,
            MainDirRegPowerPerMBA = price.MainDirRegPowerPerMBA,
            Mba = price.Mba,
            Timestamp = !string.IsNullOrEmpty(price.Timestamp) ? DateTime.Parse(price.Timestamp) : DateTime.MinValue,
            TimestampUTC = !string.IsNullOrEmpty(price.TimestampUTC) ? DateTime.Parse(price.TimestampUTC) : DateTime.MinValue,
            UpRegPrice = price.UpRegPrice,
            UpRegPriceFrrA = price.UpRegPriceFrrA,
            ValueOfAvoidedActivation = price.ValueOfAvoidedActivation
        }).ToList();

        // Retorna los datos sin almacenarlos en la base de datos
        return Ok(balancePricesBd);
    }


    // Llamar a la API externa, almacenar los datos en la base de datos y devolver los datos
    [HttpGet("fetch")]
    public async Task<IActionResult> FetchAndStoreData(string start, string end, [FromQuery] List<string> mba)
    {
        // Llamada al servicio para obtener los datos desde la API externa
        var externalPrices = await _balancePriceService.GetBalancePrices(start, end, mba);
        // Mapeo: BalancePrice (API externa) a BalancePriceBd (Base de datos)
        var balancePricesBd = externalPrices.Select(price => new BalancePriceBd
        {
            DownRegPrice = price.DownRegPrice,
            DownRegPriceFrrA = price.DownRegPriceFrrA,
            ImblPurchasePrice = price.ImblPurchasePrice,
            ImblSalesPrice = price.ImblSalesPrice,
            ImblSpotDifferencePrice = price.ImblSpotDifferencePrice,
            IncentivisingComponent = price.IncentivisingComponent,
            MainDirRegPowerPerMBA = price.MainDirRegPowerPerMBA,
            Mba = price.Mba,
            Timestamp = !string.IsNullOrEmpty(price.Timestamp) ? DateTime.Parse(price.Timestamp) : DateTime.MinValue,
            TimestampUTC = !string.IsNullOrEmpty(price.TimestampUTC) ? DateTime.Parse(price.TimestampUTC) : DateTime.MinValue,

            UpRegPrice = price.UpRegPrice,
            UpRegPriceFrrA = price.UpRegPriceFrrA,
            ValueOfAvoidedActivation = price.ValueOfAvoidedActivation
        }).ToList();

        // Almacenar en la base de datos
        _context.BalancePricesBd.AddRange(balancePricesBd);
        await _context.SaveChangesAsync();

        return Ok(balancePricesBd); // Retornar los datos almacenados
    }


}
