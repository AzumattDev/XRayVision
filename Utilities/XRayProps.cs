using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XRayVision.Utilities;

public class XRayProps : MonoBehaviour
{
    public string m_text = "";
    public string m_topic = "";
    public Text textcomp;
    public Text titlecomp;
    public Image backgroundcomp;

    public void Awake()
    {
        UpdateTextElements();
        ApplyDefaults();
    }

    public void UpdateTextElements()
    {
        Transform child1 = Utils.FindChild(transform, "Text");
        if (child1 != null)
        {
            textcomp = child1.GetComponent<Text>();
            child1.GetComponent<Text>().text = Localization.instance.Localize(m_text);
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
        child2.GetComponent<Text>().text = Localization.instance.Localize(m_topic);
    }

    public void ApplyDefaults()
    {
        if (XRayVisionPlugin.ToolTipGameObject != null &&
            XRayVisionPlugin.ToolTipGameObject.TryGetComponent(out RectTransform transform))
        {
            transform.anchoredPosition = XRayVisionPlugin._toolTipPosition.Value;
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
        if (topic == m_topic && text == m_text)
            return;
        m_topic = topic;
        m_text = text;
        if (this == null)
            return;
        UpdateTextElements();
    }
}