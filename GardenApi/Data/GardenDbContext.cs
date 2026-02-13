using Microsoft.EntityFrameworkCore;
using GardenApi.Models;

namespace GardenApi.Data;

public class GardenDbContext : DbContext
{
    public DbSet<Plant> Plants { get; set; } = null!;

    public GardenDbContext(DbContextOptions<GardenDbContext> options)
        : base(options) { }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Plant>().HasData(
        new Plant 
        { 
            Id = 1, 
            Name = "Eggplant", 
            StartMonth = 4, 
            EndMonth = 5, 
            Instructions = "Use special fertilizer." 
        },
        new Plant 
        { 
            Id = 2, 
            Name = "Carrot", 
            StartMonth = 2, 
            EndMonth = 3, 
            Instructions = "Use deep planters." 
        },
        new Plant 
        { 
            Id = 3, 
            Name = "Tulip", 
            StartMonth = 9, 
            EndMonth = 11, 
            Instructions = "Use special fertilizer." 
        },
        new Plant 
        { 
            Id = 4, 
            Name = "Daffodils", 
            StartMonth = 9, 
            EndMonth = 11, 
            Instructions = "Use special fertilizer." 
        },
        new Plant 
        { 
            Id = 5, 
            Name = "Garlic", 
            StartMonth = 10, 
            EndMonth = 11, 
            Instructions = "Use big planters." 
        },
        new Plant 
        { 
            Id = 6, 
            Name = "Onions", 
            StartMonth = 10, 
            EndMonth = 11, 
            Instructions = "Use big planters." 
        }
    );
}
}