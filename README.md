# ![Logo](./mikrotik-iisautoblocker.png) TheOwls - MikroTik Firewall IIS AutoBlocker `MikroTik-IISAutoBlocker`

Windows Service to monitor IIS logs for webscanners, then add a block rule to a MikroTik firewall

## Features

- Scan Multiple IIS Sites
- Blocks anything HTTP Status 400 <> 500
- Customiseable Block time length
- Will Block subnet, not just the IP
- Can exclude local subnet
- Runs as a Windows Service

## Instructions

### Install Service

1. Download latest release
2. extract zip to local folder
3. Install service  
   ```powershell
   New-Service -Name "MikroTik-IISAutoBlocker" -BinaryPathName "C:\Path\To\MikroTik-IISAutoBlocker.exe"
   ```

### App.config

- IISLogRootPath
- IISSubFolder
- RouterIP
- RouterAddressListName
- RuleExpireTime
- SafeSubnet
- RouterUser
- RouterPass

## Versions

### V1.1.0 (March 2026)

- Initial Version
