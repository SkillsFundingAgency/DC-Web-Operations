using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ESFA.DC.Web.Operations.Models.ServiceFabric;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Common;
using Newtonsoft.Json;

namespace ESFA.DC.Web.Operations.Controllers
{
    public class ServiceFabricController : Controller
    {
        private readonly JobQueueApiSettings _apiSettings;

        public ServiceFabricController(JobQueueApiSettings apiSettings)
        {
            _apiSettings = apiSettings;
        }

        public IActionResult Index()
        {
            var data = GetData($"{_apiSettings.ServiceFabricUrl}/Applications?api-version=6.4");
            RootObject root = JsonConvert.DeserializeObject<RootObject>(data);
            root.NodeItems = GetNodes();

            return View(root);
        }

        public List<NodeInfo> GetNodes()
        {
            var data = GetData($"{_apiSettings.ServiceFabricUrl}/Nodes?api-version=6.4");
            var items = JsonConvert.DeserializeObject<NodeInfoList>(data);

            //var result = new List<NodeType>();
            //foreach (var nodeInfo in items.Items)
            //{
            //    result.Add();
            //}

            return items.Items.ToList();
        }

        public string GetData(string url)
        {
            X509Certificate2 cert = new X509Certificate2(Convert.FromBase64String(_apiSettings.Certificate));

            // create HTTP web request with proper content type
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json;charset=UTF8";
            request.ClientCertificates.Add(cert);
            request.PreAuthenticate = true;
            request.ServerCertificateValidationCallback = ServerCertificateValidationCallback;

            string result;
            using (Stream responseStream = request.GetResponse().GetResponseStream())
            {
                var reader = new StreamReader(responseStream, Encoding.UTF8);
                result = reader.ReadToEnd();
            }

            return result;
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            Console.WriteLine(sslpolicyerrors);
            return true;
        }
    }
}