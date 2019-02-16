using System;
namespace Software.Model.Models
{	
	/// <summary>
	/// NLogInfo
	/// </summary>	
	public class NLogInfo//可以在这里加上基类等
	{

        /// <summary>
        ///    Id
        /// </summary>	
        public int Id { get; set; }
		
	  /// <summary>
	  ///    Date
	  /// </summary>	
	  public DateTime Date { get; set; }
		
	  /// <summary>
	  ///    Level
	  /// </summary>	
	  public string Level { get; set; }
		
	  /// <summary>
	  ///    Url
	  /// </summary>	
	  public string Url { get; set; }
		
	  /// <summary>
	  ///    Parameter
	  /// </summary>	
	  public string Parameter { get; set; }
		
	  /// <summary>
	  ///    Message
	  /// </summary>	
	  public string Message { get; set; }
 
	}
}
	