
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseworkWPF.MilitaryFolder;
using SOLIDCheckingLibrary;
using Xunit;

namespace CourseworkWPF.Tests
{
    public class LiskovPrincipleTests
    {
        [Fact]
        public void OrSpecificationWithChainSpecificationFollowsLiskov()
        {
            var result = LiskovPrinciple.CheckMethodsOfParentIsOverriden(typeof(ChainSpecificationOr<>), typeof(ChainSpecification<>));
            Assert.True(result.Item1, result.Item2);
        }
        [Fact]
        public void AndSpecificationWithChainSpecificationFollowsLiskov()
        {
            var result = LiskovPrinciple.CheckMethodsOfParentIsOverriden(typeof(ChainSpecificationAnd<>), typeof(ChainSpecification<>));
            Assert.True(result.Item1, result.Item2);
        }
    }
}
