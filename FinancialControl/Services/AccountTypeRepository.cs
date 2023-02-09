using Dapper;
using FinancialControl.Models;
using Microsoft.Data.SqlClient;

namespace FinancialControl.Services
{
    public interface IAccountTypeRepository
    {
        Task Create(AccountType accountType);
        Task Delete(int Id);
        Task<bool> Exists(string name, int idUser);
        Task<IEnumerable<AccountType>> Get(int UserId);
        Task<AccountType> getById(int Id, int UserId);
        Task Order(IEnumerable<AccountType> accountTypesSorted);
        Task Update(AccountType accountType);
    }
    public class AccountTypeRepository: IAccountTypeRepository
    {
        private readonly string connectionString;
        public AccountTypeRepository(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(AccountType accountType)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
                                                    (@"INSERT INTO AccountType(Name, UserId, Orden)
                                                    values (@Name, @UserId,0);
                                                    SELECT SCOPE_IDENTITY();", accountType);
            accountType.Id = id;
        }

        public async Task<bool> Exists(string name, int UserId)
        {
            using var connection = new SqlConnection(connectionString);
            var exists = await connection.QueryFirstOrDefaultAsync<int>(
                                                                        @"SELECT 1
                                                                        FROM AccountType
                                                                        WHERE Name = @Name and UserId = @UserId;",
                                                                        new { name, UserId });
            return exists == 1;
        }

        public async Task<IEnumerable<AccountType>> Get(int UserId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<AccountType>(@"SELECT Id, Name, Orden
                                                            FROM AccountType
                                                            WHERE UserId = @UserId
                                                            ORDER BY Orden",
                                                            new { UserId });
        }
        public async Task Update(AccountType accountType)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE AccountType
                                            set Name = @Name
                                            WHERE Id = @Id", accountType);
        }

        public async Task<AccountType> getById(int Id, int UserId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<AccountType>(@"SELECt Id, Name, Orden
                                                                            FROM AccountType
                                                                            WHERE Id = @Id and UserId = @UserId",
                                                                            new { Id, UserId });
        }

        public async Task Delete(int Id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE AccountType WHERE Id = @Id", new { Id });
        }

        public async Task Order(IEnumerable<AccountType> accountTypesSorted)
        {
            var query = "UPDATE AccountType SET Orden = @Orden WHERE Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, accountTypesSorted);
        }
    }
}
