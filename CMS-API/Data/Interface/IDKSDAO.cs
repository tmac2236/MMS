using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.DKS;
using API.DTOs;
using API.Helpers;

namespace API.Data.Interface
{
    public interface IDKSDAO
    {
        Task<UserLog> SearchStaffByLOGIN(string login);
        Task AddUserLogAsync(UserLog user);
        Task<bool> SaveAll();
    }
}