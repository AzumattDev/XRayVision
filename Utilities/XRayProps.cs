using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XRayVision.Utilities;

public class XRayProps : MonoBehaviour
{
    public string mText = "";
    public string mTopic = "";
    public Text textcomp = null!;
    public Text titlecomp = null!;
    public Image backgroundcomp = null!;

    public void Awake()
    {
        UpdateTextElements();
    }

    public void Start()
    {
        ApplyDefaults();
    }

    public void UpdateTextElements()
    {
        Transform child1 = Utils.FindChild(transform, "Text");
        if (child1 != null)
        {
            textcomp = child1.GetComponent<Text>();
            child1.GetComponent<Text>().text = Localization.instance.Localize(mText);
        }

        Transform bkg = Utils.FindChild(transform, "Bkg");
        if (bkg != null)
        {
            backgroundcomp = bkg.GetComponent<Image>();
        }

        Transform child2 = Utils.FindChild(transform, "Topic");
        if (child2 == null)
            return;
        titlecomp = child2.GetComponent<Text>();
        child2.GetComponent<Text>().text = Localization.instance.Localize(mTopic);
    }

    public void ApplyDefaults()
    {
        XRayVisionPlugin.XRayLogger.LogDebug("Applying defaults");
        if (XRayVisionPlugin.ToolTipGameObject != null &&
            XRayVisionPlugin.ToolTipGameObject.TryGetComponent(out RectTransform transform))
        {
            transform.anchoredPosition = XRayVisionPlugin.ToolTipPosition.Value;
        }


        if (textcomp)
        {
            textcomp.fontSize = XRayVisionPlugin.ToolTipTextSize.Value;
        }

        if (titlecomp)
        {
            titlecomp.fontSize = XRayVisionPlugin.ToolTipTitleSize.Value;
        }

        if (backgroundcomp)
        {
            backgroundcomp.color = XRayVisionPlugin.ToolTipBkgColor.Value;
            // backgroundcomp.sprite = ;
        }
    }


    public void HideTooltip()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public void ShowTooltip()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public void Set(string topic, string text)
    {
        if (topic == mTopic && text == mText)
            return;
        mTopic = topic;
        mText = text;
        if (this == null)
            return;
        UpdateTextElements();
    }
}