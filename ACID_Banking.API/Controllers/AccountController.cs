using ACID_Banking.API.DB;
using ACID_Banking.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ACID_Banking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly BankingContext _context;

        public AccountController(BankingContext context)
        {
            _context = context;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            return Ok("User Account Added Successfully.");
        }

        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit(int accountId, decimal amount)
        {
            if (amount < 0)
            {
                return BadRequest("Deposit amount must be greater than zero.");
            }
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return NotFound("Account Not Found!");
            }
            account.Balance += amount;
            await _context.SaveChangesAsync();

            return Ok($"Deposited {amount} $ Successfully. New Balance: {account.Balance} $");
        }

        [HttpPost("Withdraw")]
        public async Task<IActionResult> Withdraw(int accountId, decimal amount)
        {
            if (amount < 0)
            {
                return BadRequest("Withdraw amount must be greater than zero.");
            }
            var account = _context.Accounts.Find(accountId);
            if (account == null)
            {
                return NotFound("Account Not Found!");
            }
            if (account.Balance < amount)
            {
                return BadRequest("Insufficient Balance!");
            }
            account.Balance-= amount;

            await _context.SaveChangesAsync();
            return Ok($"Withdraw {amount} $ Successfully. New Balance: {account.Balance} $");
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(int fromAccountId, int toAccountId, decimal amount)
        {
            if(amount <= 0)
            {
                return BadRequest("Transfer amount must be greater than zero.");
            }

            var fromAccount = await _context.Accounts.FindAsync(fromAccountId);
            var toAccount = await _context.Accounts.FindAsync(toAccountId);

            if(fromAccount == null || toAccount == null)
            {
                return BadRequest("One or Both Accounts not found!");
            }

            if(fromAccount.Balance < amount)
            {
                return BadRequest("Insufficient balance in the senders account");
            }

            // Use a transaction to ensure atomicity
            using var transaction =await _context.Database.BeginTransactionAsync();
            try
            {
                fromAccount.Balance -= amount;
                toAccount.Balance += amount;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // Commit transaction for atomicity
            }
            catch
            {
                await transaction.RollbackAsync(); // Rollback transaction in case of failure
                return StatusCode(500, "An error occurred while processing the transaction!");
            }

            return Ok($"Transferred {amount} $ from Acc {fromAccount.AccountId} ({fromAccount.AccountHolder}) to Acc {toAccount.AccountId} ({toAccount.AccountHolder}) Successfully.");
        }
    }
}
