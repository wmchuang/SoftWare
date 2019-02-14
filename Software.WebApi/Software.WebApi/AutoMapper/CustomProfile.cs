using AutoMapper;
using Software.Model.Models;
using Software.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Software.WebApi.AutoMapper
{
    public class CustomProfile : Profile
    {
        private readonly string _host = "https://www.xxx.com";

        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public CustomProfile()
        {
            CreateMap<Members, MemberViewModels>()
                //.ForMember(d => d.bCreateTime.ToString("yyyy-MM-dd"), o => o.MapFrom(s => s.bCreateTime));
                .AfterMap((src, dest) => dest.HeadImg = src.HeadImg.Contains("http") ? src.HeadImg : $"{_host}{src.HeadImg}");
        }
    }
}
