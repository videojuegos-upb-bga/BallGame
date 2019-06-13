using UnityEngine;
using UnityEngine.UI;
using SA.Productivity.Console;

public class UCL_NativeUseExample : MonoBehaviour
{
    [SerializeField] Button m_TestButton = null;
    // Start is called before the first frame update
    void Start()
    {
        UCL_Logger.Init();
        Debug.Log("App star");

        m_TestButton.onClick.AddListener(UCL_Logger.ShowSharingUI);
    }

   
}
