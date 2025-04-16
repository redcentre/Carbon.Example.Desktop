using System.Windows.Controls;
using Carbon.Example.Desktop.Model;

namespace Carbon.Example.Desktop;

internal class BaseControl : UserControl
{
	protected MainController Controller => (MainController)DataContext;
}
