using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoPC
{
    public class GetIPViewModel
    {
        public string IPv4Adress { get; set; }
        public GetIPViewModel(string ipv4)
        {
            IPv4Adress = ipv4;
        }
    }
}