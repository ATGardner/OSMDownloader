namespace com.atgardner.OMFG
{
    using Fclp;
    using packagers;
    using sources;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    static class Program
    {
        private static readonly string SourceFile = @"sources\sources.json";
        private static SourceDescriptor[] sources;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                sources = SourceDescriptor.LoadSources(SourceFile);
            }
            catch
            {
                MessageBox.Show("Failed reading sources", "Missing sources", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var mainController = new MainController();

            var arguments = ParseArguments(args);
            if (!arguments.hasAllFields || arguments.Interactive)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var mainForm = new MainForm(sources);
                mainForm.MainController = mainController;
                mainForm.InputFiles = arguments.InputFiles;
                mainForm.ZoomLevels = arguments.ZoomLevels;
                mainForm.SourceDescriptor = arguments.SourceDescriptor;
                mainForm.OutputFile = arguments.OutputFile;
                mainForm.FormatType = arguments.FormatType;
                Application.Run(mainForm);
            }
            else
            {
                mainController.ProgressChanged += (s, e) =>
                {
                    Console.WriteLine(e.UserState);
                };
                Task.WaitAll(mainController.DownloadTilesAsync(arguments.InputFiles.ToArray(), arguments.ZoomLevels.ToArray(), arguments.SourceDescriptor, arguments.OutputFile, arguments.FormatType));
            }
        }

        private static Arguments ParseArguments(string[] args)
        {
            var p = new FluentCommandLineParser<Arguments>();
            //p.IsCaseSensitive = false;
            //p.SetupHelp("?", "help")
            //    .Callback(text => Console.WriteLine(text));
            p.Setup(arg => arg.Interactive)
                .As('x', "interactive")
                .WithDescription("Always show options dialog")
                .SetDefault(false);
            p.Setup(arg => arg.InputFiles)
                .As('i', "input")
                .WithDescription("A list of input GPX/KML/KMZ files")
                .SetDefault(new List<string>());
            p.Setup(arg => arg.ZoomLevelsString)
                .As('z', "zoomLevels")
                .WithDescription("Space saparated zoom levels, or range (e.g. - \"0 2-3 5\")")
                .SetDefault(new List<string>());
            p.Setup(arg => arg.SourceDescriptorString)
                .As('s', "source")
                .WithDescription("Name of required map source (e.g. - \"USA Topo Maps\")");
            p.Setup(arg => arg.OutputFile)
                .As('o', "output")
                .WithDescription("Output file name");
            p.Setup(arg => arg.FormatType)
                .As('f', "format")
                .WithDescription("Output format - BCNav/MBTiles or both")
                .SetDefault(FormatType.None);
            var result = p.Parse(args);
            if (result.HasErrors)
            {
                p.HelpOption.ShowHelp(p.Options);
            }

            var arguments = p.Object;
            arguments.ParseSourceDescriptor(sources);
            return arguments;
        }
    }
}
