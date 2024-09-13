using prueba_tecnica_net.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class BalancePriceService
{
    private readonly HttpClient _httpClient;

    public BalancePriceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public BalancePriceService()
    {
        // Constructor vacío para crear un proxy
    }

    public virtual async Task<List<BalancePrice>> GetBalancePrices(string start, string end, List<string> mbaCodes)
    {
        // Construye la parte de la consulta con los parámetros 'start' y 'end'
        var query = $"start={start}&end={end}";

        // Añade cada código MBA a la consulta como un parámetro de consulta adicional
        foreach (var mba in mbaCodes)
        {
            query += $"&mba={mba}";
        }

        // Realiza una solicitud HTTP GET a la API externa utilizando la consulta construida
        var response = await _httpClient.GetAsync($"https://api.opendata.esett.com/balance-prices?{query}");

        // Comprueba si la respuesta de la solicitud HTTP es buena
        if (response.IsSuccessStatusCode)
        {
            // Lee el contenido de la respuesta y deserialízalo en una lista de objetos BalancePrice
            return await response.Content.ReadFromJsonAsync<List<BalancePrice>>();
        }

        // Si la solicitud no fue buena, devuelve una lista vacía de BalancePrice
        return new List<BalancePrice>();
    }

}
