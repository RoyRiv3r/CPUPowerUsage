# CPUPowerUsage WIP

CPUPowerUsage is a lightweight Windows application designed to monitor and display CPU usage and power consumption in real-time. Utilizing the OpenHardwareMonitor library, it provides users with immediate feedback on their CPU's performance through a simple yet informative system tray icon.

## Features

- **Real-Time CPU Monitoring**: Tracks CPU usage and power consumption (in Watts) and updates the information every 1.5 seconds.
- **System Tray Icon**: Displays the current CPU power consumption directly in the system tray for easy access.
- **Customizable Startup**: Offers the option to automatically start with Windows, ensuring you're always keeping an eye on your CPU's performance from the moment your PC boots up.

## Screenshot

![image](https://github.com/RoyRiv3r/CPUPowerUsage/assets/41067116/b27a208d-4fdd-4965-bdeb-e2ab4e4de37b)


## Dependencies

- [OpenHardwareMonitor](https://github.com/openhardwaremonitor/openhardwaremonitor): For accessing CPU usage and power consumption data.
- [Microsoft.Win32.TaskScheduler](https://github.com/dahall/taskscheduler): For managing application startup with Windows.


## Usage

Right-click the system tray icon to access the context menu, where you can enable/disable startup with Windows or exit the application. Hover over the icon at any time to view the current CPU usage and power consumption.

## Info 

This app requires admin privilege to run due to library limitation otherwise you won't be able to see the CPU power consumption. 

Tested on AMD Ryzen CPU. 

## Todo 

- Swap the usage and the power display button for the tray icon
- Create a window graph of the power/usage/temp and logs
- Add CPU temp/cores/info
- Add alerts
- Custom colors
- Test different CPU compatibility
- ...

## License

This project is licensed under the MIT License
