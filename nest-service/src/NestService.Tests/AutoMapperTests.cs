using AutoMapper;
using NestService.Api;
using Xunit;

namespace NestService.Tests
{
    public class AutoMapperTests
    {
        [Fact(DisplayName = "Testing AutoMapper profiles.")]
        public void ShouldProperlyMapProfiles()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup)));
            config.AssertConfigurationIsValid();
        }
    }
}
