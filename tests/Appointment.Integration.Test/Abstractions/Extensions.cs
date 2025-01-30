using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace Application.Integration.Test.Abstractions
{
    public static class HttpResponseMessageExtension
    {
        public record BadRequestObject(HttpStatusCode ErrorCode, string Message);
        public static async Task<BadRequestObject> ToBadRequestObject(this HttpResponseMessage ex)
        {
            return await ex.Content.ReadFromJsonAsync<BadRequestObject>();
        }

        public static async Task<T> ToObject<T>(this HttpResponseMessage ex)
        {
            return await ex.Content.ReadFromJsonAsync<T>();
        }
    }
}
