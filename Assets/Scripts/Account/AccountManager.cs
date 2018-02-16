using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountManager : Singleton<AccountManager> {

	//Needed to prevent non singleton constructor calls.
	protected AccountManager() { }

	public LoginInfo loginInfo;

	public bool LoggedIn {
		get {
			if (loginInfo == null)
				return false;

			if (loginInfo.uid > 0 && !string.IsNullOrEmpty (loginInfo.username)) {
				return true;
			} else {
				return false;
			}
		}
	}

	// User id
	public int Uid {
		get {
			if (LoggedIn) {
				return loginInfo.uid;
			} else {
				Debug.Log ("Cannot get uid. Not logged in.");
				return 0;
			}
		}
	}

	public string Country {
		get {
			if (LoggedIn) {
				return loginInfo.country;
			} else {
				Debug.Log ("Cannot get country. Not logged in.");
				return "";
			}
		}
	}

	public string Username {
		get {
			if (LoggedIn) {
				return loginInfo.username;
			} else {
				Debug.Log ("Cannot get username. No username. Not logged in.");
				return "";
			}
		}
	}

	public void ShowLoginDialog () {
		AccountUI accountUI = FindObjectOfType<AccountUI> ();

		if (accountUI == null) {
			accountUI = Instantiate(Resources.Load ("AccountPopup", typeof(AccountUI))) as AccountUI;
			accountUI.SwitchMode (AccountUI.AccountUIMode.Login);
		} else {
			// Account UI already exists, make sure it's shown
			accountUI.gameObject.SetActive(true);
		}
	}

	public void CreateAccount (string username, string pwd, string pwd2, string email, System.Action<LoginInfo> loginInfo) {
		StartCoroutine (_CreateAccount(username, pwd, pwd2, email, _loginInfo => {
			loginInfo(_loginInfo);
		}));
	}

	private IEnumerator _CreateAccount (string username, string pwd, string pwd2, string email, System.Action<LoginInfo> loginInfo) {
		WWWForm form = new WWWForm();
		form.AddField("username", username);
		form.AddField("pwd", pwd);
		form.AddField("pwd2", pwd2);
		form.AddField("email", email);

		WWW www = new WWW(Url.WEBSITE+Url.CREATE_ACCOUNT, form);

		yield return www;

		if (www.error == null) {
			try {
				loginInfo(JsonUtility.FromJson<LoginInfo>(www.text));
			} catch (System.Exception exception) {
				Debug.LogError(exception);
				loginInfo(null);
			}
		} else {
			Debug.LogError(www.error);
			loginInfo(null);
		}
	}

	/// <summary>
	/// Get the login info 
	/// </summary>
	/// <param name="username">Username.</param>
	/// <param name="pwd">Password.</param>
	/// <param name="loginInfo">The returned login info.</param>
	public void Login (string username, string pwd, System.Action<LoginInfo> loginInfo) {
		StartCoroutine (_Login(username, pwd, _loginInfo => {
			loginInfo(_loginInfo);
		}));
	}

	private IEnumerator _Login (string username, string pwd, System.Action<LoginInfo> loginInfo) {
		WWWForm form = new WWWForm();
		form.AddField("name", username);
		form.AddField("pwd", pwd);

		WWW www = new WWW(Url.WEBSITE+Url.LOGIN, form);

		yield return www;

		if (www.error == null) {
			LoginInfo _loginInfo = new LoginInfo();
			try {
				_loginInfo = JsonUtility.FromJson<LoginInfo>(www.text);
				loginInfo(_loginInfo);
			} catch (System.Exception exception) {
				Debug.LogError(exception);
				loginInfo(null);
			}
		} else {
			Debug.LogError(www.error);
			loginInfo(null);
		}
	}

	// Pings master server to determine whether or not device is connected to the Internet
	private IEnumerator _CheckConnectionToMasterServer(System.Action<bool> connected) {
		Ping pingMasterServer = new Ping("67.225.180.24");
		float startTime = Time.time;
		while (!pingMasterServer.isDone && Time.time < startTime + 5.0f) {
			yield return new WaitForSeconds(0.1f);
		}
		if(pingMasterServer.isDone) {
			connected(true);
		} else {
			connected(false);
		}
	}

	public void CheckConnectionToMasterServer(System.Action<bool> connected) {
		StartCoroutine(_CheckConnectionToMasterServer(_connected => {
			connected(_connected);
		}));
	}


}
