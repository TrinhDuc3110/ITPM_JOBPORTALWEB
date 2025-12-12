using JOBPORTALWEB.DOMAIN.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Interfaces
{
    public interface IAuthService
    {
        string CreateToken(User user, IList<string> roles);
    }
}
