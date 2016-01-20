using Castle.Core.Logging;
using JetBrains.Annotations;
using NSubstitute;
using Selkie.XUnit.Extensions;
using Xunit;
using Xunit.Extensions;

namespace Selkie.Aop.Tests.XUnit
{
    public sealed class LoggerRepositoryTests
    {
        [Theory]
        [AutoNSubstituteData]
        public void Get_CreatesNewLogger_ForNewType([NotNull] LoggerRepository sut)
        {
            // Arrange
            // Act
            ILogger actual = sut.Get("One");

            // Assert
            Assert.NotNull(actual);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Get_ReturnsExistingLogger_ForExistingType([NotNull] ILogger loggerOne,
                                                              [NotNull] ILogger loggerTwo)
        {
            // Arrange
            var factory = Substitute.For <ILoggerFactory>();
            factory.Create(Arg.Any <string>()).Returns(loggerOne,
                                                       loggerTwo);
            var sut = new LoggerRepository(factory);

            ILogger expected = sut.Get("One");

            // Act
            ILogger actual = sut.Get("One");

            // Assert
            Assert.Equal(expected,
                         actual);
        }

        [Theory]
        [AutoNSubstituteData]
        public void Get_ReturnsDifferentLogger_ForDifferentTypes([NotNull] ILogger loggerOne,
                                                                 [NotNull] ILogger loggerTwo)
        {
            // Arrange
            var factory = Substitute.For <ILoggerFactory>();
            factory.Create(Arg.Any <string>()).Returns(loggerOne,
                                                       loggerTwo);
            var sut = new LoggerRepository(factory);

            // Act
            ILogger one = sut.Get("One");
            ILogger two = sut.Get("Two");

            // Assert
            Assert.NotEqual(two,
                            one);
        }
    }
}