using Software.IRepository;
using Software.IServices;
using Software.Model.Models;
using Software.Services.Base;
namespace Software.Services
{	
	/// <summary>
	/// MembersServices
	/// </summary>	
	public class MembersServices : BaseServices<Members>, IMembersServices
    {
	
        IMembersRepository dal;
        public MembersServices(IMembersRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }
    }
}

	