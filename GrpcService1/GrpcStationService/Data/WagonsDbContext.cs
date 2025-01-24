using Microsoft.EntityFrameworkCore;
using System;

namespace GrpcStationService.Data
{
    // Модель для сущности вагона
    public class Wagon
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime DepartureDate { get; set; }
        public string InventoryNumber { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
    }

    // Контекст базы данных
    public class WagonsDbContext : DbContext
    {
        public WagonsDbContext(DbContextOptions<WagonsDbContext> options) : base(options)
        {
        }

        // DbSet для сущности вагона
        public DbSet<Wagon> Wagons { get; set; }
    }

    // Модель для нужных данных (Инвентарный номер вагона, Время прибытия и Время отправления)
    public class WagonInfo
    {
        public string InventoryNumber { get; set; }
        public string ArrivalTime { get; set; }
        public string DepartureTime { get; set; }
    }
}