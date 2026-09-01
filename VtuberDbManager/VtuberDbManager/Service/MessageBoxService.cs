using System.Windows;

namespace VtuberDbManager.Service;

internal class MessageBoxService : IMessageBoxService
{
    public void Show(string message, string title)
    {
        MessageBox.Show(message, title);
    }
}
