using ACID_Banking.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ACID_Banking.API.DB
{
    public class BankingContext:DbContext
    {
        public BankingContext(DbContextOptions<BankingContext> options) : base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; }
    }
}
