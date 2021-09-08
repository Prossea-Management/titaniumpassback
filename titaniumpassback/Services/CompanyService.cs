using titaniumpassback.DataAccess;
using titaniumpassback.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace titaniumpassback.Services
{
    public class CompanyService
    {
        private readonly ApplicationDbContext _context;

        public CompanyService(ApplicationDbContext context)
        {
            _context = context;
        }
        public Company GetCompany(int companyID)
        {
            return _context.Company.Find(companyID);
        }
        public bool CompanyExist(string companyName)
        {
            if (_context.Company.Any(u => u.Name == companyName))
            {
                return true;
            }
            return false;
        }
        public Company CreateCompany(Company company)
        {
            _context.Company.Add(company);
            Save();
            return company;
        }
        public bool Save()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }
    }
}
