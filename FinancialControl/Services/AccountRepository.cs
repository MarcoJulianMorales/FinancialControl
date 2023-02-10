using Dapper;
using FinancialControl.Models;
using Microsoft.Data.SqlClient;

namespace FinancialControl.Services
{
    public interface IAccountRepository
    {
        Task Create(Account account);
        Task Delete(int Id);
        Task<Account> GetById(int Id, int UserId);
        Task<IEnumerable<Account>> Search(int UserId);
        Task Update(AccountCreate account);
    }
    public class AccountRepository: IAccountRepository
    {
        private readonly string connectionString;

        public AccountRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Account account)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Account(Name, AccountTypeId, Description, Balance)
                                                              VALUES (@Name, @AccountTypeId, @Description, @Balance);
                                                              SELECT SCOPE_IDENTITY();", account);
            account.Id = id;
        }

        public async Task<IEnumerable<Account>> Search(int UserId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Account>(@"SELECT a.Id, a.Name, Balance, at.Name AS AccountType
                                                          FROM Account a
                                                          INNER JOIN AccountType at
                                                          ON at.Id = a.AccountTypeId
                                                          WHERE at.UserId = @UserId
                                                          ORDER BY at.Orden", new { UserId });


        }

        public async Task<Account> GetById(int Id, int UserId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Account>(
                @"SELECT a.Id, a.Name, Balance, Description, at.Id
                 FROM Account a
                 INNER JOIN AccountType at
                 ON at.Id = a.AccountTypeId
                 WHERE at.UserId = @UserId AND a.Id = @Id", new { Id, UserId });
        }

        public async Task Update(AccountCreate account)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Account
                                        SET Name = @Name, Balance = @Balance, Description = @Description,
                                        AccountTypeId = @AccountTypeId
                                        WHERE Id = @Id", account);
        }

        public async Task Delete(int Id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE Account WHERE Id = @Id", new { Id });
        }
    }
}
