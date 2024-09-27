using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using web_app_performance.Model;

namespace web_app_performance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private static ConnectionMultiplexer redis;

        [HttpGet]
        public async Task<IActionResult> GetUsuario()
        {
            string key = "getusuario";
            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10));
            string user = await db.StringGetAsync(key);

            if (!string.IsNullOrEmpty(user))
            {
                return Ok(user);
            }


            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string query = "SELECT id, Nome, Email FROM usuarios;";
            var usuarios = await connection.QueryAsync<Usuario>(query);

            string usuariosJson = JsonConvert.SerializeObject(usuarios);
            await db.StringSetAsync(key, usuariosJson);

            return Ok(usuarios);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)

        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string sql = "INSERT INTO usuarios( id ,Nome, Email) values (@nome , @email)";


            string key = "getusuario";

            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();



        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Usuario usuario)

        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string sql = "update usuarios set Nome = @nome,Email = @email,where id = @id";
            string key = "getusuario";

            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();



        }



        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)

        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = "@Delete from ususarios where id = @id";
            await connection.ExecuteAsync(sql, new { id });

            string key = "getusuario";

            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();



        }
    }

}