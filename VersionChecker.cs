using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Qitz.DataUtil;
using LitJson;

public class SeverModel
{
    public string type;
    public string version;
    public string url;
    public string msg;

}


public class VersionChecker : MonoBehaviour
{
    public const string version_url = "https://docs.google.com/spreadsheets/d/176aDNjy12qck3mV-SwlWkANSYG0mK3miKUBSDv3b09U/edit#gid=0";

    private string _urlformarket;

    public GameObject _downloadPopup;
    public Button _updateButton;
    public Text vertext;
    public bool updateOnPlay;
    public Dictionary<string, SeverModel> version = new Dictionary<string, SeverModel>();

    void Start()
    { 
        vertext.text = "Version " + Application.version;
        _downloadPopup.SetActive(false);

        Application.runInBackground = true;

        StartCoroutine(SheetLoadJson(version_url, () => { getVersion(); }));

    }

    public IEnumerator SheetLoadJson(string url, UnityAction _action = null)
    {
        yield return JsonFromGoogleSpreadSheet.GetJsonArrayFromGoogleSpreadSheetUrl(url, (jsonArray) =>
        {
            foreach (var json in jsonArray)
            {
                SeverModel severModel = JsonMapper.ToObject<SeverModel>(json.ToString());

                version.Add(severModel.type, severModel);
            }
        });

        if (_action != null)
        {
            
            _action();
        }

        yield return null;
    }

    void getVersion()
    {
        string live = "";
        string marketUrl = "";
        string msg = "";
        
#if UNITY_ANDROID
        live = version["Live"].version;
        marketUrl = version["Live"].url;
        msg = version["Live"].msg;
#elif UNITY_IOS
        live = version["IOS"].version;
        marketUrl = version["IOS"].url;
        msg = version["IOS"].msg;
#elif UNITY_STANDALONE
        live = version["PC"].version;
        marketUrl = version["PC"].url;
        msg = version["PC"].msg;
#endif
   
        if (Application.version != live)
        {
            Debug.LogError("게임 버전이 다릅니다.");
            _downloadPopup.SetActive(true);
            _urlformarket = marketUrl;
        }
    }

    public void OnClickUpdateButton()
    {
        Debug.LogError("업데이트 진행");
        Application.OpenURL(_urlformarket);
        Application.Quit();
    }
}
