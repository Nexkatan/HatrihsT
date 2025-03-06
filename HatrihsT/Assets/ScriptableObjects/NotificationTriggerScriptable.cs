using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationTriggerScriptable : MonoBehaviour
{

    [Header("UI Content")]
    [SerializeField] private TextMeshProUGUI notificationTextUI;
    [SerializeField] private Image notificationIconUI;

    [Header("ScriptableObject")]
    [SerializeField] private NotificationScriptable noteScriptable;

    [Header("Notification Animation")]
    [SerializeField] private Animator notificationAnim;

    public List<NotificationScriptable> objectives = new List<NotificationScriptable>();

    public void TriggerNotification(NotificationScriptable notification)
    {
        StartCoroutine(EnableNotification(notification));
    }

    IEnumerator EnableNotification(NotificationScriptable notification)
    {
        notificationAnim.Play("NotificationFadeIn");
        notificationTextUI.text = notification.notificationMessage;
        notificationIconUI.sprite = notification.yourIcon;

        if (notification.disableAfterTimer)
        {
            yield return new WaitForSeconds(notification.disableTimer);
            RemoveNotification();
        }
    }

    public void RemoveNotification()
    {
        notificationAnim.Play("NotificationFadeOut"); 
        notificationTextUI.text = "";
        notificationIconUI.sprite = null;
    }

}
