using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Region.Utils
{
    /// <summary>
    /// 拼音工具
    /// </summary>
    public class PinYinUtils
    {
        /// <summary>
        /// 声母
        /// </summary>
        private static readonly string[,] INITIALS_LIST = new string[3, 2]
        {
            { "Z", "Zh" },
            { "C", "Ch" },
            { "S", "Sh" }
        };

        /// <summary>
        /// 韵母 Finals 
        /// </summary>
        private static readonly string[,] FINALS_LIST = new string[5, 2]
        {
            { "an", "ang" },
            { "en", "eng" },
            { "in", "ing" },
            { "An", "Ang" },
            { "En", "Eng" }
        };

        /// <summary>
        /// 声母加韵母
        /// </summary>
        private static readonly string[,] SPELL_LIST = new string[8, 2]
        {
            { "Z", "Zh" },
            { "C", "Ch" },
            { "S", "Sh" },
            { "an", "ang" },
            { "en", "eng" },
            { "in", "ing" },
            { "An", "Ang" },
            { "En", "Eng" }
        };

        /// <summary>
        /// 全拼字典-用于校正多音字城市
        /// </summary>
        private static readonly Dictionary<String, String> SPELL_DICTIONARY = new Dictionary<String, String>()
        {
            { "重庆", "ChongQing" },
            { "长春", "ChangChun" },
            { "长沙", "ChangSha" },
            { "石家庄", "ShiJiaZhuang" },

        };
        /// <summary>
        /// 简拼-用于校正多音字城市
        /// </summary>
        private static readonly Dictionary<String, String> SHORT_SPELL_DICTIONARY = new Dictionary<String, String>()
        {
            { "重庆", "CQ" },
            { "长春", "CC" },
            { "长沙", "CS" },
            { "石家庄", "SJZ" },
        };

        /// <summary>
        /// 创建混合索引 简拼与全拼混合体 如：北京 BJ;BJing;BeiJ;BJin;BeiJing;BeiJin
        /// </summary>
        /// <param name="chinese"></param>
        /// <returns></returns>
        public static string CreateHybridIndex(string chinese)
        {
            return CreateHybridIndex(ConvertToPinYin(chinese, true), ConvertToPinYin(chinese));
        }

        #region 汉字转化拼音

        /// <summary>
        /// 汉字转化为拼音 如：chinese=北京
        /// </summary>
        /// <param name="chinese"></param>
        /// <param name="isShortSpell">fasle为简拼（如：北京 BJ）/true为全拼（如北京：BeiJing）</param>
        /// <returns></returns>
        public static string ConvertToPinYin(string chinese, bool isShortSpell = false)
        {
            if (string.IsNullOrWhiteSpace(chinese))
            {
                return string.Empty;
            }
            if (SPELL_DICTIONARY.ContainsKey(chinese))
            {
                return isShortSpell ? SHORT_SPELL_DICTIONARY[chinese] : SPELL_DICTIONARY[chinese];
            }
            StringBuilder result = new StringBuilder();
            foreach (char chineseChar in chinese)
            {
                if (isChinese(chineseChar))//判断是否是中文
                {
                    ChineseChar pinYin = new ChineseChar(chineseChar);
                    result.Append(isShortSpell ? pinYin.Pinyins[0].Substring(0, 1) : Capitalize(pinYin.Pinyins[0]));
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// 是否是中文
        /// </summary>
        /// <param name="chinese">需检测的中文</param>
        /// <returns></returns>
        private static bool isChinese(string chinese)
        {
            if (string.IsNullOrWhiteSpace(chinese))
            {
                return false;
            }
            foreach (char chineseChar in chinese)
            {
                if (!isChinese(chineseChar))//判断是否是中文
                {
                    return false;
                }
            }
            return true;
        }
        private static bool isChinese(char chineseChar)
        {
            if (chineseChar >= 0x4e00 && chineseChar <= 0x9fbb)//判断是否是中文
            {
                return true;
            }
            return false;

        }

        /// <summary>
        /// 首字母变为大写
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="isShortSpell"></param>
        /// <returns></returns>
        private static string Capitalize(string spell, bool isShortSpell = false)
        {
            return isShortSpell ?
                spell.Substring(0, 1).ToUpper()
                : spell.Substring(0, 1).ToUpper() + spell.Substring(1, spell.Length - 2).ToLower();
        }


        #endregion

        #region 包含函数

        /// <summary>
        /// 中文名称匹配 类似 sql中 like 'abc%'
        /// </summary>
        /// <param name="value"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static bool IsChineseMatch(string value, string query)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            if (query.Length > value.Length)
            {
                return false;
            }
            int len = query.Length;
            return value.ToLower().Substring(0, len).Contains(query.ToLower());

        }
        /// <summary>
        /// 全拼匹配
        /// </summary>
        /// <param name="value"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static bool IsSpellMatch(string value, string query)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            if (IsSpellContains(value, query))
            {
                return true;
            }
            return IsSpellAppendContains(value, query);
        }

        /// <summary>
        /// 直接对比是否包含
        /// </summary>
        /// <param name="value"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static bool IsSpellContains(string value, string query)
        {
            if (query.Length > value.Length)
            {
                return false;
            }
            return value.Substring(0, query.Length).ToLower().Contains(query.ToLower());

        }

        /// <summary>
        /// 对比增量是否包含
        /// </summary>
        /// <param name="value"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static bool IsSpellAppendContains(string value, string query)
        {
            string queryAppend = Append(query, true).ToLower();
            string valueAppend = Append(value, true).ToLower();
            if (queryAppend.Length > valueAppend.Length)
            {
                return false;
            }
            return IsSpellContains(valueAppend, queryAppend);
        }
        #endregion

        /// <summary>
        /// 追加模糊匹配的全部增量(BeiJin->BeiJing)
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        public static string Append(string spell)
        {
            //for (int i = 0; i < 8; i++)
            //{
            //    spell = spell.Replace(spellList[i, 0], spellList[i, 1]);
            //}
            //spell = spell.Replace("hh", "h");
            //spell = spell.Replace("gg", "g");
            //return spell;
            return Append(spell, false);
        }

        /// <summary>
        /// 追加模糊匹配的全部增量并转换为小写(BeiJin->beijing)
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        private static string Append(string spell, bool isLower)
        {
            spell = isLower ? spell.ToLower() : spell;
            for (int i = 0; i < 8; i++)
            {
                spell = isLower ? spell.Replace(SPELL_LIST[i, 0].ToLower(), SPELL_LIST[i, 1].ToLower()) : spell.Replace(SPELL_LIST[i, 0], SPELL_LIST[i, 1]);
            }
            spell = spell.Replace("hh", "h");
            spell = spell.Replace("gg", "g");
            return spell;
        }

        /// <summary>
        /// 追加声母
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        private static string AppendInitials(string spell)
        {
            for (int i = 0; i < 3; i++)
            {
                spell = spell.Replace(INITIALS_LIST[i, 0], INITIALS_LIST[i, 1]);
            }
            spell = spell.Replace("hh", "h");
            return spell;
        }

        /// <summary>
        /// 追加韵母
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        private static string AppendFinals(string spell)
        {
            for (int i = 0; i < 5; i++)
            {
                spell = spell.Replace(FINALS_LIST[i, 0], FINALS_LIST[i, 1]);
            }
            spell = spell.Replace("gg", "g");
            return spell;
        }

        /// <summary>
        /// 去掉模糊匹配全部增量（beijing->beijin）
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        private static string Remove(string spell)
        {
            for (int i = 0; i < 8; i++)
            {
                spell = spell.Replace(SPELL_LIST[i, 1], SPELL_LIST[i, 0]);
            }
            return spell;
        }

        /// <summary>
        /// 去掉模糊匹配声母
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        private static string RemoveInitials(string spell)
        {
            for (int i = 0; i < 3; i++)
            {
                spell = spell.Replace(INITIALS_LIST[i, 1], INITIALS_LIST[i, 0]);
            }
            return spell;
        }

        /// <summary>
        /// 去掉模糊匹配韵母
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        private static string RemoveFinals(string spell)
        {
            for (int i = 0; i < 5; i++)
            {
                spell = spell.Replace(FINALS_LIST[i, 1], FINALS_LIST[i, 0]);
            }
            return spell;
        }


        /// <summary>
        /// 根据大小写分割拼音(BeiJing,分割为Bei Jing)
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        private static List<string> SplitSpell(string spell)
        {
            if (string.IsNullOrWhiteSpace(spell))
            {
                return null;
            }
            int length = spell.Length;
            List<string> list = new List<string>();
            string splitPY = null;
            for (int i = 0; i < length; i++)
            {
                if (char.IsUpper(spell, i))//大写
                {
                    if (splitPY != null)
                        list.Add(splitPY);
                    splitPY = null;//清空
                    splitPY += spell.Substring(i, 1);
                    if (i == length - 1)//如果是最后一个
                    {
                        list.Add(splitPY);
                    }
                }
                if (char.IsLower(spell, i))//小写
                {
                    splitPY += spell.Substring(i, 1);
                    if (i == length - 1)//如果是最后一个
                    {
                        list.Add(splitPY);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 创建所有混拼索引
        /// </summary>
        /// <param name="shortSpell"></param>
        /// <param name="spell"></param>
        /// <returns></returns>
        private static string CreateHybridIndex(string shortSpell, string spell)
        {
            List<List<string>> list = new List<List<string>>(); //第一层有多少个分割的拼音，第二层拼音
            list.Add(SplitSpell(shortSpell));                   //添加原始数据---简拼
            list.Add(SplitSpell(AppendInitials(shortSpell)));   //添加补全声母---简拼
            list.Add(SplitSpell(spell));                        //添加原始数据---全拼
            list.Add(SplitSpell(AppendInitials(spell)));        //添加补全声母---全拼
            list.Add(SplitSpell(Append(spell)));                //添加补全-------全拼
            list.Add(SplitSpell(AppendFinals(spell)));          //添加补全韵母---全拼
            list.Add(SplitSpell(RemoveInitials(spell)));        //移除所有声母---全拼
            list.Add(SplitSpell(RemoveFinals(spell)));          //移除所有韵母---全拼
            list.Add(SplitSpell(Remove(spell)));                //移除所有-------全拼
            list = Reverse(list); //翻转拼音

            List<string> resultList = null;
            if (list.Count >= 2)
            {
                int len = list.Count - 1;
                for (int i = 0; i < len; i++)
                {
                    if (resultList == null)
                        resultList = GetCombination(list[i], list[i + 1]);
                    else
                        resultList = GetCombination(resultList, list[i + 1]).Distinct().ToList();
                }
            }
            return GetCombinationToString(resultList);
        }

        /// <summary>
        /// 反转集合
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<List<string>> Reverse(List<List<string>> list)
        {
            List<List<string>> resultList = new List<List<string>>();
            int length = list[0].Count;
            for (int i = 0; i < length; i++)
            {
                List<string> li = new List<string>();
                foreach (var item in list)
                {
                    li.Add(item[i]);
                }
                resultList.Add(li);
            }
            return resultList;
        }

        /// <summary>
        /// 拼音的组合
        /// </summary>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        private static List<string> GetCombination(List<string> first, List<string> last)
        {
            int lenFirst = first.Count;
            int lenLast = last.Count;
            List<string> result = new List<string>();
            for (int i = 0; i < lenFirst; i++)
            {
                for (int j = 0; j < lenLast; j++)
                {
                    result.Add(first[i] + last[j]);
                }
            }
            return result.Distinct().ToList();
        }

        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static string GetCombinationToString(List<string> list)
        {           
            StringBuilder result = new StringBuilder();           
            foreach (var item in list)
            {
                result.Append(string.Format("{0}{1}", item, ";"));
            }
            return result.ToString().Substring(0, result.Length - 1);
        }

        /// <summary>
        /// 去重
        /// </summary>
        /// <param name="hybridSpell"></param>
        /// <returns></returns>
        private static string Distinct(string hybridSpell)
        {
            var list = hybridSpell.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Distinct();
            StringBuilder result = new StringBuilder();
            foreach (var item in list)
            {
                result.Append(string.Format("{0}{1}",item ,";"));
            }
            return result.ToString().Substring(0, result.Length - 1);
        }
    }
}

