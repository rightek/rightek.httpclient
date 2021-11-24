using System;
using System.Linq;
using System.Reflection;

using FluentAssertions;

using Xunit;

namespace Rightek.HttpClient.UnitTests
{
    public class AccessModifierTests
    {
        [Fact]
        public void All_classes_inside_internals_namespace_should_be_internal()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(c => c.FullName.Contains(Assembly.GetExecutingAssembly().GetName().Name.Replace(".Tests", "")))
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && (t.Namespace?.Contains("Internals") ?? false) && t.IsPublic)
                .ToList();

            string.Join(", ", types.Select(c => c.Name)).Should().Be("");
        }
    }
}