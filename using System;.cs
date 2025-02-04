using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main()
    {
        do
        {
            Console.Write("¿Cuántos usuarios aleatorios deseas obtener? ");
            if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
            {
                Console.WriteLine("Por favor, ingresa un número válido.");
                continue;
            }

            Console.WriteLine("Obteniendo usuarios...");

            var tasks = new List<Task>();
            for (int i = 0; i < cantidad; i++)
            {
                tasks.Add(ObtenerUsuarioAsync(i + 1));
            }

            await Task.WhenAll(tasks);

            Console.Write("¿Deseas buscar más usuarios? (s/n): ");
        } while (Console.ReadLine()?.Trim().ToLower() == "s");

        Console.WriteLine("Gracias por usar la aplicación. ¡Hasta luego!");
    }

    static async Task ObtenerUsuarioAsync(int index)
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync("https://randomuser.me/api/");
            NewMethod(response);

            string responseBody = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            var user = doc.RootElement.GetProperty("results")[0];

            string nombre = $"{user.GetProperty("name").GetProperty("first").GetString()} {user.GetProperty("name").GetProperty("last").GetString()}";
            string genero = user.GetProperty("gender").GetString();
            string correo = user.GetProperty("email").GetString();
            string pais = user.GetProperty("location").GetProperty("country").GetString();

            Console.WriteLine($"Usuario {index}: {nombre}, Género: {genero}, Correo: {correo}, País: {pais}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener el usuario {index}: {ex.Message}. Reintentando...");
            await ObtenerUsuarioAsync(index);
        }
    }

    private static void NewMethod(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
    }
}