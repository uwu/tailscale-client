# Tailscale Client
A modern, WinUI3-based Tailscale client for Windows.

This project was born out of wanting to mess around with WinUI3, and the fact that the Tailscale Windows client looks not great compared to MacOS.
This project aims to provide a modern, clean, and feature-rich Tailscale client for Windows while being fully open source, unlike the official one.

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

## Building and running
0. Prerequisites
   - Visual Studio 2022
   - Windows 10 21H1 or later
   - Windows SDK 22000.194 or later
   - Windows App SDK 1.3
   - .NET 8.0 SDK
1. Clone the repository
2. Open the solution in Visual Studio 2022
3. Build the solution and run

## Contributing
Contributions are welcome! Please open an issue or pull request if you have any suggestions or changes.

## License
This project is licensed under the MIT License. See the `LICENSE` file for more information.

## Acknowledgements
- [Tailscale](https://tailscale.com), for making Tailscale open-source
- [QRCoder](https://github.com/codebude/QRCoder), used to generate the QRs for login