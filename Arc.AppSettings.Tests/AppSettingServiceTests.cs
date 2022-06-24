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

using NHibernate;
using NSubstitute;
using static NSubstitute.Substitute;

namespace Arc.AppSettings.Tests;

public class AppSettingServiceTests
{
    public AppSettingServiceTests()
    {
    }

    [Fact]
    public async Task DoSetAsync_参数名称不可为空()
    {
        AppSettingService sut = new AppSettingService(For<ISession>());

        await Assert.ThrowsAsync<ArgumentException>(() => sut.DoSetAsync(string.Empty, AppSettingTypes.字符串, "FA"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.DoSetAsync(" ", AppSettingTypes.字符串, "FA"));
    }

    [Fact]
    public async Task DoSetAsync_参数类型不可为空()
    {
        AppSettingService sut = new AppSettingService(For<ISession>());

        await Assert.ThrowsAsync<ArgumentException>(() => sut.DoSetAsync("S", string.Empty, "FA"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.DoSetAsync("S", " ", "FA"));
    }

    [Fact]
    public async Task DoSetAsync_不可更改参数类型()
    {
        var session = For<ISession>();

        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(new AppSetting
            {
                SettingName = "S",
                SettingType = AppSettingTypes.字符串,
                SettingValue = "FA",
            });

        AppSettingService sut = new AppSettingService(session);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.DoSetAsync("S", AppSettingTypes.布尔, "true"));
    }

    [Fact]
    public async Task DoSetAsync_能够为已有参数设置新值()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.字符串,
            SettingValue = "FA",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.DoSetAsync("S", AppSettingTypes.字符串, "WES");
        Assert.Equal("WES", appSetting.SettingValue);
    }

    [Fact]
    public async Task SetStringAsync_一般性测试()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.字符串,
            SettingValue = "FA",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.SetStringAsync("S", "WES");

        Assert.Equal("WES", appSetting.SettingValue);
    }

    [Fact]
    public async Task SetNumberAsync_一般性测试()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.数字,
            SettingValue = "12",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.SetNumberAsync("S", 21);

        Assert.Equal("21", appSetting.SettingValue);
    }

    [Fact]
    public async Task SetBooleanAsync_一般性测试()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.布尔,
            SettingValue = "true",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.SetBooleanAsync("S", false);

        Assert.Equal("false", appSetting.SettingValue);
    }


    [Fact]
    public async Task GetStringAsync_一般性测试()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.字符串,
            SettingValue = "FA",
        };

        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetStringAsync("S", null);

        Assert.Equal("FA", val);
    }

    [Fact]
    public async Task GetStringAsync_参数不存在时会使用默认值创建()
    {
        var session = For<ISession>();

        session.GetAsync<AppSetting?>("S", Arg.Any<LockMode>())
            .Returns((AppSetting?)null);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetStringAsync("S", "FA");

        Assert.Equal("FA", val);
    }

    [Fact]
    public async Task GetNumberAsync_一般性测试()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.数字,
            SettingValue = "12",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetNumberAsync("S", null);

        Assert.Equal(12m, val);
    }

    [Fact]
    public async Task GetNumberAsync_参数不存在时会使用默认值创建()
    {
        var session = For<ISession>();
        session.GetAsync<AppSetting?>("S", Arg.Any<LockMode>())
            .Returns((AppSetting?)null);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetNumberAsync("S", 12m);

        Assert.Equal(12m, val);
    }


    [Fact]
    public async Task GetBooleanAsync_一般性测试()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.布尔,
            SettingValue = "true",
        };

        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetBooleanAsync("S", null);

        Assert.True(val);
    }

    [Fact]
    public async Task GetBooleanAsync_参数不存在时会使用默认值创建()
    {
        var session = For<ISession>();
        session.GetAsync<AppSetting?>("S", Arg.Any<LockMode>())
            .Returns((AppSetting?)null);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetBooleanAsync("S", true);

        Assert.True(val);
    }


    [Fact]
    public async Task DeleteAsync_一般性测试()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.字符串,
            SettingValue = "FA",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.DeleteAsync("S");
        await session.Received().DeleteAsync(appSetting);

    }

    [Fact]
    public async Task SetCommentAsync_一般性测试()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.字符串,
            SettingValue = "FA",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.SetCommentAsync("S", "WES");
        Assert.Equal("WES", appSetting.Comment);
    }


}
