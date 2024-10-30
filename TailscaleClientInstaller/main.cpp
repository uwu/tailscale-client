#include <Urlmon.h>
#include <shellscalingapi.h>
#include <shlobj.h>
#include <tchar.h>
#include <winuser.h>

#include <algorithm>
#include <iostream>
#include <string>
#include <vector>

#pragma comment(lib, "user32.lib")
#pragma comment(lib, "Shcore.lib")
#pragma comment(lib, "Shlwapi.lib")
#pragma comment(lib, "urlmon.lib")
#pragma comment(lib, "crypt32.lib")

constexpr LPCWSTR remoteUrl = L"https://tsc.xirreal.dev/";

static void ShowMessage(const std::wstring& message, UINT type) {
  SetProcessDpiAwareness(PROCESS_PER_MONITOR_DPI_AWARE);
  ShellMessageBoxW(NULL, NULL, message.c_str(), L"Tailscale Client Installer",
                   MB_OK | type);
}

static std::wstring GetErrorMessage(DWORD errorCode) {
  LPVOID messageBuffer = nullptr;
  DWORD formatFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER |
                      FORMAT_MESSAGE_FROM_SYSTEM |
                      FORMAT_MESSAGE_IGNORE_INSERTS;
  DWORD languageId = 0;
  std::wstring errorMessage;

  if (FormatMessageW(formatFlags, NULL, errorCode, languageId,
                     (LPWSTR)&messageBuffer, 0, NULL)) {
    errorMessage = (LPWSTR)messageBuffer;
    LocalFree(messageBuffer);
  } else {
    errorMessage = L"Unknown error.";
  }
  return errorMessage;
}

static bool DownloadFile(LPCWSTR resource, const std::wstring& path) {
  HRESULT hr = URLDownloadToFile(NULL, resource, path.c_str(), 0, NULL);
  if (FAILED(hr)) {
    std::wcerr << L"[x] Error downloading file: " << GetErrorMessage(hr)
               << std::endl;
    ShowMessage(
        L"Download failed.\n\nCheck your internet connection or try the "
        L"offline installer instead.\n",
        MB_ICONERROR);
    return false;
  }
  return true;
}

static bool CreateTempFolder(std::wstring& tempFolder) {
  wchar_t tempPath[MAX_PATH];
  GetTempPath(MAX_PATH, tempPath);

  tempFolder = tempPath;
  tempFolder += L"TailscaleClientInstaller";

  DWORD attributes = GetFileAttributes(tempFolder.c_str());
  if (attributes != INVALID_FILE_ATTRIBUTES) {
    std::wcout << L"[!] Temporary folder already exists. Cleaning up..."
               << std::endl;
    // Create double null-terminated string
    wchar_t* tempFolderDoubleNull = new wchar_t[tempFolder.size() + 2];
    std::copy(tempFolder.begin(), tempFolder.end(), tempFolderDoubleNull);
    tempFolderDoubleNull[tempFolder.size()] = L'\0';
    tempFolderDoubleNull[tempFolder.size() + 1] = L'\0';
    // Delete recursively
    SHFILEOPSTRUCT fileOp = {0};
    fileOp.wFunc = FO_DELETE;
    fileOp.pFrom = tempFolderDoubleNull;
    fileOp.fFlags = FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_SILENT;
    if (SHFileOperation(&fileOp)) {
      std::wcerr << L"[x] Failed to delete temporary folder." << std::endl;
    }
    delete[] tempFolderDoubleNull;
  }
  return CreateDirectory(tempFolder.c_str(), NULL);
}

static bool InstallCertificate(HCERTSTORE certificateStore,
                               PCCERT_CONTEXT certificateContext) {
  if (!CertAddCertificateContextToStore(certificateStore, certificateContext,
                                        CERT_STORE_ADD_REPLACE_EXISTING,
                                        NULL)) {
    std::cerr << "[x] Failed to add the certificate to the store." << std::endl;
    ShowMessage(
        L"Failed to install the certificate.\n\nPlease report this issue to "
        L"the developer.\n",
        MB_ICONERROR);
    return false;
  }
  std::cout << "[-] Certificate successfully installed." << std::endl;
  return true;
}

