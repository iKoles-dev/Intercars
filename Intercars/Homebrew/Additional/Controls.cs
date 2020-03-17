using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Homebrew
{
    public static class Controls
    {
        public static RichTextBox DebugBox { get; set; }
        public static ProgressBar WorkProgress { get; set; }
        public static Label WorkProgressLabel { get; set; }
    }
}
