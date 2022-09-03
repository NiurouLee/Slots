// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : AdInterstitial
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace GameModule
{
	public class AdInterstitial
	{   
    	// #
    	public int id { get; set; }// # 广告位
    	public int placeId { get; set; }// # 分组GROUPID; 颜色越深越重要
    	public int userGroup { get; set; }// # 间隔时间(秒
    	public int showInterval { get; set; }// # 每日次数限制
    	public int limitPerDay { get; set; }
        public int limitLevel { get; set; }
	}
}
