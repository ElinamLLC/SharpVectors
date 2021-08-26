using System;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using SharpVectors.Dom.Utils;

namespace SharpVectors.Core.Tests.Utils
{
	public sealed class PathUtilsTests
	{
		private const string ConfigurationDir =
#if DEBUG
			"Debug"
#elif RELEASE
			"Release"
#endif
		;

		private const string RuntimeDir =
#if NET40
			"net40"
#elif NET45
			"net45"
#elif NET46
			"net46"
#elif NET47
			"net47"
#elif NET48
			"net48"
#elif NET5_0
			"net5.0"
#endif
			;

		private Assembly _fixture;
		private MethodInfo _combineInternal, _getAssemblyPathInternal;

		[SetUp]
		public void Setup()
		{
			_fixture = Assembly.GetExecutingAssembly();
		}

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_combineInternal = GetPrivateMethod("CombineInternal");
			_getAssemblyPathInternal = GetPrivateMethod("GetAssemblyPathInternal");
		}

		[Test]
		public void CombineReturnBasePathLocation()
		{
			string want = Path.Combine("Tests", "SharpVectors.Core.Tests", "bin", ConfigurationDir, RuntimeDir),
				got = GetProjectRelativePath(PathUtils.Combine(_fixture));

			Assert.AreEqual(want, got);
		}

		[Test]
		public void CombineReturnBasePathAppDomain()
		{
			string want = Path.Combine("Tests", "SharpVectors.Core.Tests", "bin", ConfigurationDir, RuntimeDir),
				got = GetProjectRelativePath(CombineInternal(""));

			Assert.AreEqual(want, got);
		}

		[Test]
		public void CombineReturnCombinedPathLocation()
		{
			const string dir1 = nameof(dir1), dir2 = nameof(dir2);

			string want = Path.Combine("Tests", "SharpVectors.Core.Tests", "bin", ConfigurationDir, RuntimeDir, dir1, dir2),
				got = GetProjectRelativePath(PathUtils.Combine(_fixture, dir1, dir2));

			Assert.AreEqual(want, got);
		}

		[Test]
		public void CombineReturnCombinedPathAppDomain()
		{
			const string dir1 = nameof(dir1), dir2 = nameof(dir2);

			string want = Path.Combine("Tests", "SharpVectors.Core.Tests", "bin", ConfigurationDir, RuntimeDir, dir1, dir2),
				got = GetProjectRelativePath(CombineInternal("", dir1, dir2));

			Assert.AreEqual(want, got);
		}

		private string CombineInternal(string location, params string[] paths) =>
			(string)_combineInternal.Invoke(null, new object[] { location, paths });

		private static MethodInfo GetPrivateMethod(string name) =>
			typeof(PathUtils).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);

		/// <summary>
		/// Converts the absolute path to the relative path (from the project root directory)
		/// </summary>
		private static string GetProjectRelativePath(string absolutePath)
		{
			const string testDir = "Tests";

			var sb = new StringBuilder();
			var split = absolutePath.Split(Path.DirectorySeparatorChar);

			for (var i = split.Length - 1; i >= 0; i--)
			{
				if (sb.Length > 0)
					sb.Insert(0, Path.DirectorySeparatorChar);

				sb.Insert(0, split[i]);

				if (split[i] == testDir)
					return sb.ToString();
			}

			throw new InvalidOperationException("Cannot find the root directory of tests: /" + testDir);
		}
	}
}
