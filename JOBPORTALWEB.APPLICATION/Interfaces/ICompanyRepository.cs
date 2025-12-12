using JOBPORTALWEB.DOMAIN.Entities;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> AddCompanyAsync(Company company);
        Task<Company?> GetCompanyByIdAsync(int id);
    }
}