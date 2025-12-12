using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.DOMAIN.Enums
{
    public enum ApplicationStatus
    {
        Applied = 0,       // Mới nộp
        Shortlisted = 1,   // Đã sơ tuyển
        Interviewing = 2,  // Phỏng vấn
        Rejected = 3,      // Từ chối
        Hired = 4          // Đã tuyển
    }
}