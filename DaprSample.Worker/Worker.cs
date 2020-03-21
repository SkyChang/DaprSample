using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DaprSample.Worker.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace DaprSample.Worker
{
    public class Worker : BackgroundService
    {        
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Start Worker");

            var id = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                id++;
                var data = new Order(){ Id = id, Name = $"Hello Wolrd {id}" };

                var client = new HttpClient();

                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, @"application/json");

                var result = await client.PostAsync("http://localhost:3500/v1.0/invoke/api/method/api/orders", content);

                result.EnsureSuccessStatusCode();

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"Add Order id = {id}" );

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
