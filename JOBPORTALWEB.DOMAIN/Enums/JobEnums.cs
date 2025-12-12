using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.DOMAIN.Enums
{
    public enum JobType
    {
        FullTime = 0,
        PartTime = 1,
        Internship = 2,
        Contract = 3,
        Temporary = 4
    }

    public enum JobLevel
    {
        EntryLevel = 0,
        Junior = 1,
        MidLevel = 2,
        Senior = 3,
        Director = 4,
        VP = 5
    }

    public enum JobStatus
    {
        Active = 0,
        Expired = 1,
        Closed = 2,
        Draft = 3
    }

    public enum SalaryType
    {
        Monthly = 0,
        Yearly = 1,
        Hourly = 2
    }
}