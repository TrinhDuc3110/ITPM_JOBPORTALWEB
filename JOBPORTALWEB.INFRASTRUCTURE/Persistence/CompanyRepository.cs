using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.INFRASTRUCTURE.Data;
using System.Threading.Tasks;

namespace JOBPORTALWEB.INFRASTRUCTURE.Persistence
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company> AddCompanyAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await _context.Companies.FindAsync(id);
        }
    }
}