using MediatR;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.APPLICATION.DTOs.Company;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace JOBPORTALWEB.APPLICATION.Features.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommand : IRequest<int>
    {
        public CreateCompanyRequest DTO { get; set; }
        public Guid RecruiterId { get; set; } // Người tạo công ty

        public CreateCompanyCommand(CreateCompanyRequest dto, Guid recruiterId)
        {
            DTO = dto;
            RecruiterId = recruiterId;
        }
    }

    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, int>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly UserManager<User> _userManager;
        private readonly IJobRepository _jobRepository; 

        public CreateCompanyCommandHandler(
            ICompanyRepository companyRepository,
            UserManager<User> userManager,
            IJobRepository jobRepository)  
        {
            _companyRepository = companyRepository;
            _userManager = userManager;
            _jobRepository = jobRepository;    
        }

        public async Task<int> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            // 1. Tạo Company mới
            var company = new Company
            {
                Name = request.DTO.Name,
                Website = request.DTO.Website,
                Description = request.DTO.Description,
                Location = request.DTO.Location
            };

            var newCompany = await _companyRepository.AddCompanyAsync(company);

            // 2. Gán CompanyId cho User (Recruiter)
            var user = await _userManager.FindByIdAsync(request.RecruiterId.ToString());
            if (user != null)
            {
                user.CompanyId = newCompany.Id;
                await _userManager.UpdateAsync(user);

                // 3. ✅ Cập nhật toàn bộ JOB cũ của recruiter này
                await _jobRepository.UpdateCompanyForRecruiterJobsAsync(user.Id, newCompany.Id);
            }

            return newCompany.Id;
        }
    }
}
