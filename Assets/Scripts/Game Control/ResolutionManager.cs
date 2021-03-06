using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public enum ScreenMode { WINDOWED = 0, FULLSCREEN, FULLSCREENWINDOW }
    readonly int defaultWidth = 1920;
    readonly int defaultHeight = 1080;
    float defaultRatio;
    int width = 1920;
    int height = 1080;
    Rect camRect = new Rect(0, 0, 1, 1);
    public Resolution[] SupportedResolutions { get; private set; }
    private FullScreenMode screenMode;


    #region Instance
    private static ResolutionManager instance;
    public static ResolutionManager Instance
    {
        get
        {
            var obj = FindObjectOfType<ResolutionManager>();
            instance = obj;
            return instance;
        }
    }
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        defaultRatio = (float)defaultWidth / (float)defaultHeight;

        #region for PC
        screenMode = Screen.fullScreenMode;
        SetSupportedResolutions();
        #endregion
    }
    #endregion

    private void Update()
    {
        SetResolution();
    }

    private void SetSupportedResolutions()
    {
        List<Resolution> resols = Screen.resolutions.ToList();
        Resolution res = resols[0];

        for(int i = 1; i< resols.Count; i++)
        {
            if (resols[i].width == res.width &&
               resols[i].height == res.height)
            {
                resols.RemoveAt(i);
                i--;
            }
            else
            {
                res = resols[i];
            }
        }

        SupportedResolutions = resols.ToArray();
    }

    public void SetScreenMode(int index)
    {
        switch((ScreenMode)index)
        {
            case ScreenMode.WINDOWED:
                screenMode = FullScreenMode.Windowed;
                break;
            case ScreenMode.FULLSCREEN:
                screenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case ScreenMode.FULLSCREENWINDOW:
                screenMode = FullScreenMode.FullScreenWindow;
                break;
        }
    }

    public void SetResolution(int index)
    {
        Screen.SetResolution(SupportedResolutions[index].width, SupportedResolutions[index].height, screenMode);
    }

    private void SetResolution()
    {
        if (Camera.main == null) return;

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        if (camRect != Camera.main.rect) Camera.main.rect = camRect;
        if (width == screenWidth && height == screenHeight) return;

        float ratio = (float)screenWidth / (float)screenHeight;

        if (ratio > defaultRatio) // 너비가 비율보다 큼
        {
            // 따라서 너비를 좀 잘라내야함.
            float widthRatio = ratio - defaultRatio; // 남는 부분 비율

            Camera.main.rect = new Rect(((ratio - defaultRatio) / ratio) / 2, 0, defaultRatio / ratio, 1);
        }
        else if (ratio < defaultRatio) // 높이가 비율보다 큼
        {
            // 따라서 높이를 좀 잘라내야함.
            float heightRatio = defaultRatio - ratio; // 남는 부분 비율

            Camera.main.rect = new Rect(0, ((defaultRatio - ratio) / defaultRatio) / 2, 1, ratio / defaultRatio);
        }
        else // 같음
        {
            Camera.main.rect = new Rect(0, 0, 1, 1);
        }

        camRect = Camera.main.rect;
        width = screenWidth;
        height = screenHeight;
    }

    public Vector2 AdjustPosWithRect(Vector2 pos)
    {
        float widthSpace = width * camRect.x;
        float heightSpace = height * camRect.y;
        float widthRect = width * camRect.width;
        float heightRect = height * camRect.height;

        Vector2 newPos = new Vector2((pos.x - widthSpace) / widthRect * defaultWidth, (pos.y - heightSpace) / heightRect * defaultHeight);

        return newPos;

    }
}
