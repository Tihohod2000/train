using ClientGrpcService2.Services;

using Grpc.Net.Client;
using GrpcStationService;  // Это пространство имен из сгенерированного кода
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Создаём канал для общения с сервером
        var channel = GrpcChannel.ForAddress("https://localhost:5001");  // Адрес вашего сервера
        var client = new WagonService.WagonServiceClient(channel);

        // Запрос даты у пользователя
        Console.Write("Введите дату (YYYY-MM-DD): ");
        string date = Console.ReadLine();

        try
        {
            // Создаём запрос
            var request = new WagonRequest { Date = date };

            // Отправляем запрос на сервер и получаем ответ
            var response = await client.GetWagonsByDateAsync(request);

            // Выводим полученные данные
            Console.WriteLine("Полученные вагоны:");
            foreach (var wagon in response.Wagons)
            {
                Console.WriteLine($"Id: {wagon.Id}, Type: {wagon.Type}, Departure Date: {wagon.DepartureDate}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        Console.ReadKey();
    }
}
