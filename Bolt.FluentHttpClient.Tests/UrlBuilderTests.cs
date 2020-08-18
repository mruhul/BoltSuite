using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bolt.FluentHttpClient.Tests
{
    public class UrlBuilderTests
    {
        [Theory]
        [MemberData(nameof(TestDataForUrlBuilderBuild))]
        public void Build_Should_Return_Correct_Url(string url, NameValueUnit[] queryParams, string expected)
        {
            var got = UrlBuilder.Build(url, queryParams?.ToList());

            got.ShouldBe(expected);
        }

        public static IEnumerable<object[]> TestDataForUrlBuilderBuild =>
        new List<object[]>
        {
            // When queryparams are null
            new object[] 
            { 
                "http://hell-world.com", 
                null, 
                "http://hell-world.com" 
            },
            // when queryparams are null with path in url
            new object[] 
            { 
                "http://hell-world.com/test", 
                null, 
                "http://hell-world.com/test" 
            },
            // when contains 1 query param
            new object[] 
            { 
                "http://hell-world.com/test", 
                new [] { 
                    new NameValueUnit
                    {
                        Name = "hello",
                        Value = "world"
                    }
                }, 
                "http://hell-world.com/test?hello=world" 
            },
            // when contain multiple query params
            new object[]
            {
                "http://hell-world.com/test/",
                new [] {
                    new NameValueUnit
                    {
                        Name = "hello",
                        Value = "world"
                    },
                    new NameValueUnit
                    {
                        Name = "test",
                        Value = "1"
                    }
                },
                "http://hell-world.com/test/?hello=world&test=1"
            },
            // When contain one of query param that has null value
            new object[]
            {
                "http://hell-world.com/test",
                new [] {
                    new NameValueUnit
                    {
                        Name = "hello",
                        Value = "world"
                    },
                    new NameValueUnit
                    {
                        Name = "test",
                        Value = null
                    },
                    new NameValueUnit
                    {
                        Name = "test2",
                        Value = "2"
                    }
                },
                "http://hell-world.com/test?hello=world&test=&test2=2"
            },
            // when url has existing query strings and queryparams are not empty
            new object[]
            {
                "http://hell-world.com/test/?new=2&sample=data",
                new [] {
                    new NameValueUnit
                    {
                        Name = "hello",
                        Value = "world"
                    },
                    new NameValueUnit
                    {
                        Name = "test",
                        Value = null
                    }
                },
                "http://hell-world.com/test/?new=2&sample=data&hello=world&test="
            },
        };
    }
}
