using System.Reflection;
using SharpVectors.Dom.Utils;

namespace WpfTestCostura
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			var assembly = Assembly.GetExecutingAssembly();

			AssemblyFileNameTextBlock.Text = $"Assembly file: {PathUtils.GetAssemblyFileName(assembly)}";
			AssemblyPathTextBlock.Text = $"Assembly file: {PathUtils.GetAssemblyPath(assembly)}";
			AssemblyDirectoryTextBlock.Text = $"Assembly file: {PathUtils.Combine(assembly)}";
		}
	}
}
