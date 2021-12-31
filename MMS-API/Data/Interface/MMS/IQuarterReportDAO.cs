using System.Collections.Generic;
using API.Models.MMS;

namespace API.Data.Interface.MMS
{
    public interface IQuarterReportDAO : ICommonDAO<QuarterReport>
    {
            public List<QuarterReport> GetTop4Eps();
    }
    
}