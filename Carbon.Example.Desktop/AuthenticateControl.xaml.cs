using System.Windows;
using Carbon.Example.Desktop.Model;

namespace Carbon.Example.Desktop;

internal partial class AuthenticateControl : BaseControl
{
	public AuthenticateControl()
	{
		InitializeComponent();
		Loaded += AuthenticateControl_Loaded;
	}

	void AuthenticateControl_Loaded(object sender, RoutedEventArgs e)
	{
		void CredFocus()
		{
			if (Controller.AuthData.ActiveCredType == CredentialType.Id)
			{
				if (Controller.AuthData.CredentialId == null)
				{
					TextId.Focus();
				}
				else
				{
					TextPassword.Focus();
				}
			}
			if (Controller.AuthData.CredentialName == null)
			{
				TextName.Focus();
			}
			else
			{
				TextPassword.Focus();
			}
		}
		if (Controller.AuthData.ActiveLicensingType == LicenceProviderType.RedCentre)
		{
			if (Controller.AuthData.RcsLicBaseAddress == null)
			{
				TextBaseAddr.Focus();
			}
			else
			{
				CredFocus();
			}
		}
		else
		{
			if (Controller.AuthData.BPrLicProductKey == null)
			{
				TextProductKey.Focus();
			}
			else if (Controller.AuthData.BPrLicAdoConnect == null)
			{
				TextAdoConnect.Focus();
			}
			else
			{
				CredFocus();
			}
		}
	}
}
