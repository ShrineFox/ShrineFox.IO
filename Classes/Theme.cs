using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ShrineFox.IO
{
    public class Theme
    {
        public Theme()
        {
            ForeColor = Color.Silver;
            BackColor = Color.FromArgb(30, 30, 30);
            BackColorDark = Color.FromArgb(20, 20, 20);
            Highlight = Color.FromArgb(0, 174, 219);
        }

        public Color ForeColor { get; set; } = Color.Silver;
        public Color BackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color BackColorDark { get; set; } = Color.FromArgb(20, 20, 20);
        public Color Highlight { get; set; } = Color.FromArgb(0, 174, 219);
    }
}