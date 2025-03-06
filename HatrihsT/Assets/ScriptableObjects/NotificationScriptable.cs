
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NotificationSc", menuName = "Notifications/NotificationScriptable")]
public class NotificationScriptable : ScriptableObject
{
    [Header("Message Customisation")]
    public Sprite yourIcon;
    [TextArea] public string notificationMessage;

    [Header("Notification Removal")]
    public bool disableAfterTimer = false;
    public float disableTimer = 1.0f;
}
