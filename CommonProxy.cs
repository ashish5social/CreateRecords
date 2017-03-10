using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace odatatest
{
    public class CommonProxy
    {
        public const string servername = "crmpotassium";
        public const string orgname = "EnglishOrg";
        public const string domain = "crmpotassiumdom";
        public const string username = "administrator";
        public const string password = "T!T@n1130";

        public static HttpClient odataClient;

        public CommonProxy()
        {
            odataClient = GetHttpClient();
        }
        public HttpClient GetHttpClient()
        {
            var clientHandler = new HttpClientHandler();
            //TODO: Add support for Online environments as per test config
            clientHandler.Credentials = new System.Net.NetworkCredential(username, password, domain);
            clientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return new HttpClient(clientHandler);
        }
        public HttpResponseMessage executeSyncHelper(HttpMethod method, string relativeUrl, Dictionary<string, string> requestHeader, string body)
        {
            if (String.IsNullOrEmpty(method.ToString()))
            {
                throw new InvalidOperationException("HttpMethod cannot be null");
            }

            //http://crmpotassium/EnglishOrg/api/data/v9.0/ 
            string url = String.Format("http://{0}/{1}/api/data/v9.0/", servername, orgname) + relativeUrl;
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, new Uri(url));
            //Adding header info in the request
            if (requestHeader != null && requestHeader.Count > 0)
            {
                foreach (KeyValuePair<string, string> header in requestHeader)
                {
                    if (!header.Key.Contains("Content-Type"))
                    {
                        requestMessage.Headers.Add(header.Key, header.Value);
                    }
                }
            }
            //Adding body to the request message
            if (!String.IsNullOrEmpty(body))
            {
                if (relativeUrl.Contains("$batch"))
                {
                    requestMessage.Content = new StringContent(body);
                    if (requestMessage.Content.Headers.Contains("Content-Type"))
                        requestMessage.Content.Headers.Remove("Content-Type");
                    requestMessage.Content.Headers.TryAddWithoutValidation("Content-Type", requestHeader["Content-Type"]);
                }
                else
                {
                    requestMessage.Content = new StringContent(body,
                            Encoding.UTF8,
                            "application/json");
                }
            }

            //executing it in sync mode (Note: Task.Result enforces the execution in sync)
            HttpResponseMessage resp = odataClient.SendAsync(requestMessage).Result;
            return resp;
        }

        public class contactjson
        {
            public string lastname;
        }
        public class accountjson
        {
            public string name;
            public contactjson[] contact_customer_accounts;
        }
        public class accountonlyjson
        {
            public string name;
        }
        public void CreateAccountsWithContacts(int countAcc, int countCont, string key)
        {
            for (int i = 0; i < countAcc; i++)
            {
                accountjson act = new accountjson();
                act.name = "act" + DateTime.UtcNow.Ticks + "_" + key;
                act.contact_customer_accounts = new contactjson[countCont];
                for (int j = 0; j < countCont; j++)
                {
                    act.contact_customer_accounts[j] = new contactjson { lastname = "cont" + DateTime.UtcNow.Ticks + "_" + key };
                }

                executeSyncHelper(System.Net.Http.HttpMethod.Post, "accounts", null, JsonConvert.SerializeObject(act));
            }
        }

        public void CreateAccounts(int countAcc, string key)
        {
            for (int i = 0; i < countAcc; i++)
            {
                accountonlyjson act = new accountonlyjson();
                act.name = "Ask" + DateTime.UtcNow.Ticks + "_" + key;
                executeSyncHelper(System.Net.Http.HttpMethod.Post, "accounts", null, JsonConvert.SerializeObject(act));
                Console.WriteLine("Account {0} is created in {1}", act.name, servername); 
            }
        }
    }
}