static bool SetupCertificate(HCERTSTORE& certificateStore,
                             PCCERT_CONTEXT& certificateContext,
                             const std::wstring& certPath) {
  certificateStore =
      CertOpenStore(CERT_STORE_PROV_SYSTEM, 0, NULL,
                    CERT_SYSTEM_STORE_LOCAL_MACHINE, L"TrustedPeople");
  if (!certificateStore) {
    std::cerr << "[x] Failed to open Trusted People store." << std::endl;
    ShowMessage(
        L"This installer can only run as administrator.\n\nPlease restart the "
        L"installer using administrator permissions.",
        MB_ICONWARNING);
    return false;
  }

  // Attempt to load the certificate from file
  HANDLE hFile = CreateFile(certPath.c_str(), GENERIC_READ, FILE_SHARE_READ,
                            NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
  if (hFile == INVALID_HANDLE_VALUE) {
    ShowMessage(L"Certificate file missing or inaccessible.", MB_ICONERROR);
    return false;
  }

  DWORD fileSize = GetFileSize(hFile, NULL);
  BYTE* fileBuffer = new BYTE[fileSize];
  DWORD bytesRead = 0;

  if (!ReadFile(hFile, fileBuffer, fileSize, &bytesRead, NULL)) {
    ShowMessage(L"Failed to read certificate file.", MB_ICONERROR);
    delete[] fileBuffer;
    CloseHandle(hFile);
    return false;
  }
  certificateContext = CertCreateCertificateContext(
      X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, fileBuffer, fileSize);
  delete[] fileBuffer;
  CloseHandle(hFile);

  if (!certificateContext) {
    ShowMessage(L"Failed to parse the certificate.\n\nPlease contact support.",
                MB_ICONERROR);
    return false;
  }

  if (!InstallCertificate(certificateStore, certificateContext)) {
    CertFreeCertificateContext(certificateContext);
    CertCloseStore(certificateStore, CERT_CLOSE_STORE_FORCE_FLAG);
    return false;
  }

  return true;
}

int APIENTRY wWinMain(HINSTANCE instance, HINSTANCE previousInstance,
                      LPWSTR args, int shouldShowCmd) {
#ifdef _DEBUG
  AllocConsole();
  AttachConsole(GetCurrentProcessId());
  FILE *pConsoleOut, *pConsoleErr;
  freopen_s(&pConsoleOut, "CONOUT$", "w", stdout);
  freopen_s(&pConsoleErr, "CONOUT$", "w", stderr);
#endif

  std::cout << ">> Tailscale Client Installer - (c) 2024 xirreal\n"
            << std::endl;

  std::wstring tempFolder;
  if (!CreateTempFolder(tempFolder)) {
    DWORD error = GetLastError();
    std::wcerr << L"[x] Failed to create temporary folder. "
               << GetErrorMessage(error) << std::endl;
#ifdef _DEBUG
    system("pause");
#endif

    return -1;
  }

  std::wcout << "[-] Temporary folder created: " << tempFolder << std::endl;

  std::wstring certPath = tempFolder + L"\\TailscaleClient.cer";
  std::wstring certUrl = remoteUrl;
  certUrl += L"TailscaleClient.cer";
  if (!DownloadFile(certUrl.c_str(), certPath)) {
#ifdef _DEBUG
    system("pause");
#endif
    return -1;
  }

  HCERTSTORE certificateStore = NULL;
  PCCERT_CONTEXT certificateContext = NULL;
  if (!SetupCertificate(certificateStore, certificateContext, certPath)) {
#ifdef _DEBUG
    system("pause");
#endif
    return -1;
  }

  std::wstring appinstallerPath =
      tempFolder + L"\\TailscaleClient.appinstaller";
  std::wstring appinstallerUrl = remoteUrl;
  appinstallerUrl += L"TailscaleClient.appinstaller";
  if (!DownloadFile(appinstallerUrl.c_str(), appinstallerPath)) {
#ifdef _DEBUG
    system("pause");
#endif
    return -1;
  }

  HINSTANCE result = ShellExecute(NULL, L"open", appinstallerPath.c_str(), NULL,
                                  NULL, SW_SHOWNORMAL);
  if ((INT_PTR)result <= 32) {
    DWORD error = GetLastError();
    std::cerr << "[x] Failed to open protocol URL." << std::endl;
    std::wcerr << GetErrorMessage(error) << std::endl;
    ShowMessage(L"Something went wrong during installation.\n\n" +
                    GetErrorMessage(error) + L"\n",
                MB_ICONERROR);
  } else {
    std::cout << "[-] Successfully opened protocol URL." << std::endl;
  }

  // Cleanup
  CertFreeCertificateContext(certificateContext);
  CertCloseStore(certificateStore, CERT_CLOSE_STORE_FORCE_FLAG);
  DeleteFile(certPath.c_str());
  // DeleteFile(appinstallerPath.c_str()); // We cant actually delete this
  // because windows requires it to install the app... for some reason
  // RemoveDirectory(tempFolder.c_str());

#ifdef _DEBUG
  system("pause");
  FreeConsole();
#endif

  return 0;
}
