using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardyClassLibrary.WebIntegration.Myinstants
{
    public class MyInstantsWebSite : IBoardyWebSite
    {
        private string baseURL = "https://www.myinstants.com";
        public async Task<List<IBoardyWebSample>> Home()
        {
            List<IBoardyWebSample> result = new List<IBoardyWebSample>();
            var url = @"https://www.myinstants.com/index/it/";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            var nodes = doc.DocumentNode.SelectNodes("//div[@class='instant']");


            foreach (var x in nodes)
            {
                var nodeFile = x.SelectSingleNode(".//div[@class='small-button']");
                string _sampleUrl = nodeFile.Attributes["onmousedown"].Value.Replace("play('", "").Replace("')", "");
                _sampleUrl = baseURL + _sampleUrl;
                string _sampleDesc = x.SelectSingleNode(".//a[@class='instant-link']").InnerText;

                Uri uri = new Uri(_sampleUrl);
                string  _sampleName= System.IO.Path.GetFileName(uri.LocalPath);

                //string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                //foreach (char c in invalid)
                //{
                //    _sampleName = _sampleName.Replace(c.ToString(), "");
                //}

                result.Add(new MyInstantsWebSample() { Name = _sampleName,Description = _sampleDesc, Url = _sampleUrl });
            }

            return result;
        }

        public async Task<List<IBoardyWebSample>> SearchSample(string query)
        {
            List<IBoardyWebSample> result = new List<IBoardyWebSample>();
            var url = @"https://www.myinstants.com/search/?name=" + query.Replace(" ", "+");
            var web = new HtmlWeb();
            var doc = web.Load(url);

            try
            {
                var nodes = doc.DocumentNode.SelectNodes("//div[@class='instant']");


                foreach (var x in nodes)
                {
                    var nodeFile = x.SelectSingleNode(".//div[@class='small-button']");
                    string _sampleUrl = nodeFile.Attributes["onmousedown"].Value.Replace("play('", "").Replace("')", "");
                    _sampleUrl = baseURL + _sampleUrl;
                    string _sampleDesc = x.SelectSingleNode(".//a[@class='instant-link']").InnerText;

                    Uri uri = new Uri(_sampleUrl);
                    string _sampleName = System.IO.Path.GetFileName(uri.LocalPath);

                    //string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                    //foreach (char c in invalid)
                    //{
                    //    _sampleName = _sampleName.Replace(c.ToString(), "");
                    //}

                    result.Add(new MyInstantsWebSample() { Name = _sampleName, Description = _sampleDesc, Url = _sampleUrl });
                }
            }
            catch
            { }

            return result;
        }
    }
}
