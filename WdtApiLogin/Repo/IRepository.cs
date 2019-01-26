using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

namespace WdtApiLogin.Repo
{

    /// <summary>
    /// implementing repository pattern as per M Fowler's book "Patterns of Enterprise Application Architecture"
    /// Overriding and improving in Async Nature of REST Api
    /// Also:
    /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetAsync(string path);

        Task<T> FindAsync(Func<T, bool> filter);

        Task<Uri> AddAsync(T entity);

        void RemoveAsync(T entity);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly string EndPointUrl;
        protected readonly HttpClient HtClient;

        public Repository(string endPointUrl, HttpClient httpClient)
        {
            this.EndPointUrl = endPointUrl;
            this.HtClient = httpClient;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var response = await this.HtClient.GetAsync(this.EndPointUrl);
            response.EnsureSuccessStatusCode();

            var result = await response.Content
                             .ReadAsAsync<IEnumerable<T>>();

            return result;
        }

        public async Task<T> GetAsync(string path)
        {
            var response = await this.HtClient.GetAsync(this.EndPointUrl + @"/" + path);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<T>();

            return result;
        }

        public async Task<T> FindAsync(Func<T, bool> filter)
        {
            var response = await this.HtClient.GetAsync(this.EndPointUrl);
            response.EnsureSuccessStatusCode();

            var result = await response.Content
                             .ReadAsAsync<IEnumerable<T>>();

            var search = result.Where(filter).FirstOrDefault();

           return search;

        }

        public async Task<Uri> AddAsync(T entity)
        {
            var response = await this.HtClient.PostAsJsonAsync(this.EndPointUrl, entity);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        public void RemoveAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }

}
