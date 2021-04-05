using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace CueGen.Console
{
    class ProgressBar
    {
        public char ProgressChar { get; set; } = '#';
        public char StartChar { get; set; } = '[';
        public char EndChar { get; set; } = ']';
        public char EmptyChar { get; set; } = ' ';

        public void Report(double percentage, string message)
        {
            var barWidth = WindowWidth - 3; // StartChar + EndChar + ' '
            var pct = ((int)Math.Round(percentage * 100)).ToString("d");
            barWidth -= pct.Length + 2; // pct + "% "
            if (!string.IsNullOrEmpty(message)) barWidth -= (message.Length + 1); // ' ' + message
            var elapsedWidth = (int)Math.Round(percentage * barWidth);
            var remainingWidth = barWidth - elapsedWidth;
            var sb = new StringBuilder();

            sb.Append(pct);
            sb.Append("% ");

            sb.Append(StartChar);
            sb.Append(ProgressChar, elapsedWidth);
            sb.Append(EmptyChar, remainingWidth);
            sb.Append(EndChar);

            if (!string.IsNullOrEmpty(message))
            {
                sb.Append(' ');
                sb.Append(message);
            }

            Write('\r');
            Write(sb.ToString());
        }
    }
}
