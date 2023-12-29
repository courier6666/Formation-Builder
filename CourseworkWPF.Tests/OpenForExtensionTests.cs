using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseworkWPF;
using CourseworkWPF.MilitaryFolder;
using SOLIDCheckingLibrary;
using Xunit;

namespace CourseworkWPF.Tests
{
    public class OpenForExtensionTests
    {
        [Fact]
        public void ChainSpecificationOpenForExtension()
        {
            var result = OpenExtClosedMod.IsClassExtensible(typeof(ChainSpecification<>));

            Assert.True(result.Item1, result.Item2);
        }
        [Fact]
        public void ChainSpecificationAndOpenForExtension()
        {
            var result = OpenExtClosedMod.IsClassExtensible(typeof(ChainSpecificationAnd<>));

            Assert.True(result.Item1, result.Item2);
        }
        [Fact]
        public void ChainSpecificationOrOpenForExtension()
        {
            var result = OpenExtClosedMod.IsClassExtensible(typeof(ChainSpecificationOr<>));

            Assert.True(result.Item1, result.Item2);
        }
        [Fact]
        public void FilterOpenForExtension()
        {
            var result = OpenExtClosedMod.IsClassExtensible(typeof(ConcreteFilter<>));

            Assert.True(result.Item1, result.Item2);
        }
    }
}
