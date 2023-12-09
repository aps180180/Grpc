using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcManutencao.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcManutencao.Data
{
    public class GrpcDbContext :DbContext
    {
        public GrpcDbContext(DbContextOptions<GrpcDbContext> options): base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }
    }
}