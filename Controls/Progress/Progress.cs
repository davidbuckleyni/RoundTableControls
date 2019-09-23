using System;
using System.Collections.Generic;
using System.Text;

namespace RoundTableUkCustomControls.Controls.Progress
{
    public class Progress : EventArgs
    {
        public int Status { get; private set; }

        private Progress() { }

        public Progress(int status)
        {
            Status = status;
        }

    }
    }
