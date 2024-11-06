using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace ITS.Retail.Platform.Tests.Common
{
    public class PropertyDataWithFixtureAttribute : PropertyDataAttribute
    {
        public string FixtureTypeName { get; protected set; }

        public PropertyDataWithFixtureAttribute(string propertyName,string fixtureTypeName) : base(propertyName)
        {
            this.FixtureTypeName =fixtureTypeName;

            if (Fixture.InitializedFixtures == null)
            {
                Fixture.InitializedFixtures = new Dictionary<Type, Fixture>();
            }
        }

        public override IEnumerable<object[]> GetData(System.Reflection.MethodInfo methodUnderTest, Type[] parameterTypes)
        {
            Type testClassType = methodUnderTest.DeclaringType;
            Type FixtureType = Assembly.GetAssembly(testClassType).GetType(FixtureTypeName);
            
            Assert.NotNull(FixtureType);
            
            if (!Fixture.InitializedFixtures.ContainsKey(testClassType))
            {
                Fixture fixture = Activator.CreateInstance(FixtureType) as Fixture;
                fixture.Initialize();
                Fixture.InitializedFixtures.Add(testClassType, fixture);
            }

            return base.GetData(methodUnderTest, parameterTypes);
        }

    }
}
