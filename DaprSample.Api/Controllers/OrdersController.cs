using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DaprSample.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DaprSample.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private string _stateUrl = "http://localhost:3500/v1.0/state/statestore";
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var client = new HttpClient();
            
            var result = await client.GetAsync($"{_stateUrl}/order");

            result.EnsureSuccessStatusCode();
            var resp = await result.Content.ReadAsStringAsync();

            var order = JsonSerializer.Deserialize<Order>(resp);

            return Ok(order);
        }

        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = new List<Object>()
            {
                new { Key = "order" , Value = dto }
            };

            var client = new HttpClient();

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, @"application/json");

            var result = await client.PostAsync(_stateUrl, content);

            if(!result.IsSuccessStatusCode)
            {
                throw new Exception(await result.Content.ReadAsStringAsync());
            }

            return CreatedAtRoute("GetOrder", new { id = dto.Id }, dto);
        }

    }
}