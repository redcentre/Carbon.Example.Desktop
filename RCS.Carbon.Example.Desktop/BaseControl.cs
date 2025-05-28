using System.Windows.Controls;
using RCS.Carbon.Example.Desktop.Model;

namespace RCS.Carbon.Example.Desktop;

internal class BaseControl : UserControl
{
	protected MainController Controller => (MainController)DataContext;
}
