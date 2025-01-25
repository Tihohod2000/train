using ClientGrpcService2.Services;
using Grpc.Core;
using GrpcStationService;
using Google.Protobuf.WellKnownTypes; // Для работы с Timestamp
using System;
using System.Threading.Tasks;
using Grpc.Net.Client;

class Program
{
    static async Task Main(string[] args)
    {
        // Создаём канал для общения с сервером
        var channel = GrpcChannel.ForAddress("http://localhost:5000");  // Адрес вашего сервера
        var client = new WagonService.WagonServiceClient(channel);

        Console.WriteLine("Введите дату начала (YYYY-MM-DD): ");
        var dateStart = Console.ReadLine();
        Console.WriteLine("Введите дату окончания (YYYY-MM-DD): ");
        var dateEnd = Console.ReadLine();

        try
        {
            // Преобразуем строки в DateTime (с учетом времени по UTC)
            var startDate = DateTime.Parse(dateStart).ToUniversalTime();
            var endDate = DateTime.Parse(dateEnd).ToUniversalTime();

            // Выводим для проверки
            Console.WriteLine($"Start Date (UTC): {startDate}");
            Console.WriteLine($"End Date (UTC): {endDate}");

            // Преобразуем DateTime в Timestamp
            var startTimestamp = Timestamp.FromDateTime(startDate);
            var endTimestamp = Timestamp.FromDateTime(endDate);

            // Теперь создаем объект запроса с Timestamp значениями
            var wagonRequest = new WagonRequest
            {
                DateStart = startTimestamp,  // передаем Timestamp
                DateEnd = endTimestamp       // передаем Timestamp
            };

            // Отправка запроса на сервер
            var response = await client.GetWagonsByDateAsync(wagonRequest);

            // Выводим результат
            Console.WriteLine("Список вагонов:");
            foreach (var wagon in response.Wagons)
            {
                Console.WriteLine($"Инвентарный номер: {wagon.InventoryNumber}");
                Console.WriteLine($"Время прибытия: {wagon.ArrivalTime}");
                Console.WriteLine($"Время отправления: {wagon.DepartureTime}");
                Console.WriteLine();
            }
        }
        catch (RpcException e)
        {
            Console.WriteLine($"Ошибка: {e.Status.Detail}");
        }
        finally
        {
            await channel.ShutdownAsync();
        }

        Console.ReadKey();
    }
}
