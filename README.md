# Tailscale Client
A modern, WinUI3-based Tailscale client for Windows.

This project was born out of wanting to mess around with WinUI3, and the fact that the Tailscale Windows client looks... not great compared to MacOS.
It aims to provide a modern, clean, and feature-rich Tailscale client for Windows while being fully open source, unlike the official one.

> [!IMPORTANT]
> Not all features (even somewhat basic ones!) are implemented at this time. This is a work in progress.

> [!WARNING]
> This does not replace the Tailscale daemon. This is just the client UI meant to replace/integrate the Tailscale tray icon app.

## Features
- [x] Account switching
- [x] Full login flow, supporting custom control servers such as headscale instances
- [x] QR code logins
- [x] Netmap view (devices page)
- [x] Device details and copy (such as IP, domain, etc.)
- [ ] Exit nodes
- [ ] Settings (Run unattended, allow Tailscale DNS, accept subnets)
- [ ] Taildrop

### Preview

<p style="display:flex;justify-content:center;column-gap:8%;">
  <img style="text-align:center" src="https://raw.githubusercontent.com/uwu/tailscale-client/refs/heads/main/Publishing/login.png" width="30%" />
  <img style="text-alignt:center" src="https://raw.githubusercontent.com/uwu/tailscale-client/refs/heads/main/Publishing/preview.png" width="30%" />
</p>

## Installation

### Method 1.
Grab the installer from the [CDN](https://tsc.xirreal.dev/TailscaleClientInstaller.exe) or from the [latest release](https://github.com/uwu/tailscale-client/releases/latest).
This is an automated installer that will:
1) Download the certificate and install it
2) Download the .appinstaller and run it.

It should be the easiest and most seamless way to install Tailscale Client, giving you access to automatic updates.

> [!IMPORTANT]
> This method requires UAC (admin rights) to install the certificate.

### Method 2.
Download the appinstaller from the [latest release](https://github.com/uwu/tailscale-client/releases/latest), along with the certificate.
This is the manual version of the method explained above: you will need to install the certificate in LocalMachine/TrustedPeople and run the appinstaller.
This version will still work with automatic updates.

### Method 3.
Download the msixbundle from from the [latest release](https://github.com/uwu/tailscale-client/releases/latest).
This version does NOT support automatic updates.

## Building and running
0. Prerequisites
   - Visual Studio 2022
   - Windows 10 21H1 or later
   - Windows SDK 22000.194 or later
   - Windows App SDK 1.6
   - .NET 8.0 SDK
1. Clone the repository
2. Open the solution in Visual Studio 2022
3. Build the solution and run
4. To install manually, you will need to build using "Project > Package and Publish > Create App Packages" and following the wizard.

> [!WARNING]
> To install manually, you will need to sideload the msix package. You MUST install the certificate you used to sign the package on your device, or it will not install.

## Contributing
Contributions are welcome! Please open an issue or pull request if you have any suggestions or changes.

## License
This project is licensed under the MIT License. See the `LICENSE` file for more information.

## Acknowledgements
- [Tailscale](https://tailscale.com), for making Tailscale open-source
- [QRCoder](https://github.com/codebude/QRCoder), used to generate the QRs for login
