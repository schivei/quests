using System;
using System.Net.Http;

namespace Quests.App.Services
{
    public class ServiceException : Exception
    {
        public ServiceException(HttpResponseMessage httpResponse, Exception innerException) : base(GetMessage(httpResponse), innerException)
        {
        }

        private static string GetMessage(HttpResponseMessage httpResponse) =>
            httpResponse.Content.ReadAsStringAsync()
                .ConfigureAwait(false).GetAwaiter().GetResult();
    }
}
