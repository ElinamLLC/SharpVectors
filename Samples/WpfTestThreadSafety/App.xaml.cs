using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfTestThreadSafety
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void FrameworkSelectionTest()
        {
#if NET20
        Trace.WriteLine("Target framework: NET20");
#elif NET35
        Trace.WriteLine("Target framework: NET35");
#elif NET40
        Trace.WriteLine("Target framework: NET40");
#elif NET45
        Trace.WriteLine("Target framework: NET45");
#elif NET451
        Trace.WriteLine("Target framework: NET451");
#elif NET452
        Trace.WriteLine("Target framework: NET452");
#elif NET46
        Trace.WriteLine("Target framework: NET46");
#elif NET461
        Trace.WriteLine("Target framework: NET461");
#elif NET462
        Trace.WriteLine("Target framework: NET462");
#elif NET47
        Trace.WriteLine("Target framework: NET47");
#elif NET471
        Trace.WriteLine("Target framework: NET471");
#elif NET472
        Trace.WriteLine("Target framework: NET472");
#elif NETSTANDARD1_0
        Trace.WriteLine("Target framework: NETSTANDARD1_0");
#elif NETSTANDARD1_1
        Trace.WriteLine("Target framework: NETSTANDARD1_1");
#elif NETSTANDARD1_2
        Trace.WriteLine("Target framework: NETSTANDARD1_2");
#elif NETSTANDARD1_3
        Trace.WriteLine("Target framework: NETSTANDARD1_3");
#elif NETSTANDARD1_4
        Trace.WriteLine("Target framework: NETSTANDARD1_4");
#elif NETSTANDARD1_5
        Trace.WriteLine("Target framework: NETSTANDARD1_5");
#elif NETSTANDARD1_6
        Trace.WriteLine("Target framework: NETSTANDARD1_6");
#elif NETSTANDARD2_0
        Trace.WriteLine("Target framework: NETSTANDARD2_0");
#elif NETCOREAPP1_0
        Trace.WriteLine("Target framework: NETCOREAPP1_0");
#elif NETCOREAPP1_1
        Trace.WriteLine("Target framework: NETCOREAPP1_1");
#elif NETCOREAPP2_0
        Trace.WriteLine("Target framework: NETCOREAPP2_0");
#elif NETCOREAPP2_1
        Trace.WriteLine("Target framework: NETCOREAPP2_1");
#elif NETCOREAPP2_2
        Trace.WriteLine("Target framework: NETCOREAPP2_2");
#elif NETCOREAPP3_0
        Trace.WriteLine("Target framework: NETCOREAPP3_0");
#elif NETCOREAPP3_1
            Trace.WriteLine("Target framework: NETCOREAPP3_1");
#else
            Trace.WriteLine("Could not tell which framework we're using.");
#endif
        }
    }
}
