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
4. Configure the `App.config` file with your settings

### App.config

- IISLogRootPath
- IISSubFolders
- RouterIP
- RouterAddressListName
- RuleExpireTime
- SafeSubnets
- RouterUser
- RouterPass

## Versions

### V1.2.0 (October 2026)

- Config change for `IISSubFolders` to allow multiple folders to be scanned
- Added `SafeSubnets` to allow for multiple subnets to be excluded from blocking

### V1.1.0 (March 2026)

- Initial Version
