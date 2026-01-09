using System.Globalization;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ping.Client.Counter;
using Ping.Client.Counter.Services;
using Sdk.Client.NotificationArea.Services;
using Sdk.Client.Services;
using Sdk.Testing.Client;
using Xunit;

namespace Ping.Tests.Client.Components;

public sealed class CounterPageTests
{
    public class FormatMessage
    {
        [Fact]
        public void Should_increment_by_one_three_times()
        {
            var culture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            using var ctx = new TestContext();
            ctx.Services.AddLocalization();
            ctx.Services.AddSingleton<SimpleCounter>();
            ctx.Services.AddSingleton(Substitute.For<ILayoutService>());
            ctx.Services.AddSingleton(Substitute.For<IActiveNotificationElementPolicy>());
            ctx.Services.AddSingleton(Substitute.For<IJsInterop>());
            ctx.Services.AddSingleton(Substitute.For<IMessageBannerService>());
            var sut = ctx.RenderComponent<CounterPage>();

            sut.Find("#increment-result").Children[0].MarkupMatches("<em>You have clicked 0 times</em>");

            sut.Find("#increment-button").Click();

            sut.Find("#increment-result").Children[0].MarkupMatches("<em>You have clicked 1 times</em>");

            sut.Find("#increment-button").Click();

            sut.Find("#increment-result").Children[0].MarkupMatches("<em>You have clicked 2 times</em>");

            sut.Find("#increment-button").Click();

            sut.Find("#increment-result").Children[0].MarkupMatches("<em>You have clicked 3 times</em>");
        }
    }
}
