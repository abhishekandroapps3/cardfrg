using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserCredentialsManager : MonoBehaviour
{
    private const string UserIdKey = "UserId";
    private const string PasswordKey = "Password";
    private const string TokenKey = "Token";

    public static void SaveCredentials(string userId, string password, string token)
    {
        PlayerPrefs.SetString(UserIdKey, userId);
        PlayerPrefs.SetString(PasswordKey, password);
        PlayerPrefs.SetString(TokenKey, token);
        PlayerPrefs.Save();
    }

    public static void LoadCredentials(out string userId, out string password, out string token)
    {
        userId = PlayerPrefs.GetString(UserIdKey, "");
        password = PlayerPrefs.GetString(PasswordKey, "");
        token = PlayerPrefs.GetString(TokenKey, "");
    }

    public static void ClearCredentials()
    {
        PlayerPrefs.DeleteKey(UserIdKey);
        PlayerPrefs.DeleteKey(PasswordKey);
        PlayerPrefs.DeleteKey(TokenKey);
        PlayerPrefs.Save();
    }
}
