using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Region.Utils.Test
{
    [TestClass]
    public class SpellIndexUtilsTest 
    {
        /// <summary>
        /// 创建混合索引
        /// </summary>
        [TestMethod]
        public void TestCreateHybridIndex()
        {
            String result = SpellIndexUtils.CreateHybridIndex("妹子");
            Debug.Print(result);          
            Assert.IsNotNull(result);                
           
        }
    }
}
