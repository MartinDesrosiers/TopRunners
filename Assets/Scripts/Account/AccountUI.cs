using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountUI : MonoBehaviour {

	[SerializeField] private InputField createUsernameInputField;
	[SerializeField] private InputField createPasswordInputField;
	[SerializeField] private InputField createPassword2InputField;
	[SerializeField] private InputField createEmailInputField;
	[SerializeField] private InputField loginUsernameInputField;
	[SerializeField] private InputField loginPasswordInputField;
	[SerializeField] private Button closeBtn;
	[SerializeField] private GameObject loginPanel;
	[SerializeField] private GameObject createPanel;

	public enum AccountUIMode {
		Login,
		Create
	}

	void Awake () {
		closeBtn.onClick.AddListener (() => {
			Destroy(this.gameObject);
		});
	}

	public void SwitchMode (AccountUIMode accountUIMode) {
		Debug.Log ("Switch UI mode");
	}

	private void ClearFields (AccountUIMode accountUIMode) {
		if (accountUIMode == AccountUIMode.Create) {
			createUsernameInputField.text = "";
			createPasswordInputField.text = "";
			createPassword2InputField.text = "";
			createEmailInputField.text = "";
		} else { // Login
			loginUsernameInputField.text = "";
			loginPasswordInputField.text = "";
		}
	}

	public void OnCreateAccount () {
		AccountManager.Instance.CreateAccount (
			createUsernameInputField.text,
			createPasswordInputField.text,
			createPassword2InputField.text,
			createEmailInputField.text,
			loginInfo => {
				Debug.Log(string.Format("Error code {0}: {1}", loginInfo.errorCode, loginInfo.errorMessage));
				if (loginInfo.errorCode == 1) {
					// Account creation successful
					AccountManager.Instance.loginInfo = loginInfo;
					Destroy(this.gameObject);
				} else {
					// Account creation failed
					createPasswordInputField.text = "";
					createPassword2InputField.text = "";
				}
			}
		);
	}

	public void OnLogin () {
		AccountManager.Instance.Login (
			loginUsernameInputField.text,
			loginPasswordInputField.text,
			loginInfo => {
				Debug.Log(string.Format("Error code {0}: {1}", loginInfo.errorCode, loginInfo.errorMessage));
				if (loginInfo.errorCode == 1) {
					// Successful login
					AccountManager.Instance.loginInfo = loginInfo;
					Destroy(this.gameObject);
				} else {
					// Login failed
					loginPasswordInputField.text = "";
				}
			}
		);
	}
}
