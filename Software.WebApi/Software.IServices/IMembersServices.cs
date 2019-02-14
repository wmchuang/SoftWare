using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Software.IServices.Base;
using Software.Model.Models;
namespace Software.IServices
{	
	/// <summary>
	/// MembersServices
	/// </summary>	
    public interface IMembersServices :IBaseServices<Members>
	{
        Task<Members> Login(string openId, string session_key, string headurl, string nickname, string sex, string province, string city);

        Task<List<Members>> List();
    }
}

	