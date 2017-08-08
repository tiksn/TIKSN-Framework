using Microsoft.Extensions.Localization;
using System;
using TIKSN.Localization;

namespace TIKSN.Shell
{
	public abstract class ShellAttributeBase : Attribute
	{
		private readonly int? _integerNameKey;
		private readonly string _stringNameKey;

		protected ShellAttributeBase(int nameKey)
		{
			_integerNameKey = nameKey;
		}

		protected ShellAttributeBase(string nameKey)
		{
			_stringNameKey = nameKey;
		}

		public string GetName(IStringLocalizer stringLocalizer)
		{
			if (_integerNameKey.HasValue)
				return stringLocalizer.GetRequiredString(_integerNameKey.Value);
			else if (Guid.TryParse(_stringNameKey, out Guid key))
				return stringLocalizer.GetRequiredString(key);
			else
				return stringLocalizer.GetRequiredString(_stringNameKey);
		}
	}
}
