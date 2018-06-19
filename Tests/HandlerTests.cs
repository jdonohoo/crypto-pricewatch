using Microsoft.Extensions.Configuration;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Handlers;
using Handlers.Helpers;
using Handlers.Models;
using System;
using Xunit;

namespace Tests
{
    public class HandlerTests
    {
        public static string ServiceName => "crypto-pricewatch";

        [Fact]
        public void TestHealthCheck()
        {
            var request = new APIGatewayProxyRequest();
            var context = new TestLambdaContext();

            var handler = new Handler();
            var response = handler.HealthCheck(request, context);

            Assert.Equal(200, response.StatusCode);
            Assert.Equal("OK", response.Body);
        }

        [Fact]
        public void TestPriceCheck()
        {
            var context = new TestLambdaContext();
            var handler = new Handler();

            var response = handler.PriceCheck(new Request(), context);

            Assert.NotEmpty(response.Message);
        }
                
        [Theory]
        [InlineData("dev")]
        public void ConfigurationTest(string stage)
        {
            Environment.SetEnvironmentVariable("region", "us-east-1");
            Environment.SetEnvironmentVariable("serviceName", ServiceName);
            Environment.SetEnvironmentVariable("parameterPath", $"/{stage}/{ServiceName}/settings/");
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
             
            Assert.Equal(ServiceName, AppConfig.Instance.ServiceName);
            Assert.Equal($"/{stage}/{ServiceName}/settings/", AppConfig.Instance.ParameterPath);
            Assert.Equal("https://min-api.cryptocompare.com/data/", AppConfig.Instance.GetParameter("CoinPriceUrl"));
            Assert.NotEmpty(AppConfig.Instance.GetParameter("CoinsToWatch"));
        }
    }
}
