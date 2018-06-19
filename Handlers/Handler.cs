using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Handlers.Helpers;
using Handlers.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Handlers
{
    public class Handler
    {
        public Response PriceCheck(Request request, ILambdaContext context)
        {
            var message = new StringBuilder();
            context.Logger.LogLine($"{context.FunctionName} execution started");
            context.Logger.LogLine(context.LogGroupName);
            context.Logger.LogLine(context.LogStreamName);

            var coins = AppConfig.Instance.GetParameter("CoinsToWatch").Split(',');
            var priceList = new Dictionary<string, double>();

            foreach(var c in coins)
            {
                priceList.Add(c, CryptoCompare.GetCurrentCryptoPrice(c));
            }

            message.AppendLine("```");
            message.AppendLine("=== Crypto-PriceWatch ===");
            foreach (var p in priceList)
            {
                message.AppendLine($"{p.Key} : {p.Value}");
            }
            message.AppendLine("=========================");
            message.AppendLine("```");

            SlackHookHelper.SendSlackNotification(message.ToString());


            return new Response { Message = message.ToString() };
        }

        public APIGatewayProxyResponse HealthCheck(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine($"{context.FunctionName} execution started");

            return new APIGatewayProxyResponse()
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string>() { { "Context-Type", "text/html" } },
                Body = "OK"
            };
        }

    }
}
