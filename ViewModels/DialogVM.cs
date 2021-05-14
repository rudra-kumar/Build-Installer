using System;
using System.Collections.Generic;
using System.Text;

namespace Build_Installer.ViewModels
{
    class DialogVM : ViewModel
    {
        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                NotifyPropertyChanged(nameof(IsVisible));
            }
        }
    }
}
