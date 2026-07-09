using System;
using System.Collections.Generic;
using System.Linq;
using WrapperFromCToSharp;

namespace GenericSim
{
    /// <summary>Static catalog of all messages discovered from the C files.</summary>
    public sealed class MessageInfo
    {
        public string Name { get; init; } = string.Empty;
        public int MessageId { get; init; }
        public int Length { get; init; }
        public FieldInfo[] Fields { get; init; } = Array.Empty<FieldInfo>();
        public Func<IntPtr> GetPhysical { get; init; } = () => IntPtr.Zero;
        public Action<byte[], IntPtr> ConvertToInterface { get; init; } = (_, _) => { };
        public Action<byte[], IntPtr> ConvertToPhysical { get; init; } = (_, _) => { };
        public ArrayInfo[] Arrays { get; init; } = Array.Empty<ArrayInfo>();
    }

    public static class MessageCatalog
    {
        public static readonly IReadOnlyList<MessageInfo> Messages = BuildAll();

        public static MessageInfo? ByName(string name) => Messages.FirstOrDefault(m => m.Name == name);
        public static MessageInfo? ById(int id) => Messages.FirstOrDefault(m => m.MessageId == id);

        private static List<MessageInfo> BuildAll()
        {
            var list = new List<MessageInfo>();
            list.Add(new MessageInfo
            {
                Name = "SET_DISCRETES_MESSAGE",
                MessageId = 1,
                Length = 541,
                GetPhysical = () => WrapperInterop.Wrapper_Get_SET_DISCRETES_MESSAGE(),
                ConvertToInterface = (buf, phys) => WrapperInterop.SET_DISCRETES_MESSAGE_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.SET_DISCRETES_MESSAGE_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_LCU2IO.msgId", Type = "UINT16", Size = 2, DefaultValue = "1" },
                    new FieldInfo { Offset = 2, Field = "HDR_LCU2IO.totalLength", Type = "UINT16", Size = 2, DefaultValue = "541" },
                    new FieldInfo { Offset = 4, Field = "HDR_LCU2IO.requestSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_LCU2IO.enableResponse", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_LCU2IO.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_LCU2IO.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                    new FieldInfo { Offset = 24, Field = "DT_SET_DISC.discNum", Type = "UINT8", Size = 1, DefaultValue = "0" },
                    new FieldInfo { Offset = 537, Field = "TL_LCU2IO.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = new[]
                {
                    new ArrayInfo
                    {
                        Name = "DT_SET_DISC.discretes", BaseOffset = 25, Stride = 2, MaxCount = 256, IndexVar = "i1", CountField = "DT_SET_DISC.discNum",
                        Elements = new[]
                        {
                            new ArrayElementInfo { RelativeOffset = 0, Field = "DT_SET_DISC.discretes[i1].discreteId", Type = "UINT8", Size = 1 },
                            new ArrayElementInfo { RelativeOffset = 1, Field = "DT_SET_DISC.discretes[i1].discreteValue", Type = "UINT8", Size = 1 },
                        }
                    },
                }
            });
            list.Add(new MessageInfo
            {
                Name = "SERIAL_LCU2IO_MESSAGE",
                MessageId = 2,
                Length = 1060,
                GetPhysical = () => WrapperInterop.Wrapper_Get_SERIAL_LCU2IO_MESSAGE(),
                ConvertToInterface = (buf, phys) => WrapperInterop.SERIAL_LCU2IO_MESSAGE_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.SERIAL_LCU2IO_MESSAGE_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_LCU2IO.msgId", Type = "UINT16", Size = 2, DefaultValue = "2" },
                    new FieldInfo { Offset = 2, Field = "HDR_LCU2IO.totalLength", Type = "UINT16", Size = 2, DefaultValue = "1060" },
                    new FieldInfo { Offset = 4, Field = "HDR_LCU2IO.requestSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_LCU2IO.enableResponse", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_LCU2IO.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_LCU2IO.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                    new FieldInfo { Offset = 24, Field = "DT_SERIAL_LCU2IO.uartId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 28, Field = "DT_SERIAL_LCU2IO.data.Size", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 1056, Field = "TL_LCU2IO.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = new[]
                {
                    new ArrayInfo
                    {
                        Name = "DT_SERIAL_LCU2IO.data.data", BaseOffset = 32, Stride = 1, MaxCount = 1024, IndexVar = "i1", CountField = "DT_SERIAL_LCU2IO.data.Size",
                        Elements = new[]
                        {
                            new ArrayElementInfo { RelativeOffset = 0, Field = "DT_SERIAL_LCU2IO.data.data[i1]", Type = "UINT8", Size = 1 },
                        }
                    },
                }
            });
            list.Add(new MessageInfo
            {
                Name = "SET_SYSTEM_STATE_MESSAGE",
                MessageId = 3,
                Length = 32,
                GetPhysical = () => WrapperInterop.Wrapper_Get_SET_SYSTEM_STATE_MESSAGE(),
                ConvertToInterface = (buf, phys) => WrapperInterop.SET_SYSTEM_STATE_MESSAGE_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.SET_SYSTEM_STATE_MESSAGE_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_LCU2IO.msgId", Type = "UINT16", Size = 2, DefaultValue = "3" },
                    new FieldInfo { Offset = 2, Field = "HDR_LCU2IO.totalLength", Type = "UINT16", Size = 2, DefaultValue = "32" },
                    new FieldInfo { Offset = 4, Field = "HDR_LCU2IO.requestSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_LCU2IO.enableResponse", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_LCU2IO.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_LCU2IO.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                    new FieldInfo { Offset = 24, Field = "DT_SET_SYS_STATE.sysState", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 28, Field = "TL_LCU2IO.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = Array.Empty<ArrayInfo>()
            });
            list.Add(new MessageInfo
            {
                Name = "START_INTERFACE_MESSAGE",
                MessageId = 5,
                Length = 56,
                GetPhysical = () => WrapperInterop.Wrapper_Get_START_INTERFACE_MESSAGE(),
                ConvertToInterface = (buf, phys) => WrapperInterop.START_INTERFACE_MESSAGE_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.START_INTERFACE_MESSAGE_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_LCU2IO.msgId", Type = "UINT16", Size = 2, DefaultValue = "5" },
                    new FieldInfo { Offset = 2, Field = "HDR_LCU2IO.totalLength", Type = "UINT16", Size = 2, DefaultValue = "56" },
                    new FieldInfo { Offset = 4, Field = "HDR_LCU2IO.requestSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_LCU2IO.enableResponse", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_LCU2IO.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_LCU2IO.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                    new FieldInfo { Offset = 24, Field = "DT_START_INTERFACE.uartId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 28, Field = "DT_START_INTERFACE.baudrate", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 32, Field = "DT_START_INTERFACE.txParity", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 36, Field = "DT_START_INTERFACE.rxParity", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 40, Field = "DT_START_INTERFACE.stopBits", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 44, Field = "DT_START_INTERFACE.dataBits", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 48, Field = "DT_START_INTERFACE.loopback", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 52, Field = "TL_LCU2IO.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = Array.Empty<ArrayInfo>()
            });
            list.Add(new MessageInfo
            {
                Name = "STOP_INTERFACE_MESSAGE",
                MessageId = 6,
                Length = 32,
                GetPhysical = () => WrapperInterop.Wrapper_Get_STOP_INTERFACE_MESSAGE(),
                ConvertToInterface = (buf, phys) => WrapperInterop.STOP_INTERFACE_MESSAGE_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.STOP_INTERFACE_MESSAGE_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_LCU2IO.msgId", Type = "UINT16", Size = 2, DefaultValue = "6" },
                    new FieldInfo { Offset = 2, Field = "HDR_LCU2IO.totalLength", Type = "UINT16", Size = 2, DefaultValue = "32" },
                    new FieldInfo { Offset = 4, Field = "HDR_LCU2IO.requestSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_LCU2IO.enableResponse", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_LCU2IO.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_LCU2IO.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                    new FieldInfo { Offset = 24, Field = "DT_STOP_INTERFACE.uartId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 28, Field = "TL_LCU2IO.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = Array.Empty<ArrayInfo>()
            });
            list.Add(new MessageInfo
            {
                Name = "KEEP_ALIVE_MESSAGE",
                MessageId = 7,
                Length = 28,
                GetPhysical = () => WrapperInterop.Wrapper_Get_KEEP_ALIVE_MESSAGE(),
                ConvertToInterface = (buf, phys) => WrapperInterop.KEEP_ALIVE_MESSAGE_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.KEEP_ALIVE_MESSAGE_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_LCU2IO.msgId", Type = "UINT16", Size = 2, DefaultValue = "7" },
                    new FieldInfo { Offset = 2, Field = "HDR_LCU2IO.totalLength", Type = "UINT16", Size = 2, DefaultValue = "28" },
                    new FieldInfo { Offset = 4, Field = "HDR_LCU2IO.requestSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_LCU2IO.enableResponse", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_LCU2IO.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_LCU2IO.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                    new FieldInfo { Offset = 24, Field = "TL_LCU2IO.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = Array.Empty<ArrayInfo>()
            });
            list.Add(new MessageInfo
            {
                Name = "REQUEST_STATUS_MESSAGE",
                MessageId = 101,
                Length = 1064,
                GetPhysical = () => WrapperInterop.Wrapper_Get_REQUEST_STATUS_MESSAGE(),
                ConvertToInterface = (buf, phys) => WrapperInterop.REQUEST_STATUS_MESSAGE_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.REQUEST_STATUS_MESSAGE_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_IO2LCU.msgId", Type = "UINT16", Size = 2, DefaultValue = "101" },
                    new FieldInfo { Offset = 2, Field = "HDR_IO2LCU.totalLength", Type = "UINT16", Size = 2, DefaultValue = "1064" },
                    new FieldInfo { Offset = 4, Field = "HDR_IO2LCU.statusSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_IO2LCU.unitId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_IO2LCU.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_IO2LCU.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                    new FieldInfo { Offset = 24, Field = "DT_REQUEST_STATUS.reqId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 28, Field = "DT_REQUEST_STATUS.errId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 32, Field = "DT_REQUEST_STATUS.errString.Size", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 1060, Field = "TL_IO2LCU.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = new[]
                {
                    new ArrayInfo
                    {
                        Name = "DT_REQUEST_STATUS.errString.data", BaseOffset = 36, Stride = 1, MaxCount = 1024, IndexVar = "i1", CountField = "DT_REQUEST_STATUS.errString.Size",
                        Elements = new[]
                        {
                            new ArrayElementInfo { RelativeOffset = 0, Field = "DT_REQUEST_STATUS.errString.data[i1]", Type = "UINT8", Size = 1 },
                        }
                    },
                }
            });
            list.Add(new MessageInfo
            {
                Name = "STATUS_MESSAGE",
                MessageId = 102,
                Length = 11764,
                GetPhysical = () => WrapperInterop.Wrapper_Get_STATUS_MESSAGE(),
                ConvertToInterface = (buf, phys) => WrapperInterop.STATUS_MESSAGE_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.STATUS_MESSAGE_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_IO2LCU.msgId", Type = "UINT16", Size = 2, DefaultValue = "102" },
                    new FieldInfo { Offset = 2, Field = "HDR_IO2LCU.totalLength", Type = "UINT16", Size = 2, DefaultValue = "11764" },
                    new FieldInfo { Offset = 4, Field = "HDR_IO2LCU.statusSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_IO2LCU.unitId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_IO2LCU.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_IO2LCU.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                    new FieldInfo { Offset = 24, Field = "DT_MONITORED_DATA.registeredUnits.length", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 108, Field = "DT_MONITORED_DATA.internalUnitInfo.unitMainVer", Type = "UINT8", Size = 1, DefaultValue = "0" },
                    new FieldInfo { Offset = 109, Field = "DT_MONITORED_DATA.internalUnitInfo.unitSubVer", Type = "UINT8", Size = 1, DefaultValue = "0" },
                    new FieldInfo { Offset = 110, Field = "DT_MONITORED_DATA.internalUnitInfo.unitRevision", Type = "UINT16", Size = 2, DefaultValue = "0" },
                    new FieldInfo { Offset = 112, Field = "DT_MONITORED_DATA.internalUnitInfo.unitId", Type = "UINT8", Size = 1, DefaultValue = "0" },
                    new FieldInfo { Offset = 113, Field = "DT_MONITORED_DATA.internalUnitInfo.cbitStatus", Type = "UINT8", Size = 1, DefaultValue = "0" },
                    new FieldInfo { Offset = 114, Field = "DT_MONITORED_DATA.internalUnitInfo.registerStatus", Type = "UINT8", Size = 1, DefaultValue = "0" },
                    new FieldInfo { Offset = 115, Field = "DT_MONITORED_DATA.internalUnitInfo.sysState", Type = "UINT8", Size = 1, DefaultValue = "0" },
                    new FieldInfo { Offset = 116, Field = "DT_MONITORED_DATA.periodicDiscInfo.length", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 632, Field = "DT_MONITORED_DATA.analogInputsInfo.length", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 1436, Field = "DT_MONITORED_DATA.serialInputsInfo.length", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 11760, Field = "TL_IO2LCU.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = new[]
                {
                    new ArrayInfo
                    {
                        Name = "DT_MONITORED_DATA.registeredUnits.registeredUnits", BaseOffset = 28, Stride = 8, MaxCount = 10, IndexVar = "i1", CountField = "DT_MONITORED_DATA.registeredUnits.length",
                        Elements = new[]
                        {
                            new ArrayElementInfo { RelativeOffset = 0, Field = "DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitMainVer", Type = "UINT8", Size = 1 },
                            new ArrayElementInfo { RelativeOffset = 1, Field = "DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitSubVer", Type = "UINT8", Size = 1 },
                            new ArrayElementInfo { RelativeOffset = 2, Field = "DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitRevision", Type = "UINT16", Size = 2 },
                            new ArrayElementInfo { RelativeOffset = 4, Field = "DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitId", Type = "UINT8", Size = 1 },
                            new ArrayElementInfo { RelativeOffset = 5, Field = "DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].cbitStatus", Type = "UINT8", Size = 1 },
                            new ArrayElementInfo { RelativeOffset = 6, Field = "DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].registerStatus", Type = "UINT8", Size = 1 },
                            new ArrayElementInfo { RelativeOffset = 7, Field = "DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].sysState", Type = "UINT8", Size = 1 },
                        }
                    },
                    new ArrayInfo
                    {
                        Name = "DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo", BaseOffset = 120, Stride = 2, MaxCount = 256, IndexVar = "i1", CountField = "DT_MONITORED_DATA.periodicDiscInfo.length",
                        Elements = new[]
                        {
                            new ArrayElementInfo { RelativeOffset = 0, Field = "DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteId", Type = "UINT8", Size = 1 },
                            new ArrayElementInfo { RelativeOffset = 1, Field = "DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteValue", Type = "UINT8", Size = 1 },
                        }
                    },
                    new ArrayInfo
                    {
                        Name = "DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo", BaseOffset = 636, Stride = 8, MaxCount = 100, IndexVar = "i1", CountField = "DT_MONITORED_DATA.analogInputsInfo.length",
                        Elements = new[]
                        {
                            new ArrayElementInfo { RelativeOffset = 0, Field = "DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogId", Type = "UINT32", Size = 4 },
                            new ArrayElementInfo { RelativeOffset = 4, Field = "DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogVal", Type = "FLOAT32", Size = 4 },
                        }
                    },
                    new ArrayInfo
                    {
                        Name = "DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo", BaseOffset = 1440, Stride = 1032, MaxCount = 10, IndexVar = "i1", CountField = "DT_MONITORED_DATA.serialInputsInfo.length",
                        Elements = new[]
                        {
                            new ArrayElementInfo { RelativeOffset = 0, Field = "DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].serialId", Type = "UINT32", Size = 4 },
                            new ArrayElementInfo { RelativeOffset = 4, Field = "DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.Size", Type = "UINT32", Size = 4 },
                        }
                    },
                }
            });
            list.Add(new MessageInfo
            {
                Name = "SERIAL_IO2LCU_MESSAGE",
                MessageId = 103,
                Length = 1060,
                GetPhysical = () => WrapperInterop.Wrapper_Get_SERIAL_IO2LCU_MESSAGE(),
                ConvertToInterface = (buf, phys) => WrapperInterop.SERIAL_IO2LCU_MESSAGE_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.SERIAL_IO2LCU_MESSAGE_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_IO2LCU.msgId", Type = "UINT16", Size = 2, DefaultValue = "103" },
                    new FieldInfo { Offset = 2, Field = "HDR_IO2LCU.totalLength", Type = "UINT16", Size = 2, DefaultValue = "1060" },
                    new FieldInfo { Offset = 4, Field = "HDR_IO2LCU.statusSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_IO2LCU.unitId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_IO2LCU.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_IO2LCU.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                    new FieldInfo { Offset = 24, Field = "DT_SERIAL_IO2LCU.serialId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 28, Field = "DT_SERIAL_IO2LCU.data.Size", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 1056, Field = "TL_IO2LCU.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = new[]
                {
                    new ArrayInfo
                    {
                        Name = "DT_SERIAL_IO2LCU.data.data", BaseOffset = 32, Stride = 1, MaxCount = 1024, IndexVar = "i1", CountField = "DT_SERIAL_IO2LCU.data.Size",
                        Elements = new[]
                        {
                            new ArrayElementInfo { RelativeOffset = 0, Field = "DT_SERIAL_IO2LCU.data.data[i1]", Type = "UINT8", Size = 1 },
                        }
                    },
                }
            });
            list.Add(new MessageInfo
            {
                Name = "LOG_DATA_MESSAGE",
                MessageId = 104,
                Length = 1064,
                GetPhysical = () => WrapperInterop.Wrapper_Get_LOG_DATA_MESSAGE(),
                ConvertToInterface = (buf, phys) => WrapperInterop.LOG_DATA_MESSAGE_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.LOG_DATA_MESSAGE_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_IO2LCU.msgId", Type = "UINT16", Size = 2, DefaultValue = "104" },
                    new FieldInfo { Offset = 2, Field = "HDR_IO2LCU.totalLength", Type = "UINT16", Size = 2, DefaultValue = "1064" },
                    new FieldInfo { Offset = 4, Field = "HDR_IO2LCU.statusSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_IO2LCU.unitId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_IO2LCU.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_IO2LCU.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                    new FieldInfo { Offset = 24, Field = "DT_LOG.logLevel", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 28, Field = "DT_LOG.logId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 32, Field = "DT_LOG.logStr.Size", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 1060, Field = "TL_IO2LCU.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = new[]
                {
                    new ArrayInfo
                    {
                        Name = "DT_LOG.logStr.data", BaseOffset = 36, Stride = 1, MaxCount = 1024, IndexVar = "i1", CountField = "DT_LOG.logStr.Size",
                        Elements = new[]
                        {
                            new ArrayElementInfo { RelativeOffset = 0, Field = "DT_LOG.logStr.data[i1]", Type = "UINT8", Size = 1 },
                        }
                    },
                }
            });
            list.Add(new MessageInfo
            {
                Name = "NONCE_MESSAGE",
                MessageId = 200,
                Length = 36,
                GetPhysical = () => WrapperInterop.Wrapper_Get_NONCE_MESSAGE(),
                ConvertToInterface = (buf, phys) => WrapperInterop.NONCE_MESSAGE_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.NONCE_MESSAGE_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_IO2LCU.msgId", Type = "UINT16", Size = 2, DefaultValue = "200" },
                    new FieldInfo { Offset = 2, Field = "HDR_IO2LCU.totalLength", Type = "UINT16", Size = 2, DefaultValue = "36" },
                    new FieldInfo { Offset = 4, Field = "HDR_IO2LCU.statusSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_IO2LCU.unitId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_IO2LCU.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_IO2LCU.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                    new FieldInfo { Offset = 24, Field = "DT_NONCE.ipAddr", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 28, Field = "DT_NONCE.pbitStatus", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 32, Field = "TL_IO2LCU.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = Array.Empty<ArrayInfo>()
            });
            list.Add(new MessageInfo
            {
                Name = "HDR_ONLY_MESSAGE_IO2LCU",
                MessageId = 1001,
                Length = 24,
                GetPhysical = () => WrapperInterop.Wrapper_Get_HDR_ONLY_MESSAGE_IO2LCU(),
                ConvertToInterface = (buf, phys) => WrapperInterop.HDR_ONLY_MESSAGE_IO2LCU_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.HDR_ONLY_MESSAGE_IO2LCU_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_IO2LCU.msgId", Type = "UINT16", Size = 2, DefaultValue = "1001" },
                    new FieldInfo { Offset = 2, Field = "HDR_IO2LCU.totalLength", Type = "UINT16", Size = 2, DefaultValue = "24" },
                    new FieldInfo { Offset = 4, Field = "HDR_IO2LCU.statusSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_IO2LCU.unitId", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_IO2LCU.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_IO2LCU.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                },
                Arrays = Array.Empty<ArrayInfo>()
            });
            list.Add(new MessageInfo
            {
                Name = "HDR_ONLY_MESSAGE_LCU2IO",
                MessageId = 1002,
                Length = 24,
                GetPhysical = () => WrapperInterop.Wrapper_Get_HDR_ONLY_MESSAGE_LCU2IO(),
                ConvertToInterface = (buf, phys) => WrapperInterop.HDR_ONLY_MESSAGE_LCU2IO_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.HDR_ONLY_MESSAGE_LCU2IO_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "HDR_LCU2IO.msgId", Type = "UINT16", Size = 2, DefaultValue = "1002" },
                    new FieldInfo { Offset = 2, Field = "HDR_LCU2IO.totalLength", Type = "UINT16", Size = 2, DefaultValue = "24" },
                    new FieldInfo { Offset = 4, Field = "HDR_LCU2IO.requestSeqNum", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 8, Field = "HDR_LCU2IO.enableResponse", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 12, Field = "HDR_LCU2IO.nonce", Type = "UINT32", Size = 4, DefaultValue = "0" },
                    new FieldInfo { Offset = 16, Field = "HDR_LCU2IO.timestamp", Type = "FLOAT64", Size = 8, DefaultValue = "0" },
                },
                Arrays = Array.Empty<ArrayInfo>()
            });
            list.Add(new MessageInfo
            {
                Name = "TL_ONLY_MESSAGE_IO2LCU",
                MessageId = 1003,
                Length = 4,
                GetPhysical = () => WrapperInterop.Wrapper_Get_TL_ONLY_MESSAGE_IO2LCU(),
                ConvertToInterface = (buf, phys) => WrapperInterop.TL_ONLY_MESSAGE_IO2LCU_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.TL_ONLY_MESSAGE_IO2LCU_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "TL_IO2LCU.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = Array.Empty<ArrayInfo>()
            });
            list.Add(new MessageInfo
            {
                Name = "TL_ONLY_MESSAGE_LCU2IO",
                MessageId = 1004,
                Length = 4,
                GetPhysical = () => WrapperInterop.Wrapper_Get_TL_ONLY_MESSAGE_LCU2IO(),
                ConvertToInterface = (buf, phys) => WrapperInterop.TL_ONLY_MESSAGE_LCU2IO_ConvertToInterface(phys, buf),
                ConvertToPhysical = (buf, phys) => WrapperInterop.TL_ONLY_MESSAGE_LCU2IO_ConvertToPhysical(phys, buf),
                Fields = new[]
                {
                    new FieldInfo { Offset = 0, Field = "TL_LCU2IO.crc", Type = "UINT32", Size = 4, DefaultValue = "0" },
                },
                Arrays = Array.Empty<ArrayInfo>()
            });
            return list;
        }
    }
}
