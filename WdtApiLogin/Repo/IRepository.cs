using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WdtApiLogin.Repo
{
    /// <summary>
    ///     implementing repository pattern as per M Fowler's book "Patterns of Enterprise Application Architecture"
    ///     Overriding and improving in Async Nature of REST Api
    ///     Also:
    ///     https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetAsync(string path);

        Task<T> FindAsync(Func<T, bool> filter);

        Task<IEnumerable<T>> FindAllAsync(Func<T, bool> filter);

        Task<Uri> AddAsync(T entity);

        Task<Uri> UpdateAsync(string path, T entity);
        
        Task<Uri> UpdateAsync(T entity);

        Task<T> RemoveAsync(string path);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly string EndPointUrl;
        protected readonly Lazy<HttpClient> HtClient;

        public Repository(string endPointUrl, Lazy<HttpClient> httpClient)
        {
            EndPointUrl = endPointUrl;
            HtClient = httpClient;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var response = await HtClient.Value.GetAsync(EndPointUrl);
            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadAsAsync<IEnumerable<T>>();

            return result;
        }

        public async Task<T> GetAsync(string path)
        {
            var response = await HtClient.Value.GetAsync(EndPointUrl + @"/" + path);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<T>();

            return result;
        }

        public async Task<T> FindAsync(Func<T, bool> filter)
        {
            var response = await HtClient.Value.GetAsync(EndPointUrl);
            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadAsAsync<IEnumerable<T>>();

            var search = result.Where(filter).FirstOrDefault();

            return search;
        }

        public async Task<IEnumerable<T>> FindAllAsync(Func<T, bool> filter)
        {
            var response = await HtClient.Value.GetAsync(EndPointUrl);
            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadAsAsync<IEnumerable<T>>();

            var search = result.Where(filter);

            return search;
        }

        public async Task<Uri> AddAsync(T entity)
        {
            var response = await HtClient.Value.PostAsJsonAsync(EndPointUrl, entity);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        public async Task<Uri> UpdateAsync(string path, T entity)
        {
            var response = await HtClient.Value.PutAsJsonAsync(EndPointUrl + @"/" + path, entity);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
        
        public async Task<Uri> UpdateAsync(T entity)
        {
            var response = await HtClient.Value.PutAsJsonAsync(EndPointUrl, entity);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        public async Task<T> RemoveAsync(string path)
        {
            var response = await HtClient.Value.DeleteAsync(EndPointUrl + @"/" + path);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<T>();

            return result;
        }
    }
}