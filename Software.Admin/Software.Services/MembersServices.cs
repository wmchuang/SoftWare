using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Software.Common;
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
        private readonly IMembersRepository _membersRepository;
        public MembersServices(IMembersRepository dal)
        {
            _membersRepository = dal;
            base.baseDal = dal;
        }

        [Caching(AbsoluteExpiration = 10)]//增加特性
        public async Task<List<Members>> List()
        {
            var list = (await _membersRepository.Query(a => true));
            return list;
        }

        public async Task<Members> Login(string openId, string session_key, string headurl, string nickname, string sex, string province, string city)
        {
            var member = (await _membersRepository.Query(a => a.OpenId == openId)).FirstOrDefault();
            if (member == null)
            {
                //新建用户
                member = new Members()
                {
                    OpenId = openId,
                    Session_key = session_key,
                    Status = 2,
                    HeadImg = headurl,
                    NickName = nickname,
                    Sex = sex == "男" ? 0 : 1,
                    Province = province,
                    City = city,
                    CreateTime = DateTime.Now
                };

                int id = await _membersRepository.Add(member);
                member.Id = id;
            }
            else
            {
                //修改用户信息
                if (!string.IsNullOrWhiteSpace(headurl))
                {
                    member.HeadImg = headurl;
                    member.NickName = nickname;
                    await _membersRepository.Update(member, new List<string>() { "HeadImg", "NickName" });
                }
            }
            return member;
        }
    }
}

	