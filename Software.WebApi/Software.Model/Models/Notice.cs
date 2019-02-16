using System;
namespace Software.Model.Models
{	
	/// <summary>
	/// Notice
	/// </summary>	
	public class Notice//可以在这里加上基类等
	{
		
	  /// <summary>
	  /// 主键
	  /// </summary>	
	  public int Id { get; set; }
		
	  /// <summary>
	  /// 标题
	  /// </summary>	
	  public string Title { get; set; }
		
	  /// <summary>
	  /// 内容
	  /// </summary>	
	  public string Content { get; set; }
		
	  /// <summary>
	  /// 状态
	  /// </summary>	
	  public int Status { get; set; }
		
	  /// <summary>
	  /// 创建时间
	  /// </summary>	
	  public DateTime CreateTime { get; set; }
 
	}
}
	