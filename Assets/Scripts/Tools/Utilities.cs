using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {
	public const string DATE_FORMAT = "dd/MM/yyyy";

	public static DateTime UnixTimeStampToDateTime (int unixTimeStamp) {
		// Unix timestamp is seconds past epoch
		System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddSeconds((double)unixTimeStamp).ToLocalTime();
		return dtDateTime;
	}

	public static bool IsValidEmailFormat (string email) {
		// Taken from https://stackoverflow.com/questions/1365407/c-sharp-code-to-validate-email-address
		try {
			var addr = new System.Net.Mail.MailAddress(email);
			return addr.Address == email;
		} catch {
			return false;
		}
	}
}
