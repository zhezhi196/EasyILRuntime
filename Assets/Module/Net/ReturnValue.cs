/*
 * 脚本名称：HttpReturnValueBase
 * 脚本作者：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-03-13 12:58:00
 * 脚本作用：
*/

using System.Runtime.Serialization;

namespace Module
{
	[DataContract]
	public class ReturnValue<T> : ReturnValueBase<T> where T : ISqlData
	{
		public T data { get; set; }
	}
}