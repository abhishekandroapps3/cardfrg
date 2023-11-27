
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace DemiRegister
{
    [System.Serializable]
    public class RegisterDetails
    {

        public InputField userNameInput;
        public InputField mobileInput;
        public InputField passwordInput;
        public InputField referralCodeInput;
        public Toggle agreeTermsToggle;
        public Toggle maleToggle;
        public Toggle femaleToggle;
        public Button registerButton;
        public Button loginButton;
        public Button registerPAgeButton;
        public GameObject registrationPanel;
        public GameObject loginPanel;
        public GameObject otpVerificationPanel;
        public GameObject homePanel;
        public Text errorText;
    }


    public class Register : MonoBehaviour
    {
        public RegisterDetails RegisterDetail;


        public void Start()
        {
            RegisterDetail.registerButton.onClick.AddListener(RegisterUser);
            RegisterDetail.loginButton.onClick.AddListener(GoToLoginPage); // Register the login button click event
            RegisterDetail.registerPAgeButton.onClick.AddListener(GoToRegisterPage);
        }

        private void RegisterUser()
        {
            string userName = RegisterDetail.userNameInput.text;
            string mobile = RegisterDetail.mobileInput.text;
            string password = RegisterDetail.passwordInput.text;
            string referralCode = RegisterDetail.referralCodeInput.text;


            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(referralCode))
            {
                SetErrorText("Please fill in all fields.");
                return;
            }

            // Check if terms are agreed

            if (!RegisterDetail.agreeTermsToggle.isOn)
            {
                SetErrorText("Please agree to the terms and conditions.");
                return;
            }
            // Check mobile number length
            if (mobile.Length != 10)
            {
                SetErrorText("Mobile number must be 10 digits.");
                return;
            }

            // Check password length
            if (password.Length < 6)
            {
                SetErrorText("Password must be at least 6 characters.");
                return;
            }

            // Check which toggle is selected
            if (RegisterDetail.maleToggle.isOn)
            {
                // Male is selected
                Debug.Log("Male is selected");
            }
            else if (RegisterDetail.femaleToggle.isOn)
            {
                // Female is selected
                Debug.Log("Female is selected");
            }
            else
            {
                // No toggle is selected
                SetErrorText("Please select your gender.");
                return;
            }
            StartCoroutine(RegisterUserRequest(userName, mobile, password, referralCode));
        }

        IEnumerator RegisterUserRequest(string userName, string mobile, string password, string referralCode)
        {
            WWWForm form = new WWWForm();
            form.AddField("userName", userName);
            form.AddField("mobile", mobile);
            form.AddField("password", password);
            form.AddField("referralCode", referralCode);
            string url = Configuration.Api + Configuration.UserRegister;
            using (var www = UnityWebRequest.Post(url, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Network error: " + www.error);
                    RegisterResponse.Invoke(false, "Network error. Please try again.");
                }
                else
                {
                    string responseText = www.downloadHandler.text;

                    // Parse the JSON response
                    RegisterResponse registerResponse = JsonUtility.FromJson<RegisterResponse>(responseText);

                    if (registerResponse != null && registerResponse.success)
                    {
                        Debug.Log("Registration successful!");

                        // Store user data in PlayerPrefs
                        PlayerPrefs.SetString("UserName", registerResponse.userName);
                        PlayerPrefs.SetString("AuthToken", registerResponse.authToken);

                        onRegisterComplete.Invoke(true, "");
                    }
                    else
                    {
                        Debug.Log("Registration failed. Message: " + registerResponse?.message);
                        onRegisterComplete?.Invoke(false, "Registration failed. " + registerResponse?.message);
                    }
                }
            }
        }

        IEnumerator LoginUserRequest(string mobile, string password)
        {
            WWWForm form = new WWWForm();
            form.AddField("mobile", mobile);
            form.AddField("password", password);
            string url = Configuration.Api + Configuration.UserLogin;

            using (var www = UnityWebRequest.Post(url, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Login Error: " + www.error);
                    //   SetErrorText("Login failed. Please try again.");
                }
                else
                {
                    // Login successful, proceed to the homepage
                    LoadHomePage();
                }
            }
        }

        public class RegisterResponse
        {
            public bool success;
            public string userName;
            public string authToken;
            public string message;
        }
            void LoadHomePage()
        {
            RegisterDetail.homePanel.SetActive(true);
            RegisterDetail.otpVerificationPanel.SetActive(false);
            // Add code to load your homepage scene or perform actions after login
            Debug.Log("Login successful! Loading homepage...");
        }

        void SetErrorText(string message)
        {
            RegisterDetail.errorText.text = message;
            RegisterDetail.errorText.gameObject.SetActive(true);
        }
        public void GoToLoginPage()
        {
            RegisterDetail.loginPanel.SetActive(true);
            RegisterDetail.registrationPanel.SetActive(false);
            // Add code to switch to the login panel or load the login scene
            Debug.Log("Going to the login page...");

        }
        void GoToRegisterPage()
        {
            RegisterDetail.registrationPanel.SetActive(true);
            RegisterDetail.loginPanel.SetActive(false);
            // Add code to switch to the login panel or load the login scene
            Debug.Log("Going to the register page...");

        }
    }
}