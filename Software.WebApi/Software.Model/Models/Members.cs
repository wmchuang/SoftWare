using System;
namespace Software.Model.Models
{	
	/// <summary>
	/// Members
	/// </summary>	
	public class Members//可以在这里加上基类等
	{
		
	  /// <summary>
	  ///    Id
	  /// </summary>	
	  public int Id { get; set; }
		
	  /// <summary>
	  /// 微信中用户唯一标识
	  /// </summary>	
	  public string OpenId { get; set; }
		
	  /// <summary>
	  /// 会话密钥
	  /// </summary>	
	  public string Session_key { get; set; }
		
	  /// <summary>
	  /// 昵称
	  /// </summary>	
	  public string NickName { get; set; }
		
	  /// <summary>
	  /// 头像
	  /// </summary>	
	  public string HeadImg { get; set; }
		
	  /// <summary>
	  /// 性别；0：男 1:女
	  /// </summary>	
	  public int? Sex { get; set; }
		
	  /// <summary>
	  /// 状态（2：正常）
	  /// </summary>	
	  public int Status { get; set; }
		
	  /// <summary>
	  ///    Province
	  /// </summary>	
	  public string Province { get; set; }
		
	  /// <summary>
	  ///    City
	  /// </summary>	
	  public string City { get; set; }
		
	  /// <summary>
	  /// 创建时间
	  /// </summary>	
	  public DateTime CreateTime { get; set; }
 
	}
}
	