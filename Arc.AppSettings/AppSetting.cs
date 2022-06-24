// Copyright 2022 王建军
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using NHibernateUtils;
using System.ComponentModel.DataAnnotations;

namespace Arc.AppSettings;


/// <summary>
/// 表示系统参数。
/// </summary>
public class AppSetting
{
    /// <summary>
    /// 初始化此类的新实例。
    /// </summary>
    internal protected AppSetting()
    {
    }

    /// <summary>
    /// 系统参数名称
    /// </summary>
    [Required]
    [MaxLength(128)]
    public virtual string SettingName { get; internal protected set; } = default!;

    /// <summary>
    /// 系统参数类型，可用的类型在 <see cref="AppSettingTypes"/> 中定义。
    /// </summary>
    [Required]
    [MaxLength(10)]
    public virtual string SettingType { get; internal protected set; } = default!;

    /// <summary>
    /// 系统参数值值
    /// </summary>
    [Required]
    [MaxLength(9999)]
    public virtual string SettingValue { get; internal protected set; } = default!;

    /// <summary>
    /// 系统参数备注
    /// </summary>
    [MaxLength(9999)]
    public virtual string? Comment { get; internal protected set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [CreationTime]
    public virtual DateTime CreationTime { get; set; }


    /// <summary>
    /// 创建人
    /// </summary>
    [MaxLength(30)]
    [CreationUser]
    public virtual string? CreationUser { get; set; }

    /// <summary>
    /// 更改时间
    /// </summary>
    [ModificationTime]
    public virtual DateTime ModificationTime { get; set; }

    /// <summary>
    /// 更改人
    /// </summary>
    [MaxLength(30)]
    [ModificationUser]
    public virtual string? ModificationUser { get; set; }
}
