using Software.IRepository;
using Software.IServices;
using Software.Model.Models;
using Software.Services.Base;
namespace Software.Services
{	
	/// <summary>
	/// ManageServices
	/// </summary>	
	public class ManageServices : BaseServices<Manage>, IManageServices
    {
	
        IManageRepository dal;
        public ManageServices(IManageRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }
    }
}

	