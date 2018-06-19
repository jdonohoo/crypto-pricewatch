using Newtonsoft.Json;
using SlackWebHooks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Handlers.Helpers
{
    public static class SlackHookHelper
    {
        private static string SlackChannel => AppConfig.Instance.GetParameter("SlackChannel");
        private static string SlackUser => AppConfig.Instance.GetParameter("SlackUser");
        private static string SlackWebHook => AppConfig.Instance.GetParameter("SlackWebHook");
        static SlackHookHelper()
        {
            Client = new WebHookClient(SlackWebHook);
        }

        private static WebHookClient Client { get; }

        public static void SendSlackNotification(string message)
        {
            var payload = new Message(
                message,
                channel: "#" + SlackChannel,
                username: SlackUser
            );

            var task = Client.SendMessageAsync(payload);
            task.Wait();
        }
        public static void SendSlackObjNotification(object message)
        {
            SendSlackNotification(JsonConvert.SerializeObject(message));
        }

        public const string AtChannel = "<!channel|@channel>";
        public const string AtHere = "<!here|@here>";

        public static class SlackColors
        {
            public static string Good => "good";
            public static string Warning => "warning";
            public static string Danger => "danger";
            public static string Info => "#5bc0de";
        }
    }
}
