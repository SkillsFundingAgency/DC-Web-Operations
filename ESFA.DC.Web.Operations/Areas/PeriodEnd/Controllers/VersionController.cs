using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Models.ServiceFabric;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Common;
using Newtonsoft.Json;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Controllers
{
    [Area(AreaNames.PeriodEnd)]
    [Route(AreaNames.PeriodEnd + "/periodEndVersion")]
    public class VersionController : Controller
    {
        private readonly ILogger _logger;
        private readonly JobQueueApiSettings _apiSettings;

        public VersionController(
            ILogger logger,
            JobQueueApiSettings apiSettings)
        {
            _logger = logger;
            _apiSettings = apiSettings;
        }

        public IActionResult Index()
        {
            RootObject root;
            try
            {
                var data = GetData($"{_apiSettings.ServiceFabricUrl}/Applications?api-version=6.4");
                root = JsonConvert.DeserializeObject<RootObject>(data);
                root.NodeItems = GetNodes();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

            return View(root);
        }

        private List<NodeInfo> GetNodes()
        {
            var data = GetData($"{_apiSettings.ServiceFabricUrl}/Nodes?api-version=6.4");
            var items = JsonConvert.DeserializeObject<NodeInfoList>(data);

            return items.Items.ToList();
        }

        private string GetData(string url)
        {
            string result;
            using (var cert = new X509Certificate2(Convert.FromBase64String(_apiSettings.Certificate)))
            {
                var request = WebRequest.Create(url) as HttpWebRequest;

                if (request == null)
                {
                    return null;
                }

                request.ContentType = "application/json;charset=UTF8";
                request.ClientCertificates.Add(cert);
                request.PreAuthenticate = true;
                request.ServerCertificateValidationCallback = ServerCertificateValidationCallback;

                using (var responseStream = request.GetResponse().GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        return null;
                    }

                    using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }

            return result;
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                _logger.LogError(sslPolicyErrors.ToString());
            }

            return true;
        }
    }
}