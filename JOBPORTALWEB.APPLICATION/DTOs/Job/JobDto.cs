using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.DTOs.Job
{
    public class JobDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string Location { get; set; }
        public string Salary { get; set; }
        public string Type { get; set; }
        public bool IsRemote { get; set; }
        public List<string> Tags { get; set; }
        public DateTime PostedDate { get; set; }
    }
}