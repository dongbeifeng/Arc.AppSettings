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

using Microsoft.Toolkit.Diagnostics;
using NHibernate;
using NHibernate.Linq;

namespace Arc.AppSettings;

public class AppSettingService : IAppSettingService
{
    ISession _session;

    public AppSettingService(ISession session)
    {
        _session = session;
    }

    public Task<AppSetting> GetAsync(string name)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        name = name.Trim();
        return _session.GetAsync<AppSetting>(name, LockMode.Upgrade);
    }

    internal async Task<AppSetting> DoSetAsync(string name, string type, string value)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        Guard.IsNotNullOrWhiteSpace(type, nameof(type));
        Guard.IsNotNullOrWhiteSpace(value, nameof(value));

        name = name.Trim();
        type = type.Trim();
        value = value.Trim();

        AppSetting setting = await _session.GetAsync<AppSetting>(name, LockMode.Upgrade).ConfigureAwait(false);
        if (setting == null)
        {
            setting = new AppSetting();
            setting.SettingName = name;
            setting.SettingType = type;
            setting.SettingValue = value;
            await _session.SaveAsync(setting).ConfigureAwait(false);
        }
        else
        {
            if (setting.SettingType != type)
            {
                throw new InvalidOperationException("不能更改设置类型。");
            }
            setting.SettingValue = value;
            await _session.UpdateAsync(setting).ConfigureAwait(false);
        }
        return setting;
    }

    public Task SetNumberAsync(string name, decimal value)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        name = name.Trim();

        return this.DoSetAsync(name, AppSettingTypes.数字, value.ToString("0.##########"));
    }

    public async Task<decimal?> GetNumberAsync(string name, decimal? defaultValue)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        name = name.Trim();

        var s = await this.GetAsync(name).ConfigureAwait(false);
        if (s == null)
        {
            if (defaultValue == null)
            {
                return null;
            }
            else
            {
                await SetNumberAsync(name, defaultValue.Value).ConfigureAwait(false);
                return defaultValue;
            }
        }
        if (s.SettingType != AppSettingTypes.数字)
        {
            throw new InvalidOperationException($"设置【{name}】的类型不是数字。");
        }

        return decimal.Parse(s.SettingValue);
    }


    public Task SetStringAsync(string name, string value)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        Guard.IsNotNullOrWhiteSpace(value, nameof(value));

        name = name.Trim();
        value = value.Trim();

        return this.DoSetAsync(name, AppSettingTypes.字符串, value);
    }

    public async Task<string?> GetStringAsync(string name, string? defaultValue)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        name = name.Trim();

        var s = await this.GetAsync(name).ConfigureAwait(false);
        if (s == null)
        {
            if (defaultValue == null)
            {
                return null;
            }
            else
            {
                await SetStringAsync(name, defaultValue).ConfigureAwait(false);
                return defaultValue;
            }
        }
        if (s.SettingType != AppSettingTypes.字符串)
        {
            throw new InvalidOperationException($"；设置【{name}】的类型不是字符串。");
        }

        return s.SettingValue;
    }

    public Task SetBooleanAsync(string name, bool value)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        name = name.Trim();

        return this.DoSetAsync(name, AppSettingTypes.布尔, value.ToString().ToLower());
    }

    public async Task<bool?> GetBooleanAsync(string name, bool? defaultValue)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        name = name.Trim();

        var s = await this.GetAsync(name).ConfigureAwait(false);
        if (s == null)
        {
            if (defaultValue == null)
            {
                return null;
            }
            else
            {
                await SetBooleanAsync(name, defaultValue.Value).ConfigureAwait(false);
                return defaultValue.Value;
            }
        }
        if (s.SettingType != AppSettingTypes.布尔)
        {
            throw new InvalidOperationException($"；设置【{name}】的类型不是布尔。");
        }

        return bool.Parse(s.SettingValue);
    }

    public Task<List<AppSetting>> GetAllAsync()
    {
        return _session.Query<AppSetting>().ToListAsync();
    }

    public async Task DeleteAsync(string name)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        name = name.Trim();

        AppSetting setting = await _session.GetAsync<AppSetting>(name, LockMode.Upgrade).ConfigureAwait(false);
        if (setting != null)
        {
            await _session.DeleteAsync(setting).ConfigureAwait(false);
        }
    }

    public async Task SetCommentAsync(string name, string? comment)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));
        name = name.Trim();

        AppSetting setting = await _session.GetAsync<AppSetting>(name, LockMode.Upgrade).ConfigureAwait(false);
        if (setting != null)
        {
            setting.Comment = comment;
        }
    }

}
