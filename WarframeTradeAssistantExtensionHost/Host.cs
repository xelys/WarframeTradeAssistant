namespace WarframeTradeAssistantExtensionHost
{
    using System;
    using System.Dynamic;
    using System.IO;
    using System.Text;
    using Common;
    using Newtonsoft.Json;

    internal static class Host
    {
        public static void ProcessMessages()
        {
            try
            {
                var input = Receive();
                Storage.SaveToken(input.token);
                dynamic response = new ExpandoObject();
                response.success = true;
                Send(response);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                dynamic response = new ExpandoObject();
                response.success = false;
                Send(response);
            }
        }

        private static dynamic Receive()
        {
            using (Stream stdin = Console.OpenStandardInput())
            {
                var lengthBytes = new byte[4];
                stdin.Read(lengthBytes, 0, 4);
                var length = (int)BitConverter.ToUInt32(lengthBytes, 0);
                var inputBytes = new byte[length];
                stdin.Read(inputBytes, 0, length);
                var input = Encoding.UTF8.GetString(inputBytes);
                return JsonConvert.DeserializeObject<ExpandoObject>(input);
            }
        }

        private static void Send(object message)
        {
            using (Stream stdout = Console.OpenStandardOutput())
            {
                string output = JsonConvert.SerializeObject(message);
                var outputBytes = Encoding.UTF8.GetBytes(output);
                var lengthBytes = BitConverter.GetBytes((uint)outputBytes.Length);
                stdout.Write(lengthBytes, 0, lengthBytes.Length);
                stdout.Write(outputBytes, 0, outputBytes.Length);
            }
        }
    }
}