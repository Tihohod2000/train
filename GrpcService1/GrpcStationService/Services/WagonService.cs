using GrpcStationService.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Grpc.AspNetCore.Web;


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
          
            // ""Epc"".""Number"" AS ""InventoryNumber"",
            // ""EventArrival"".""Time"" AS ""ArrivalTime"",
            // ""EventDeparture"".""Time"" AS ""DepartureTime""
            
            // SQL-запрос с использованием параметров
            var query = @"SELECT
                ""Epc"".""Number"" AS ""InventoryNumber"",
                MIN(""EventArrival"".""Time"") AS ""ArrivalTime"",
                MAX(""EventDeparture"".""Time"") AS ""DepartureTime""
                --""Path"".""Id"" AS ""Идентификатор пути""
                --""EpcEvent"".""Type"" AS ""Тип события""
                FROM
                ""Epc""
                LEFT JOIN
                    ""EpcEvent"" ON ""Epc"".""Id"" = ""EpcEvent"".""IdEpc""
                LEFT JOIN
                    ""EventArrival"" ON ""EpcEvent"".""IdPath"" = ""EventArrival"".""IdPath""
                LEFT JOIN
                    ""EventDeparture"" ON ""EpcEvent"".""IdPath"" = ""EventDeparture"".""IdPath""
                LEFT JOIN
                    ""Path"" ON ""EpcEvent"".""IdPath"" = ""Path"".""Id""
                WHERE
                    ""Epc"".""Type"" = 1  -- Только вагоны
                    AND ""Epc"".""Number"" != '00000000'
                    AND ""EventDeparture"".""Time"" BETWEEN @p0 AND @p1
                    AND ""EventArrival"".""IdPath"" = ""EventDeparture"".""IdPath""
                    AND ""EventDeparture"".""Time"" > ""EventArrival"".""Time""
                GROUP BY
                    ""Epc"".""Number"", ""Path"".""Id""";
            
            
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
