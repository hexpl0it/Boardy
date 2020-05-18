using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BoardyClassLibrary.WebIntegration
{
    public interface IBoardyWebSample
    {
        public string Name { get; }
        public string Description { get; }
        public string Url { get; }
        public Stream Download();
    }
}
