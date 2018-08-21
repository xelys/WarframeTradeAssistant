namespace WarframeTradeAssistant.Market
{
    using System;
    using System.Dynamic;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal sealed class ApiClient : IDisposable
    {
        private const string MarketURL = "https://warframe.market";

        private const string OrdersURL = "https://api.warframe.market/v1/profile/{0}/orders";

        private const string OrderURL = "https://api.warframe.market/v1/profile/orders/{0}";

        private const string ProfileURL = "https://api.warframe.market/v1/profile/{0}/orders?include=profile";

        private HttpClient client = new HttpClient(new HttpClientHandler { UseCookies = false });

        private string csrfToken;

        private string jwtToken;

        private string username;

        private ApiClient()
        {
        }

        public Statuses InitialStatus { get; private set; } = Statuses.invisible;

        public static async Task<ApiClient> Connect(string token)
        {
            var apiClient = new ApiClient();
            var info = await apiClient.GetAccountInfo(token);
            if (info.Name != null)
            {
                apiClient.jwtToken = token;
                apiClient.username = info.Name;
                apiClient.csrfToken = info.Token;
                apiClient.InitialStatus = await apiClient.GetUserStatus();
                return apiClient;
            }

            return null;
        }

        public void Dispose()
        {
            if (this.client != null)
            {
                this.client.Dispose();
            }

            this.client = null;
        }

        public async Task<Statuses> GetUserStatus()
        {
            try
            {
                var message = new HttpRequestMessage(HttpMethod.Get, string.Format(ProfileURL, this.username));
                message.Headers.Add("Cookie", "JWT=" + this.jwtToken);
                var result = await this.client.SendAsync(message);
                dynamic orders = JsonConvert.DeserializeObject<ExpandoObject>(await result.Content.ReadAsStringAsync());

                var statusString = orders.include.profile.status;
                if (statusString == "offline")
                {
                    return Statuses.invisible;
                }

                return (Statuses)Enum.Parse(typeof(Statuses), statusString);
            }
            catch (Exception ex)
            {
                Common.Logger.Write(ex.ToString());
                return Statuses.invisible;
            }
        }

        public async Task<bool> UpdateOrders()
        {
            try
            {
                var response = await this.client.GetStringAsync(string.Format(OrdersURL, this.username));
                dynamic orders = JsonConvert.DeserializeObject<ExpandoObject>(response);
                var sellOrders = orders.payload.sell_orders;
                var buyOrders = orders.payload.sell_orders;

                foreach (var order in sellOrders)
                {
                    if (order.visible)
                    {
                        await this.UpdateOrder(order);
                    }
                }

                foreach (var order in buyOrders)
                {
                    if (order.visible)
                    {
                        await this.UpdateOrder(order);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Common.Logger.Write(ex.ToString());
                return false;
            }
        }

        private static string GetCurrentUserJSON(string page)
        {
            var currentUserPos = page.IndexOf("current_user");
            if (currentUserPos == -1)
            {
                return null;
            }

            var counter = 0;
            var jsonStart = -1;
            var jsonEnd = -1;
            for (int i = currentUserPos; i < page.Length; i++)
            {
                if (page[i] == '{')
                {
                    if (counter == 0)
                    {
                        jsonStart = i;
                    }

                    counter++;
                }

                if (page[i] == '}')
                {
                    counter--;
                    if (counter == 0)
                    {
                        jsonEnd = i + 1;
                        break;
                    }
                }
            }

            return page.Substring(jsonStart, jsonEnd - jsonStart);
        }

        private async Task<AccountInfo> GetAccountInfo(string jwtToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, MarketURL);
            message.Headers.Add("Cookie", "JWT=" + jwtToken);
            var result = await this.client.SendAsync(message);
            var page = await result.Content.ReadAsStringAsync();
            dynamic currentUser = JsonConvert.DeserializeObject<ExpandoObject>(GetCurrentUserJSON(page));
            var name = currentUser.ingame_name;
            Regex r = new Regex(@"<\s*meta\s*name\s*=\s*""csrf-token""\s*content\s*=\s*""(.*?)""\s*>");
            var match = r.Match(page);
            var token = match != null && match.Groups.Count > 1 ? match.Groups[1].Value : null;
            return new AccountInfo(name, token);
        }

        private async Task<bool> UpdateOrder(dynamic order)
        {
            dynamic request = new ExpandoObject();
            if (order.mod_rank != null)
            {
                request.mod_rank = order.mod_rank;
            }

            request.order_id = order.id;
            request.platinum = order.platinum;
            request.quantity = order.quantity;
            request.visible = true;

            var message = new HttpRequestMessage(HttpMethod.Put, string.Format(OrderURL, order.id));
            message.Headers.Add("x-csrftoken", this.csrfToken);
            message.Headers.Add("Cookie", "JWT=" + this.jwtToken);
            message.Content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
            var result = await this.client.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        private struct AccountInfo
        {
            public AccountInfo(string name, string token) : this()
            {
                this.Name = name;
                this.Token = token;
            }

            public string Name { get; private set; }

            public string Token { get; private set; }
        }
    }
}
