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
            if (!DateTime.TryParse(request.Date, out var requestedDate))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid date format."));
            }

            // Запрос к базе данных через Entity Framework
            var wagons = await _dbContext.Wagons
                .Where(w => w.DepartureDate.Date == requestedDate.Date)
                .ToListAsync();

            var response = new WagonResponse
            {
                Wagons = { wagons.Select(w => new Wagon
                {
                    Id = w.Id,
                    Type = w.Type,
                    DepartureDate = w.DepartureDate.ToString("yyyy-MM-dd")
                }) }
            };
            
            // var response = new WagonResponse
            // {
            //     Wagons = { 
            //         Id = 12,
            //         Type = w.Type,
            //         DepartureDate = w.DepartureDate.ToString("yyyy-MM-dd")
            //     } 
            // };

            return response;
        }
    }
}
