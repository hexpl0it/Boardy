using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace BoardyClassLibrary.WebIntegration.Myinstants
{
    public class MyInstantsWebSample : IBoardyWebSample
    {
        public string Name { get; internal set; }

        public string Url { get; internal set; }

        public string Description { get; internal set; }

        public Stream Download()
        {
            return WebRequest.Create(Url)
                    .GetResponse().GetResponseStream();
        }
    }
}
