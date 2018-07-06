using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DotnetTestAbstractions.Fixtures
{
    public class JsonServerFixture<TStartup, TContext>
        : TestServerFixture<TStartup, TContext>
        where TStartup : class
        where TContext : DbContext
    {
        protected async Task<List<T>> GetAllAsync<T>(string requestUri, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            return await GetAsync<List<T>>(requestUri, expectedStatusCode);
        }

        protected async Task<T> GetAsync<T>(string requestUri, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var scope = CreateRequestScope();
            var response = await Client.GetAsync(requestUri);
            scope.ActuallyDispose();

            if (response.StatusCode != expectedStatusCode)
                throw new DotnetTestAbstractionsFixtureException($"{nameof(GetAsync)} received '{response.StatusCode}' but expected '{expectedStatusCode}'");

            var body = await response.Content.ReadAsStringAsync();
            if (body == null)
                return default(T);

            return Deserialize<T>(body);
        }

        protected async Task<T> PostAsync<T>(string requestUri, T resource, HttpStatusCode expectedStatusCode = HttpStatusCode.Created)
        {
            var json = JsonConvert.SerializeObject(resource);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var scope = CreateRequestScope();
            var response = await Client.PostAsync(requestUri, content);
            scope.ActuallyDispose();

            if (response.StatusCode != expectedStatusCode)
                throw new DotnetTestAbstractionsFixtureException($"{nameof(GetAsync)} received '{response.StatusCode}' but expected '{expectedStatusCode}'");

            var body = await response.Content.ReadAsStringAsync();
            if (body == null)
                return default(T);

            return Deserialize<T>(body);
        }

        private T Deserialize<T>(string body)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch (Exception e)
            {
                throw new DotnetTestAbstractionsFixtureException($"Failed to deserialize body '{body}' to type '{typeof(T)}'", e);
            }
        }
    }
}
