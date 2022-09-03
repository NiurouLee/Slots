// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : AdReward
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace GameModule
{
	public class AdReward
	{   
    	// #
    	public int id { get; set; }// # 广告位
    	public int placeId { get; set; }// # 分组GROUPID
    	public int userGroup { get; set; }// # 间隔时间(秒
    	public int showInterval { get; set; }// # 每日次数限制
    	public int limitPerDay { get; set; }// # 奖励ID，见BONUS表
        
        public int effectiveLevel { get; set;} //广告开启等级限制，大于等于这个等级时开启
        
    	public List<int> bonus { get; set; }// # 参数
    	public int arg1 { get; set; }
    	public List<int> arg2 { get; set; }
	}
}
