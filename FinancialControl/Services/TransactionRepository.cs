using Dapper;
using FinancialControl.Models;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace FinancialControl.Services
{
    public interface ITransactionsRepository
    {
        Task Create(Transaction transaction);
        Task Delete(int Id);
        Task<Transaction> GetById(int Id, int UserId);
        Task Update(Transaction transaction, decimal PrevAmount, int PrevAccountId);
    }
    public class TransactionRepository : ITransactionsRepository
    {
        private readonly string connectionString;
        public TransactionRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Transaction transaction)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transactions_Insert", 
                new { 
                    UserId = transaction.UserId, 
                    TransactionDate = transaction.TransactionDate, 
                    Amount = transaction.Amount, 
                    CategoryId = transaction.CategoryId, 
                    AccountId = transaction.AccountId, 
                    Note = transaction.Note 
                },
                commandType: System.Data.CommandType.StoredProcedure);
            transaction.Id = id;
        }

        public async Task Update(Transaction transaction, decimal PrevAmount, int PrevAccountId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transactions_Update",
                new
                {
                    transaction.Id,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.CategoryId,
                    transaction.AccountId,
                    transaction.Note,
                    PrevAmount,
                    PrevAccountId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }
        public async Task<Transaction> GetById(int Id, int UserId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaction>(@"SELECT t.*, cat.OperationTypeId
                                                                            FROM Transactions t
                                                                            INNER JOIN Categories cat
                                                                            ON cat.Id = t.CategoryId
                                                                            WHERE t.Id = @Id and t.UserId = @UserId;",
                                                                            new { Id, UserId});
        }

        public async Task Delete(int Id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transactions_Delete",
                new { Id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
