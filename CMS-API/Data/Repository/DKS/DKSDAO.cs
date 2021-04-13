using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using API.Models.DKS;
using API.Data.Repository;
using API.Data.Interface;
using API.DTOs;
using Microsoft.Data.SqlClient;
using System;
using API.Helpers;

namespace API.Data.Repository
{
    public class DKSDAO : IDKSDAO
    {
        private readonly DKSContext _context;
        public DKSDAO(DKSContext context)
        {
            _context = context;
        }
        //依LOGIN查帳號
        public async Task<UserLog> SearchStaffByLOGIN(string login)
        {
            var staff = await _context.USER_LOG.FirstOrDefaultAsync(x => x.LOGINNAME == login.Trim());
            return staff;
        }

        //新增一筆user log 資料
        public async Task AddUserLogAsync(UserLog user)
        {
            _context.Add(user);
            await SaveAll();
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}