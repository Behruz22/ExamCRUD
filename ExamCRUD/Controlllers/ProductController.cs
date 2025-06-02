using ExamCRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace ExamCRUD.Controlllers;

[ApiController]
[Route("api/[controller]")]

public class ProductController : ControllerBase
{
    private readonly string _connectionString;
    public ProductController(IConfiguration _configuration)
    {
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }
    [HttpGet("getall")]
    public async Task<ActionResult<List<Product>>> GetProductsAsync()
    {
        var products = new List<Product>();
        await using NpgsqlConnection connection = new(_connectionString);

        try
        {
            await connection.OpenAsync();
            string selectCommand = "SELECT * FROM Products";
            using var command = new NpgsqlCommand(selectCommand, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = reader.GetFloat(2),
                    Description = reader.IsDBNull(3) ? null: reader.GetString(3),
                    ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }

            await connection.CloseAsync();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        finally
        {
            connection?.Dispose();
        }

        return Ok(products);
    }

    [HttpPost("create")]
    public async Task<ActionResult<int>> CreateProduct(Product product)
    { 
       int result;
       await using NpgsqlConnection connection = new(_connectionString);
       try
       {
           await connection.OpenAsync();
           var createQuery =
               "INSERT INTO Products(Name,Price,Description,ImageUrl) VALUES(@Name,@Price,@Description,@ImageUrl)";

           using var command = new NpgsqlCommand(createQuery, connection);
           command.Parameters.AddWithValue("@Name", product.Name);
           command.Parameters.AddWithValue("@Price", product.Price);
           command.Parameters.AddWithValue("@Description", product.Description);
           command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);
           result = await command.ExecuteNonQueryAsync();
           await connection.CloseAsync();
       }
       catch (Exception ex)
       {
           return BadRequest(ex.Message);
       }
       finally
       {
           connection?.Dispose();
       }
       
       return Ok(result);
    }
    
    [HttpDelete("delete")]
    public async Task<ActionResult<int>> DeleteProduct(int id)
    {
        int result;
        await using NpgsqlConnection connection = new(_connectionString);
        try
        {
            await connection.OpenAsync();
            var deleteQuery = "DELETE FROM Products WHERE Id=@Id";
            using var command = new NpgsqlCommand(deleteQuery, connection);
            command.Parameters.AddWithValue("@Id", id);
            result = await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        finally
        {
            connection?.Dispose();
        }
        return Ok(result);
    }
    
    [HttpGet("getbyid")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        Product product;
        await using NpgsqlConnection connection = new(_connectionString);
        try
        {
            await connection.OpenAsync();
            var selectQuery = "SELECT * FROM Products WHERE Id=@Id";
            using var command = new NpgsqlCommand(selectQuery, connection);
            command.Parameters.AddWithValue("@Id", id);
            var reader = command.ExecuteReader();
            reader.Read();
            product = new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetFloat(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4)
            };
            await connection.CloseAsync();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        finally
        {
            connection?.Dispose();
        }
        
        return Ok(product);
    }
    
    [HttpPut("update")]
    public async Task<ActionResult<int>> UpdateProduct(Product product)
    {
        int result;
        await using NpgsqlConnection connection = new(_connectionString);
        try
        {
            await connection.OpenAsync();
            var updateQuery =
                "UPDATE Products SET Name=@Name,Price=@Price,Description=@Description,ImageUrl=@ImageUrl WHERE Id=@Id";
            
            using var command = new NpgsqlCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Description", product.Description);
            command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);
            result = await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        finally
        {
            connection?.Dispose();
        }
        return Ok(result);
    }
}
