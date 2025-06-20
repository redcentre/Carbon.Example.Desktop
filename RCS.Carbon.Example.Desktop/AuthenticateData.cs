using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using RCS.Carbon.Example.Desktop.Model;

namespace RCS.Carbon.Example.Desktop;

/// <summary>
/// Encapsulates all of the authentication values and their validation states for easy binding.
/// </summary>
/// <remarks>
/// Putting all the properties in here means the MVVM toolkit can't directly use their changes
/// as part of the relay command enabling. See the note in MainController ctor.
/// </remarks>
internal partial class AuthenticateData : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	#region Control Binding Properties

	LicenceProviderType _activeLicensingType;
	public LicenceProviderType ActiveLicensingType
	{
		get => _activeLicensingType;
		set
		{
			if (_activeLicensingType != value)
			{
				_activeLicensingType = value;
				OnPropertyChanged(nameof(ActiveLicensingType));
				CalcErrors();
			}
		}
	}

	string? _rcsLicBaseAddress;
	public string? RcsLicBaseAddress
	{
		get => _rcsLicBaseAddress;
		set
		{
			string? newval = string.IsNullOrEmpty(value) ? null : value;
			if (_rcsLicBaseAddress != newval)
			{
				_rcsLicBaseAddress = newval;
				OnPropertyChanged(nameof(RcsLicBaseAddress));
				CalcErrors();
			}
		}
	}

	string? _rcsLicApiKey;
	public string? RcsLicApiKey
	{
		get => _rcsLicApiKey;
		set
		{
			string? newval = string.IsNullOrEmpty(value) ? null : value;
			if (_rcsLicApiKey != newval)
			{
				_rcsLicApiKey = newval;
				OnPropertyChanged(nameof(RcsLicApiKey));
				CalcErrors();
			}
		}
	}

	int _rcsLicTimeout;
	public int RcsLicTimeout
	{
		get => _rcsLicTimeout;
		set
		{
			if (_rcsLicTimeout != value)
			{
				_rcsLicTimeout = value;
				OnPropertyChanged(nameof(RcsLicTimeout));
				CalcErrors();
			}
		}
	}

	string? _bprLicProductKey;
	public string? BPrLicProductKey
	{
		get => _bprLicProductKey;
		set
		{
			string? newval = string.IsNullOrEmpty(value) ? null : value;
			if (_bprLicProductKey != newval)
			{
				_bprLicProductKey = newval;
				OnPropertyChanged(nameof(BPrLicProductKey));
				CalcErrors();
			}
		}
	}

	string? _bprLicAdoConnect;
	public string? BPrLicAdoConnect
	{
		get => _bprLicAdoConnect;
		set
		{
			string? newval = string.IsNullOrEmpty(value) ? null : value;
			if (_bprLicAdoConnect != newval)
			{
				_bprLicAdoConnect = newval;
				OnPropertyChanged(nameof(BPrLicAdoConnect));
				CalcErrors();
			}
		}
	}

	CredentialType _activeCredType;
	public CredentialType ActiveCredType
	{
		get => _activeCredType;
		set
		{
			if (_activeCredType != value)
			{
				_activeCredType = value;
				OnPropertyChanged(nameof(ActiveCredType));
				CalcErrors();
			}
		}
	}

	string? _credentialId;
	public string? CredentialId
	{
		get => _credentialId;
		set
		{
			string? newval = string.IsNullOrEmpty(value) ? null : value;
			if (_credentialId != newval)
			{
				_credentialId = newval;
				OnPropertyChanged(nameof(CredentialId));
				CalcErrors();
			}
		}
	}

	string? _credentialName;
	public string? CredentialName
	{
		get => _credentialName;
		set
		{
			string? newval = string.IsNullOrEmpty(value) ? null : value;
			if (_credentialName != newval)
			{
				_credentialName = newval;
				OnPropertyChanged(nameof(CredentialName));
				CalcErrors();
			}
		}
	}

	string? _password;
	public string? Password
	{
		get => _password;
		set
		{
			string? newval = string.IsNullOrEmpty(value) ? null : value;
			if (_password != newval)
			{
				_password = newval;
				OnPropertyChanged(nameof(Password));
				CalcErrors();
			}
		}
	}

	bool _rememberMe;
	public bool RememberMe
	{
		get => _rememberMe;
		set
		{
			if (_rememberMe != value)
			{
				_rememberMe = value;
				OnPropertyChanged(nameof(RememberMe));
				CalcErrors();
			}
		}
	}

	#endregion

	public ObservableCollection<string> ErrorMessages { get; } = [];

	void CalcErrors()
	{
		ErrorMessages.Clear();
		if (_activeLicensingType == LicenceProviderType.RedCentre)
		{
			if (_rcsLicBaseAddress == null)
			{
				ErrorMessages.Add(Strings.AuthErrorNeedBaseUri);
			}
			else
			{
				if (!RegBaseUri().IsMatch(_rcsLicBaseAddress))
				{
					ErrorMessages.Add(Strings.AuthErrorBadUri);
				}
			}
			if (_rcsLicTimeout < 10 || _rcsLicTimeout > 60)
			{
				ErrorMessages.Add(Strings.AuthErrorBadTimeout);
			}
		}
		else
		{
			if (_bprLicProductKey == null)
			{
				ErrorMessages.Add(Strings.AuthErrorNeedProdKey);
			}
			if (_bprLicAdoConnect == null)
			{
				ErrorMessages.Add(Strings.AuthErrorNeedAdo);
			}
		}
		if (_activeCredType == CredentialType.Id)
		{
			if (_credentialId == null)
			{
				ErrorMessages.Add(Strings.AuthErrorNeedId);
			}
		}
		else
		{
			if (_credentialName == null)
			{
				ErrorMessages.Add(Strings.AuthErrorNeedName);
			}
		}
		if (_password == null)
		{
			ErrorMessages.Add(Strings.AuthErrorNeedPass);
		}
		Trace.WriteLine($"#### AnyErrors {ErrorMessages.Count}");
		OnPropertyChanged(nameof(AnyErrors));
	}

	public bool AnyErrors => ErrorMessages.Count > 0;

	[GeneratedRegex(@"^https?://.+/.+")]
	private static partial Regex RegBaseUri();
}
