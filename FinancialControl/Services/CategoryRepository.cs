using Dapper;
using FinancialControl.Models;
using Microsoft.Data.SqlClient;

namespace FinancialControl.Services
{
    public interface ICategoryRepository
    {
        Task Create(Category category);
        Task Delete(int Id);
        Task<IEnumerable<Category>> Get(int UserId);
        Task<IEnumerable<Category>> Get(int UserId, OperationType operationTypeId);
        Task<Category> GetById(int Id, int UserId);
        Task Update(Category categrory);
    }
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string connectionString;
        public CategoryRepository(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Category category)
        {
            using var connetcion = new SqlConnection(connectionString);
            var id = await connetcion.QuerySingleAsync<int>(@"INSERT INTO Categories (Name, OperationTypeId, UserId)
                                                              VALUES (@Name, @OperationTypeId, @UserId);
                                                              SELECT SCOPE_IDENTITY();", category);
            category.Id = id;
        }

        public async Task<IEnumerable<Category>> Get(int UserId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(@"SELECT * FROM Categories WHERE UserId = @UserId;", new { UserId });
        }

        public async Task<IEnumerable<Category>> Get(int UserId, OperationType operationTypeId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(@"
                                                        SELECT * FROM Categories 
                                                        WHERE UserId = @UserId
                                                        AND OperationTypeId = @OperationTypeId;", new { UserId, operationTypeId });
        }

        public async Task<Category> GetById(int Id, int UserId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Category>(@"
                        SELECT * FROM Categories WHERE Id = @Id and UserId = @UserId", new { Id, UserId });
        }

        public async Task Update(Category categrory)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categories
                                            SET Name = @Name, OperationTypeId = @OperationTypeId
                                            WHERE Id = @Id;", categrory);
        }

        public async Task Delete (int Id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE Categories WHERE Id = @Id;", new { Id });
        }
    }
}
