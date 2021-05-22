using System;

namespace Build_Installer
{
    class ProgressChangedEventArgs : EventArgs
    {
        public int Progress;
        public string Description;

        public ProgressChangedEventArgs(int progress, string description)
        {
            Progress = progress;
            Description = description;
        }
    }
}
