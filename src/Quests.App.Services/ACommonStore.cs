using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Quests.App.Services
{
    public abstract class ACommonStore
    {
        protected ACommonStore(HttpClient client) => Client = client;

        protected HttpClient Client { get; }

        private readonly JsonSerializerSettings _serializer = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        private T ParseContent<T>(string content) =>
            JsonConvert.DeserializeObject<T>(content, _serializer);

        protected async Task<T> GetAsync<T>(PathString endpoint)
        {
            var response = await Client.GetAsync(endpoint.Value);

            try
            {
                response = response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                throw new ServiceException(response, e);
            }

            var content = await response.Content.ReadAsStringAsync();

            return ParseContent<T>(content);
        }

        protected async Task<T> PostAsync<T>(PathString endpoint, object data)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(data, _serializer),
                Encoding.UTF8, "application/json");

            var response = await Client.PostAsync(endpoint.Value, httpContent);
            
            try
            {
                response = response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                throw new ServiceException(response, e);
            }

            var content = await response.Content.ReadAsStringAsync();

            return ParseContent<T>(content);
        }

        protected async Task DeleteAsync(PathString endpoint)
        {
            var response = await Client.DeleteAsync(endpoint.Value);
            
            try
            {
                response = response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                throw new ServiceException(response, e);
            }
        }
    }
}
