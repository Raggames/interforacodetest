using System.Collections.Generic;
using System.Net.Http;

namespace Atomix.Backend
{
    /// <summary>
    /// Classe basse pour la gestion d'URL, peut servir à gérer plusieurs environnements en switchant l'url de base (prod, dev, preprod...)
    /// Dans ce cas on n'en utilise qu'une seule
    /// </summary>
    public static class BackendCore
    {
        private const string _devServer = "https://atomix.games/backendserver/v2/";

        public static string CreateURL(string apiHook, Dictionary<string, string> queryString = null)
        {
            string url = _devServer;

            url += apiHook + ".php";

            if (queryString != null)
            {
                using (var content = new FormUrlEncodedContent(queryString))
                {
                    url += "?" + content.ReadAsStringAsync().Result;
                }
            }

            return url;
        }
    }
}
