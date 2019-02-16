using Software.IRepository;
using Software.IServices;
using Software.Model.Models;
using Software.Services.Base;
namespace Software.Services
{	
	/// <summary>
	/// NoticeServices
	/// </summary>	
	public class NoticeServices : BaseServices<Notice>, INoticeServices
    {
	
        INoticeRepository dal;
        public NoticeServices(INoticeRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }
    }
}

	