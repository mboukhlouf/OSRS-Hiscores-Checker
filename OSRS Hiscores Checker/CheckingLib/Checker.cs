using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace CheckingLib
{
    public class Checker : IDisposable
    {
        private bool disposed = false;
        private readonly HttpClientHandler ClientHandler;
        private readonly HttpClient Client;
        public readonly Variables Variables;

        public IWebProxy Proxy
        {
            get
            {
                return ClientHandler.Proxy;
            }
            set
            {
                ClientHandler.UseProxy = value != null;
                ClientHandler.Proxy = value;
            }
        }

        public CookieContainer Cookies
        {
            get
            {
                return ClientHandler.CookieContainer;
            }
            set
            {
                ClientHandler.CookieContainer = value;
            }
        }

        public Checker()
        {
            ClientHandler = new HttpClientHandler();
            Client = new HttpClient(ClientHandler);

            ClientHandler.UseDefaultCredentials = true;
            ClientHandler.UseCookies = true;
            ClientHandler.CookieContainer = new CookieContainer();
            ClientHandler.AllowAutoRedirect = false;
            Variables = new Variables();
        }

        public async Task<Response> ExecuteAsync(Step Step)
        {
            var Message = new HttpRequestMessage()
            {
                Method = new HttpMethod(Step.Method),
                RequestUri = new Uri(Variables.Transform(Step.Url)),
                Content = new StringContent(Variables.Transform(Step.PostData ?? ""))
            };

            foreach (KeyValuePair<String, String> header in Step.Headers)
            {
                Message.Headers.Add(header.Key, Variables.Transform(header.Value));
            }

            if (!String.IsNullOrEmpty(Step.ContentType))
                Message.Content.Headers.ContentType = new MediaTypeHeaderValue(Step.ContentType);

            using (HttpResponseMessage httpResponse = await Client.SendAsync(Message))
            using (HttpContent content = httpResponse.Content)
            {
                Dictionary<string, string[]> headers = new Dictionary<string, string[]>();

                // Adding response headers
                foreach(var header in httpResponse.Headers)
                {
                    headers.Add(header.Key, (string[])header.Value);
                }

                // Adding content headres
                foreach(var header in content.Headers)
                {
                    foreach(var value in header.Value)
                    {
                        headers.Add(header.Key, (string[])header.Value);
                    }
                
                }
                Response response = new Response()
                {
                    Version = httpResponse.Version.ToString(),
                    StatusCode = (int)httpResponse.StatusCode,
                    ReasonPhrase = httpResponse.ReasonPhrase,
                    Body = await content.ReadAsStringAsync(),
                    Headers = headers
                };
                Message.Dispose();
                return response;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                
            }
            Client.Dispose();
            ClientHandler.Dispose();
            disposed = true;
        }

        ~Checker()
        {
            Dispose(false);
        }
    }
}
