using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Software.IServices;
using Software.Model.Models;
using Software.Model.ParmModels;
using Software.Model.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Software.Admin.Controllers
{
    [Authorize]
    public class NoticeController : Controller
    {
        private readonly INoticeServices _noticeServices;
        private IHostingEnvironment hostingEnv;

        public NoticeController(IHostingEnvironment env, INoticeServices noticeServices)
        {
            this.hostingEnv = env;
            _noticeServices = noticeServices;
        }

        public IActionResult Index()
        {
            return View();
        }


        public ActionResult LoadData([FromQuery]PageModel model)
        {
            List<Notice> list = new List<Notice>();
            int totalCount = 0;
            Expression<Func<Notice, bool>> whereExpression = null;

            if (!string.IsNullOrWhiteSpace(model.Key))
            {
                whereExpression = a => a.Title.Contains(model.Key);
            }
            list = _noticeServices.QueryPage(ref totalCount, whereExpression, model.Page, model.Limit, "Id Desc");
            var result = new TableDataModel
            {
                count = totalCount,
                data = list
            };
            return Json(result);
        }

        [HttpGet]
        public IActionResult AddOrModify()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<Result<string>> AddOrModify([FromForm]Notice item)
        {
            var result = new Result<string>();
            Notice notice = new Notice();

            bool flag = true;
            if (item.Id > 0)
            {
                flag = await _noticeServices.Update(item, new List<string>() { "Title", "Content" });
            }
            else
            {
                int id = 0;
                item.CreateTime = DateTime.Now;
                id = await _noticeServices.Add(item);
                if (id == 0) flag = false;
            }

            if (!flag)
            {
                result.status = false;
                result.message = "提交失败！";
            }
            return result;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<Result<string>> Delete(int[] Id)
        {
            var result = new Result<string>();

            var flag = await _noticeServices.DeleteByIds(Id);
            if (!flag)
            {
                result.status = false;
                result.message = "删除失败";
            }
            return result;
        }

        /// layui在线编辑器里的上传图片功能
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadImage()
        {
            #region 文件上传
            var imgFile = Request.Form.Files[0];
            if (imgFile != null && !string.IsNullOrEmpty(imgFile.FileName))
            {
                long size = 0;
                string tempname = "";
                var filename = ContentDispositionHeaderValue
                                .Parse(imgFile.ContentDisposition)
                                .FileName
                                .Trim('"');
                var extname = filename.Substring(filename.LastIndexOf("."), filename.Length - filename.LastIndexOf("."));
                var filename1 = System.Guid.NewGuid().ToString().Substring(0, 6) + extname;
                tempname = filename1;
                var path = hostingEnv.WebRootPath;
                string dir = DateTime.Now.ToString("yyyyMMdd");
                if (!System.IO.Directory.Exists(hostingEnv.WebRootPath + $@"/upload/{dir}"))
                {
                    System.IO.Directory.CreateDirectory(hostingEnv.WebRootPath + $@"/upload/{dir}");
                }
                filename = hostingEnv.WebRootPath + $@"/upload/{dir}/{filename1}";
                size += imgFile.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    imgFile.CopyTo(fs);
                    fs.Flush();
                }

                var url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                return Json(new { code = 0, msg = "上传成功", data = new { src = $"{url}/upload/{dir}/{filename1}", title = "图片标题" } });
            }
            return Json(new { code = 1, msg = "上传失败", });
            #endregion
        }
    }
}
