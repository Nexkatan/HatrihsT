using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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


    public void TriggerNotification()
    {
        EnableNotification();
    }

    IEnumerator EnableNotification()
    {
        notificationAnim.Play("NotificationFadeIn");
        notificationTextUI.text = notificationMessage;
        notificationIconUI.sprite = yourIcon;

        if (disableAfterTimer)
        {
            yield return new WaitForSeconds(disableTimer);
            RemoveNotification();
        }
    }

    private void RemoveNotification()
    {
        notificationAnim.Play("NotificationFadeOut");
        gameObject.SetActive(false);
    }
}
