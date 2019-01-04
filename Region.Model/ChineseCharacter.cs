using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Region.Model
{
    /// <summary>
    /// 汉字拼音实体
    /// </summary>
    public class ChineseCharacter
    {
        /// <summary>
        /// 简拼
        /// </summary>
        public string ShortSpell { get; set; }

        /// <summary>
        /// 全拼
        /// </summary>
        public string Spell { get; set; }
    }
}
