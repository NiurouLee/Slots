// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : Rules
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace GameModule
{
	public class Rules
	{   
    	// 分层名称; 自定义，仅说明描述意义，无逻辑使用意义
    	public string groupName { get; set; }// 分层ID; ①用于关联功能数值配置中的USERGROUP列，重要！重要！重要！; ②1-5已被占用，可自定义其他数字
    	public int groupId { get; set; }// 字段名称; ①USERGROUP已占用，表示FIREBASE预测字段; ②其余字段名称可在【用户画像】字段内选择; 用户画像标签字段文档--数据平台; ③不可使用其他不可识别的字段
    	public string dataKey { get; set; }// 条件判断运算符; ①EQ: 等于，NEQ: 不等于; ②GT: 大于，GTE: 大于等于; ③LT: 小于，LTE: 小于等于; ④IN: 存在于DATAVALUE数组内，NIN: 不存在于DATAVALUE数组内
    	public string Operator { get; set; }// 条件值数据类型; ①若为ARRAYSTRING、ARRAYNUMBER或ARRAYTYPECODE 类型，则条件值以英文逗号“,”分隔; ②若为TIME类型，则条件值填写UTC0时区的时间字串; ③NUMBER表示整数，不支持小数; ④DAYS表示距离某个时间点多少天，字段名称必须为时间点类型的字段; ⑤SUM表示求和运算，DATAKEY必须为COLUMNA+COLUMNB的格式; ⑥MINUS表示求差运算，DATAKEY必须为COLUMNA-COLUMNB的格式; ⑦PRODUCT表示求乘积运算，DATAKEY必须为COLUMNA*COLUMNB的格式; ⑧DIVIDE表示求商运算，DATAKEY必须为COLUMNA/COLUMNB的格式; ⑨TYPECODE 表示枚举值
    	public string dataType { get; set; }// 条件值
    	public string dataValue { get; set; }// 描述
    	public string desc { get; set; }
	}
}
