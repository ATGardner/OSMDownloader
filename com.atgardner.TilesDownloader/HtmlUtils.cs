using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.atgardner.TilesDownloader
{
    static class HtmlUtils
    {
        private static readonly Regex hrefRegex = new Regex(@"<a href=""(?<href>[^""]*)"">(?<text>[^<]*)</a>");

        public static void ConfigLinkLabel(LinkLabel lnkLabel, string sourceString)
        {
            lnkLabel.Links.Clear();
            lnkLabel.Text = sourceString;
            if (string.IsNullOrWhiteSpace(sourceString))
            {
                return;
            }

            var match = hrefRegex.Match(sourceString);
            while (match.Success)
            {
                var href = match.Groups["href"];
                var text = match.Groups["text"];
                sourceString = sourceString.Replace(match.Value, text.Value);
                lnkLabel.Text = sourceString;
                lnkLabel.Links.Add(match.Index, text.Length, href.Value);
                match = hrefRegex.Match(sourceString);
            }
        }
    }
}
