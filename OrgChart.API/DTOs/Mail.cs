using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class Mail
    {
        public string Email { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
        public IEnumerable<File> Attachments { get; set; }
    }
}
