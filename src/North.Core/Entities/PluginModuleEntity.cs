﻿using SqlSugar;

namespace North.Core.Entities
{
    [SugarTable("PluginModules")]
    public class PluginModuleEntity : Entity
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Name { get; set; }

        /// <summary>
        /// 模块所属类别
        /// </summary>
        [SugarColumn(Length = 64)]
        public string Category { get; set; }

        /// <summary>
        /// 执行顺序
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 是否使能
        /// </summary>
        public bool IsEnabled { get; set; }

        #region 导航属性
        public Guid PluginId { get; set; }
        #endregion
    }
}
