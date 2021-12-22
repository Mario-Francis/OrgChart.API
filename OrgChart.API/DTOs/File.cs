using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class File
    {
        public string FileName { get; set; }
        public byte[] FileBuffer { get; set; }
        public string ContentType { get; set; }
    }
}
