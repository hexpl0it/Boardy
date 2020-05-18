using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BoardyClassLibrary.WebIntegration
{
    public interface IBoardyWebSite
    {
        public Task<List<IBoardyWebSample>> SearchSample(string query);

        public Task<List<IBoardyWebSample>> Home();
    }
}
