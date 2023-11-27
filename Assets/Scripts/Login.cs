using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField mobileInput;
    public InputField passwordInput;
    public Toggle ageCheckbox;
    public Button loginButton;
    public Button registerButton;
    public GameObject registrationPanel;
    public GameObject loginPanel;
    public Text errorText;


    private void Start()
    {
        loginButton.onClick.AddListener(LoginPage);
        registerButton.onClick.AddListener(GoToRegisterPanel);
    }

    public void LoginPage()
    {
        // Check if the "I am 18 years of age" checkbox is checked
        if (!ageCheckbox.isOn)
        {
            DisplayError("You must be 18 years of age to log in.");
            return;
        }

        // Validate mobile number and password
        string mobile = mobileInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(password))
        {
            DisplayError("Please enter both mobile number and password.");
            return;
        }

        // Make API request to authenticate user
        StartCoroutine(LoginUser(mobile, password));
    }

    public void GoToRegisterPanel()
    {
        // Implement navigation to the register panel here
        // For this example, let's assume the register panel is in another scene
        registrationPanel.SetActive(true);
        loginPanel.SetActive(false);


            }

    private void DisplayError(string message)
    {
        errorText.text = message;
    }

    private void HideError()
    {
        errorText.text = "";
    }

    private IEnumerator LoginUser(string mobile, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("mobile", mobile);
        form.AddField("password", password);
        string url = Configuration.Api + Configuration.UserLogin;

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Network error: " + www.error);
                DisplayError("Network error. Please try again.");
            }
            else
            {
                string responseText = www.downloadHandler.text;

                // Parse the JSON response
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);

                if (loginResponse != null && loginResponse.success)
                {
                    Debug.Log("Login successful!");

                    // Check for additional information in the API response if needed
                    // For example, you might receive a user token

                    // Load the home scene or perform other actions after successful login
                    SceneManager.LoadScene("HomeScene");
                }
                else
                {
                    Debug.Log("Login failed. Invalid credentials.");
                    DisplayError("Login failed. Invalid credentials.");
                }
            }
        }
    }

    [System.Serializable]
    public class LoginResponse
    {
        public bool success;
        // You can add more fields based on your API response
    }
}