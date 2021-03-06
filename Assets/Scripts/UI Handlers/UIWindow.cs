using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow : MonoBehaviour
{
    public UIWindowID UIWindowID = UIWindowID.Empty;

    public GameObject LandscapeMainPanel = null;
    public GameObject PortraitMainPanel = null;

    public virtual void Start()
    {
        EventsManager.Instance.OnDeviceOrientationUpdate.AddListener((DeviceOrientation) =>
        {
            if (LandscapeMainPanel.activeSelf || PortraitMainPanel.activeSelf)
            {
                switch (GameManager.Instance.CurrentDeviceOrientation)
                {
                    case DeviceOrientation.Portrait:

                        LandscapeMainPanel.SetActive(false);
                        PortraitMainPanel.SetActive(true);
                        break;

                    case DeviceOrientation.PortraitUpsideDown:

                        LandscapeMainPanel.SetActive(false);
                        PortraitMainPanel.SetActive(true);
                        break;

                    case DeviceOrientation.LandscapeLeft:

                        PortraitMainPanel.SetActive(false);
                        LandscapeMainPanel.SetActive(true);
                        break;

                    case DeviceOrientation.LandscapeRight:

                        PortraitMainPanel.SetActive(false);
                        LandscapeMainPanel.SetActive(true);
                        break;
                }
            }         
        });
    }

    public virtual void OpenWindow()
    {
        switch (GameManager.Instance.CurrentDeviceOrientation)
        {
            case DeviceOrientation.Portrait:

                PortraitMainPanel.SetActive(true);
                break;

            case DeviceOrientation.PortraitUpsideDown:

                PortraitMainPanel.SetActive(true);
                break;

            case DeviceOrientation.LandscapeLeft:

                LandscapeMainPanel.SetActive(true);
                break;

            case DeviceOrientation.LandscapeRight:

                LandscapeMainPanel.SetActive(true);
                break;

            default:

                PortraitMainPanel.SetActive(true);
                break;
        }
    }

    public virtual void OnCloseWindowButton()
    {
        switch (UIWindowID)
        {
            case UIWindowID.UIWindowPause:
                GameManager.Instance.UpdateGameState(GameState.Running);
                break;
        }

        AudioManager.Instance.Play("Click");
        LandscapeMainPanel.SetActive(false);
        PortraitMainPanel.SetActive(false);
    }
}
