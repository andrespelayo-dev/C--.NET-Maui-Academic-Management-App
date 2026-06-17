using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace C971project.Services;

public class NotificationPermissionService
{
    public async Task RequestAsync()
    {
#if ANDROID
        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
        if (status != PermissionStatus.Granted)
        {
            await Permissions.RequestAsync<Permissions.PostNotifications>();
        }
#endif
    }
}
