# WrapperFromCToSharp

This native C++ DLL project was generated automatically by **InterfaceWrapper**.
It wraps the generated `cvi.c` / `cvp.c` convert functions behind a flat
`extern "C"` ABI so they can be called from a C# project.

## Contents

- `WrapperFromCToSharp.vcxproj` – native DLL project (build with MSVC / Visual Studio).
- `WrapperFromCToSharp.h` / `WrapperFromCToSharp.cpp` – exported wrapper functions.
- `Messages/` – a self-contained copy of the parsed C source and headers.
- `WrapperInterop.cs` – ready-to-use P/Invoke bindings for the C# side.

## Discovered messages

| Message | Message Id | Physical Type | Directions |
|---------|-----------|---------------|------------|
| HDR_ONLY_MESSAGE_IO2LCU | 1001 | PHS_HDRONLYMESSAGEIO2LCU | ToPhysical + ToInterface |
| HDR_ONLY_MESSAGE_LCU2IO | 1002 | PHS_HDRONLYMESSAGELCU2IO | ToPhysical + ToInterface |
| KEEP_ALIVE_MESSAGE | 7 | PHS_KEEPALIVEMESSAGE | ToPhysical + ToInterface |
| LOG_DATA_MESSAGE | 104 | PHS_LOGDATAMESSAGE | ToPhysical + ToInterface |
| NONCE_MESSAGE | 200 | PHS_NONCEMESSAGE | ToPhysical + ToInterface |
| REQUEST_STATUS_MESSAGE | 101 | PHS_REQUESTSTATUSMESSAGE | ToPhysical + ToInterface |
| SERIAL_IO2LCU_MESSAGE | 103 | PHS_SERIALIO2LCUMESSAGE | ToPhysical + ToInterface |
| SERIAL_LCU2IO_MESSAGE | 2 | PHS_SERIALLCU2IOMESSAGE | ToPhysical + ToInterface |
| SET_DISCRETES_MESSAGE | 1 | PHS_SETDISCRETESMESSAGE | ToPhysical + ToInterface |
| SET_SYSTEM_STATE_MESSAGE | 3 | PHS_SETSYSTEMSTATEMESSAGE | ToPhysical + ToInterface |
| START_INTERFACE_MESSAGE | 5 | PHS_STARTINTERFACEMESSAGE | ToPhysical + ToInterface |
| STATUS_MESSAGE | 102 | PHS_STATUSMESSAGE | ToPhysical + ToInterface |
| STOP_INTERFACE_MESSAGE | 6 | PHS_STOPINTERFACEMESSAGE | ToPhysical + ToInterface |
| TL_ONLY_MESSAGE_IO2LCU | 1003 | PHS_TLONLYMESSAGEIO2LCU | ToPhysical + ToInterface |
| TL_ONLY_MESSAGE_LCU2IO | 1004 | PHS_TLONLYMESSAGELCU2IO | ToPhysical + ToInterface |

## Usage from C#

```csharp
// 1. Fill the static physical structure from a received ethernet buffer:
WrapperInterop.Wrapper_ConvertToPhysical((int)WrapperInterop.MessageId.STATUS_MESSAGE, rxBuffer);
IntPtr phys = WrapperInterop.Wrapper_Get_STATUS_MESSAGE();
// Marshal 'phys' to a matching [StructLayout] struct to read the values.

// 2. Serialize the static physical structure into a buffer to send:
WrapperInterop.Wrapper_ConvertToInterface((int)WrapperInterop.MessageId.STATUS_MESSAGE, txBuffer);
```
