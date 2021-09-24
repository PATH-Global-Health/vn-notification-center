Listen at hub: /notificationHub<br>

Listen New Notification at: newNotify<br>
Listen New Notification Count at: newNotifyCount<br>

Queue to add new notification throw rabbitMQ: NewNotification<br>
NewNotification require:<br>
-string Action<br>
-string Description<br>
-Guid UserId<br>
