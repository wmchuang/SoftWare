using System;
using Software.Model.Models;
using Software.Repository.Base;
using Software.IRepository;
namespace Software.Repository
{	
	/// <summary>
	/// ManageRepository
	/// </summary>	
	public class ManageRepository : BaseRepository<Manage>, IManageRepository
    {
        public async Task<TEntity> QueryByID(object objId)
        {
            return await Task.Run(() => db.Queryable<TEntity>().InSingle(objId));
        }
    }
}
	