using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class NotificationTriggerEvent : MonoBehaviour
{
    [Header("UI Content")]
    [SerializeField] private TextMeshProUGUI notificationTextUI;
    [SerializeField] private Image notificationIconUI;

    [Header("Message Customisation")]
    [SerializeField] private Sprite yourIcon;
    [SerializeField][TextArea] private string notificationMessage;

    [Header("Notification Removal")]
    [SerializeField] private bool disableAfterTimer = false;
    [SerializeField] float disableTimer = 1.0f;

    [Header("Notification Animation")]
    [SerializeField] private Animator notificationAnim;

    public ObjectiveProgressItem progressItem;

    public void TriggerNotification(NotificationScriptable notification)
    {
        StartCoroutine(EnableNotification(notification));
    }

    IEnumerator EnableNotification(NotificationScriptable notification)
    {
        notificationAnim.Play("NotificationFadeIn");
        notificationTextUI.text = notification.notificationMessage;
        notificationIconUI.sprite = notification.yourIcon;

        progressItem.progressText.text = notification.progressItemText;

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
