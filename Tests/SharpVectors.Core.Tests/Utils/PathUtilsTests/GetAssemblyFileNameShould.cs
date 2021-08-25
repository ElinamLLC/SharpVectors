using System.Reflection;
using FluentAssertions;
using SharpVectors.Dom.Utils;
using Xunit;

namespace SharpVectors.Core.Tests.Utils.PathUtilsTests
{
	public sealed class GetAssemblyFileNameShould
	{
		[Fact]
		public void ReturnAssemblyName()
		{
			const string expectedResult = "SharpVectors.Core.Tests.dll";

			var result = PathUtils.GetAssemblyFileName(Assembly.GetExecutingAssembly());

			result
				.Should()
				.Be(expectedResult);
		}
	}
}
