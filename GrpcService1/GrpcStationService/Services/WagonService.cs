using GrpcStationService.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace GrpcStationService.Services
{
    public class WagonService : GrpcStationService.WagonService.WagonServiceBase
    {
        private readonly WagonsDbContext _dbContext;

        public WagonService(WagonsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<WagonResponse> GetWagonsByDate(WagonRequest request, ServerCallContext context)
        {
            // Получаем DateTime из запроса
            DateTime startDate = request.DateStart.ToDateTime(); // Для DateTime
            DateTime endDate = request.DateEnd.ToDateTime(); // Для DateTime
            
            // SQL-запрос с использованием параметров
            var query = @"
SELECT DISTINCT
    ""Epc"".""Number"" AS ""InventoryNumber"",
    ""EventArrival"".""Time"" AS ""ArrivalTime"",
    ""EventDeparture"".""Time"" AS ""DepartureTime""
FROM
    ""Epc""
JOIN
    ""EpcEvent"" ON ""Epc"".""Id"" = ""EpcEvent"".""IdEpc""
JOIN
    ""EventArrival"" ON ""EpcEvent"".""IdPath"" = ""EventArrival"".""IdPath""
JOIN
    ""EventDeparture"" ON ""EpcEvent"".""IdPath"" = ""EventDeparture"".""IdPath""
WHERE
    ""Epc"".""Type"" = 1
    AND ""EventDeparture"".""Time"" BETWEEN @p0 AND @p1
    AND ""Epc"".""Number"" != '00000000'
    AND ""EventDeparture"".""Time"" > ""EventArrival"".""Time""";
            
            
            // Выполнение запроса с параметрами DateTime
            var wagons = await _dbContext.Wagons
                .FromSqlRaw(query, startDate, endDate)  // Параметры DateTime
                .Select(w => new WagonInfo
                {
                    InventoryNumber = w.InventoryNumber,
                    ArrivalTime = w.ArrivalTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    DepartureTime = w.DepartureTime.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync();

            // Формируем ответ
            var response = new WagonResponse();
            foreach (var wagon in wagons)
            {
                response.Wagons.Add(wagon);
            }

            return response;
        }
    }
}
