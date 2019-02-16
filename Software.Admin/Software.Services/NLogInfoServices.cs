using Software.IRepository;
using Software.IServices;
using Software.Model.Models;
using Software.Services.Base;
namespace Software.Services
{	
	/// <summary>
	/// NLogInfoServices
	/// </summary>	
	public class NLogInfoServices : BaseServices<NLogInfo>, INLogInfoServices
    {
	
        INLogInfoRepository dal;
        public NLogInfoServices(INLogInfoRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }
    }
}

	