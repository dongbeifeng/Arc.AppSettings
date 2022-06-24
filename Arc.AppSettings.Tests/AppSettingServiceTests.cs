// Copyright 2022 ������
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
    public async Task DoSetAsync_�������Ʋ���Ϊ��()
    {
        AppSettingService sut = new AppSettingService(For<ISession>());

        await Assert.ThrowsAsync<ArgumentException>(() => sut.DoSetAsync(string.Empty, AppSettingTypes.�ַ���, "FA"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.DoSetAsync(" ", AppSettingTypes.�ַ���, "FA"));
    }

    [Fact]
    public async Task DoSetAsync_�������Ͳ���Ϊ��()
    {
        AppSettingService sut = new AppSettingService(For<ISession>());

        await Assert.ThrowsAsync<ArgumentException>(() => sut.DoSetAsync("S", string.Empty, "FA"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.DoSetAsync("S", " ", "FA"));
    }

    [Fact]
    public async Task DoSetAsync_���ɸ��Ĳ�������()
    {
        var session = For<ISession>();

        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(new AppSetting
            {
                SettingName = "S",
                SettingType = AppSettingTypes.�ַ���,
                SettingValue = "FA",
            });

        AppSettingService sut = new AppSettingService(session);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.DoSetAsync("S", AppSettingTypes.����, "true"));
    }

    [Fact]
    public async Task DoSetAsync_�ܹ�Ϊ���в���������ֵ()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.�ַ���,
            SettingValue = "FA",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.DoSetAsync("S", AppSettingTypes.�ַ���, "WES");
        Assert.Equal("WES", appSetting.SettingValue);
    }

    [Fact]
    public async Task SetStringAsync_һ���Բ���()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.�ַ���,
            SettingValue = "FA",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.SetStringAsync("S", "WES");

        Assert.Equal("WES", appSetting.SettingValue);
    }

    [Fact]
    public async Task SetNumberAsync_һ���Բ���()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.����,
            SettingValue = "12",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.SetNumberAsync("S", 21);

        Assert.Equal("21", appSetting.SettingValue);
    }

    [Fact]
    public async Task SetBooleanAsync_һ���Բ���()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.����,
            SettingValue = "true",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.SetBooleanAsync("S", false);

        Assert.Equal("false", appSetting.SettingValue);
    }


    [Fact]
    public async Task GetStringAsync_һ���Բ���()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.�ַ���,
            SettingValue = "FA",
        };

        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetStringAsync("S", null);

        Assert.Equal("FA", val);
    }

    [Fact]
    public async Task GetStringAsync_����������ʱ��ʹ��Ĭ��ֵ����()
    {
        var session = For<ISession>();

        session.GetAsync<AppSetting?>("S", Arg.Any<LockMode>())
            .Returns((AppSetting?)null);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetStringAsync("S", "FA");

        Assert.Equal("FA", val);
    }

    [Fact]
    public async Task GetNumberAsync_һ���Բ���()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.����,
            SettingValue = "12",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetNumberAsync("S", null);

        Assert.Equal(12m, val);
    }

    [Fact]
    public async Task GetNumberAsync_����������ʱ��ʹ��Ĭ��ֵ����()
    {
        var session = For<ISession>();
        session.GetAsync<AppSetting?>("S", Arg.Any<LockMode>())
            .Returns((AppSetting?)null);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetNumberAsync("S", 12m);

        Assert.Equal(12m, val);
    }


    [Fact]
    public async Task GetBooleanAsync_һ���Բ���()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.����,
            SettingValue = "true",
        };

        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetBooleanAsync("S", null);

        Assert.True(val);
    }

    [Fact]
    public async Task GetBooleanAsync_����������ʱ��ʹ��Ĭ��ֵ����()
    {
        var session = For<ISession>();
        session.GetAsync<AppSetting?>("S", Arg.Any<LockMode>())
            .Returns((AppSetting?)null);

        AppSettingService sut = new AppSettingService(session);

        var val = await sut.GetBooleanAsync("S", true);

        Assert.True(val);
    }


    [Fact]
    public async Task DeleteAsync_һ���Բ���()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.�ַ���,
            SettingValue = "FA",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.DeleteAsync("S");
        await session.Received().DeleteAsync(appSetting);

    }

    [Fact]
    public async Task SetCommentAsync_һ���Բ���()
    {
        var session = For<ISession>();

        var appSetting = new AppSetting
        {
            SettingName = "S",
            SettingType = AppSettingTypes.�ַ���,
            SettingValue = "FA",
        };
        session.GetAsync<AppSetting>("S", Arg.Any<LockMode>())
            .Returns(appSetting);

        AppSettingService sut = new AppSettingService(session);

        await sut.SetCommentAsync("S", "WES");
        Assert.Equal("WES", appSetting.Comment);
    }


}
