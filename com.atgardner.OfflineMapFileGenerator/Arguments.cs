namespace com.atgardner.OMFG
{
    using packagers;
    using sources;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    class Arguments
    {
        private string outputFile;
        public bool Interactive { get; set; }
        public List<string> InputFiles { get; set; }
        public List<string> ZoomLevelsString
        {
            get
            {
                return (from z in ZoomLevels select z.ToString()).ToList();
            }
            set
            {
                ZoomLevels = new List<int>();
                foreach (var z in value)
                {
                    var zoom = 0;
                    if (int.TryParse(z, out zoom))
                    {
                        ZoomLevels.Add(zoom);
                    }
                    else
                    {
                        var range = z.Split('-');
                        var start = 0;
                        var end = 0;
                        if (int.TryParse(range[0], out start) && int.TryParse(range[1], out end))
                        {
                            for (var i = start; i <= end; i += 1)
                            {
                                ZoomLevels.Add(i);
                            }
                        }
                    }
                }
            }
        }

        public string SourceDescriptorString { get; set; }

        public string OutputFile
        {
            get
            {
                return outputFile ?? GenerateOutputFile();
            }
            set
            {
                outputFile = value;
            }
        }

        public FormatType FormatType { get; set; }
        public List<int> ZoomLevels { get; private set; }
        public SourceDescriptor SourceDescriptor { get; private set; }

        public bool hasAllFields
        {
            get
            {
                return InputFiles.Count > 0 && ZoomLevels.Count > 0 && SourceDescriptor != null && FormatType != FormatType.None;
            }
        }

        public void ParseSourceDescriptor(SourceDescriptor[] sources)
        {
            SourceDescriptor = (from s in sources
                                where s.Name == SourceDescriptorString
                                select s).SingleOrDefault();
        }

        private string GenerateOutputFile()
        {
            var zoomCount = ZoomLevels.Count();
            if (InputFiles.Count == 0 || zoomCount == 0 || SourceDescriptor == null)
            {
                return string.Empty;
            }

            var firstInputFileName = Path.GetFileNameWithoutExtension(InputFiles.Last());
            return string.Format("{0} - {1} - {2}-{3}", firstInputFileName, SourceDescriptor.Name, ZoomLevels.Min(), ZoomLevels.Max());
        }
    }
}
