using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;

namespace NHub.Services
{
    /// <summary>
    /// Base service. Provides singlton pattern approach, and inherited functionality for making http requests.
    /// </summary>
    public abstract class BaseService
    {
        protected HttpClient client;

        protected virtual string BaseAddress => string.Empty;

        protected BaseService(Action<HttpClient> httpClientModifier = null)
        {
            client = new HttpClient();
            httpClientModifier?.Invoke(this.client as HttpClient);
        }

        internal enum RequestType
        {
            Delete,
            Get,
            Post,
            Put
        }

        protected Task<T> DeleteAsync<T>(string requestUri)
        {
            return SendWithRetryAsync<T>(RequestType.Delete, requestUri);
        }

        protected Task<T> PutAsync<T, K>(string requestUri, K obj) // where object
        {
            var jsonRequest = !obj.Equals(default(K)) ? JsonConvert.SerializeObject(obj) : null;
            return SendWithRetryAsync<T>(RequestType.Put, requestUri, jsonRequest);
        }

        async Task<T> SendWithRetryAsync<T>(RequestType requestType, string requestUri, string jsonRequest = null)
        {
            T result = default(T);

            result = await Policy
                .Handle<WebException>()
                .WaitAndRetryAsync(5, retryAttempt =>
                                   TimeSpan.FromMilliseconds((200 * retryAttempt)),
                    (exception, timeSpan, context) =>
                    {
                        Debug.WriteLine(exception.ToString());
                    }
                )
                .ExecuteAsync(async () => { return await SendAsync<T>(requestType, requestUri, jsonRequest); });

            return result;
        }

        async Task<T> SendAsync<T>(RequestType requestType, string requestUri, string jsonRequest = null)
        {
            T result = default(T);

            HttpContent content = null;

            if (jsonRequest != null)
                content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            if (client.BaseAddress == null)
                client.BaseAddress = new Uri(BaseAddress);

            Task<HttpResponseMessage> httpTask;

            switch (requestType)
            {
                case RequestType.Delete:
                    httpTask = client.DeleteAsync(requestUri);
                    break;
                case RequestType.Put:
                    httpTask = client.PutAsync(requestUri, content);
                    break;
                default:
                    throw new Exception("Not a valid request type");
            }

            var response = await httpTask.ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                return result;

            string json = string.Empty;

            if (response != null)
                json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!string.IsNullOrEmpty(json))
                result = JsonConvert.DeserializeObject<T>(json);

            return result;
        }
    }
}