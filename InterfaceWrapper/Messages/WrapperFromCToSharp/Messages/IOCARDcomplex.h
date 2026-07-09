/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDcomplex.h                                                                                                                 */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Complex Types declaration                                                                                                       */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDcomplex_h
#define IOCARDcomplex_h

#include "IOCARDenum.h"
#include "typelib.h"

#ifdef __cplusplus
extern "C" {
#endif


/* HDR_LCU2IO_STRUCT */
#ifndef _STRUCTURE_TYPE_HDR_LCU2IO_STRUCT
#define _STRUCTURE_TYPE_HDR_LCU2IO_STRUCT

typedef struct HDR_LCU2IO_STRUCT{
	/* MESSAGE_ID == MESSAGE_TYPE. Determines the message type. */
	E_MESSAGE_TYPE msgId;
	/* PACKET LENGTH, INCLUDING HEADER (!) */
	AdiUInt16 totalLength;
	/* Sequence number of the message. */
	AdiUInt32 requestSeqNum;
	/* To enable response or to not. */
	E_BOOLEAN enableResponse;
	/* Generated in IO Unit, unique for each session IDLE --> OPER */
	AdiUInt32 nonce;
	/* UTC/PTP Timestamp */
	AdiFloat32 timestamp;
} HDR_LCU2IO_STRUCT;

#endif /* _STRUCTURE_TYPE_HDR_LCU2IO_STRUCT */

/* TL_LCU2IO_STRUCT */
#ifndef _STRUCTURE_TYPE_TL_LCU2IO_STRUCT
#define _STRUCTURE_TYPE_TL_LCU2IO_STRUCT

typedef struct TL_LCU2IO_STRUCT{
	/* CRC in 32 bits */
	AdiUInt32 crc;
} TL_LCU2IO_STRUCT;

#endif /* _STRUCTURE_TYPE_TL_LCU2IO_STRUCT */

/* SERIAL_DATA_BUFFER_ARR */
#ifndef _ARRAY_TYPE_SERIAL_DATA_BUFFER_ARR
#define _ARRAY_TYPE_SERIAL_DATA_BUFFER_ARR

typedef AdiUInt8 SERIAL_DATA_BUFFER_ARR[1024];

#endif /* _ARRAY_TYPE_SERIAL_DATA_BUFFER_ARR */

/* BUFFER_STRUCT */
#ifndef _STRUCTURE_TYPE_BUFFER_STRUCT
#define _STRUCTURE_TYPE_BUFFER_STRUCT

typedef struct BUFFER_STRUCT{
	AdiUInt32 Size;
	SERIAL_DATA_BUFFER_ARR data;
} BUFFER_STRUCT;

#endif /* _STRUCTURE_TYPE_BUFFER_STRUCT */

/* DT_SERIAL_LCU2IO_STRUCT */
#ifndef _STRUCTURE_TYPE_DT_SERIAL_LCU2IO_STRUCT
#define _STRUCTURE_TYPE_DT_SERIAL_LCU2IO_STRUCT

typedef struct DT_SERIAL_LCU2IO_STRUCT{
	/* Uart relative location in IO Unit / LUE2D / FIB */
	E_UART_ID uartId;
	BUFFER_STRUCT data;
} DT_SERIAL_LCU2IO_STRUCT;

#endif /* _STRUCTURE_TYPE_DT_SERIAL_LCU2IO_STRUCT */

/* DISCRETE_STRUCT */
#ifndef _STRUCTURE_TYPE_DISCRETE_STRUCT
#define _STRUCTURE_TYPE_DISCRETE_STRUCT

typedef struct DISCRETE_STRUCT{
	/* Discrete's ID */
	E_DISCRETE_ID discreteId;
	/* Discrete's value */
	E_BOOLEAN discreteValue;
} DISCRETE_STRUCT;

#endif /* _STRUCTURE_TYPE_DISCRETE_STRUCT */

/* DISCRETE_ARR */
#ifndef _ARRAY_TYPE_DISCRETE_ARR
#define _ARRAY_TYPE_DISCRETE_ARR

typedef DISCRETE_STRUCT DISCRETE_ARR[256];

#endif /* _ARRAY_TYPE_DISCRETE_ARR */

/* DT_SET_DISC_STRUCT */
#ifndef _STRUCTURE_TYPE_DT_SET_DISC_STRUCT
#define _STRUCTURE_TYPE_DT_SET_DISC_STRUCT

typedef struct DT_SET_DISC_STRUCT{
	/* how many instances in this array, represented as a byte */
	AdiUInt8 discNum;
	/* Discretes array as generic. This is just the struct. */
	DISCRETE_ARR discretes;
} DT_SET_DISC_STRUCT;

#endif /* _STRUCTURE_TYPE_DT_SET_DISC_STRUCT */

/* DT_SET_SYS_STATE_STRUCT */
#ifndef _STRUCTURE_TYPE_DT_SET_SYS_STATE_STRUCT
#define _STRUCTURE_TYPE_DT_SET_SYS_STATE_STRUCT

typedef struct DT_SET_SYS_STATE_STRUCT{
	/* Enumeration - Operational, Programming, Idle.. */
	E_UNIT_STATE sysState;
} DT_SET_SYS_STATE_STRUCT;

#endif /* _STRUCTURE_TYPE_DT_SET_SYS_STATE_STRUCT */

/* DT_START_INTERFACE_STRUCT */
#ifndef _STRUCTURE_TYPE_DT_START_INTERFACE_STRUCT
#define _STRUCTURE_TYPE_DT_START_INTERFACE_STRUCT

typedef struct DT_START_INTERFACE_STRUCT{
	/* Uart relative location in IO Unit / LUE2D / FIB */
	E_UART_ID uartId;
	/* baud rate.. */
	AdiUInt32 baudrate;
	E_PARITY txParity;
	E_PARITY rxParity;
	AdiUInt32 stopBits;
	AdiUInt32 dataBits;
	/* On/Off */
	E_BOOLEAN loopback;
} DT_START_INTERFACE_STRUCT;

#endif /* _STRUCTURE_TYPE_DT_START_INTERFACE_STRUCT */

/* DT_STOP_INTERFACE_STRUCT */
#ifndef _STRUCTURE_TYPE_DT_STOP_INTERFACE_STRUCT
#define _STRUCTURE_TYPE_DT_STOP_INTERFACE_STRUCT

typedef struct DT_STOP_INTERFACE_STRUCT{
	/* Uart relative location in IO Unit / LUE2D / FIB */
	E_UART_ID uartId;
} DT_STOP_INTERFACE_STRUCT;

#endif /* _STRUCTURE_TYPE_DT_STOP_INTERFACE_STRUCT */

/* HDR_IO2LCU_STRUCT */
#ifndef _STRUCTURE_TYPE_HDR_IO2LCU_STRUCT
#define _STRUCTURE_TYPE_HDR_IO2LCU_STRUCT

typedef struct HDR_IO2LCU_STRUCT{
	/* MESSAGE_ID == MESSAGE_TYPE. Determines the message type. */
	E_MESSAGE_TYPE msgId;
	/* PACKET LENGTH, INCLUDING HEADER (!) */
	AdiUInt16 totalLength;
	/* Sequence number of the message. */
	AdiUInt32 statusSeqNum;
	/* Unit identification num - IOUnit, FIB, LUE2D1 ... */
	E_UNIT_ID unitId;
	/* Generated in IO Unit, unique for each session IDLE --> OPER */
	AdiUInt32 nonce;
	/* UTC/PTP Timestamp */
	AdiFloat32 timestamp;
} HDR_IO2LCU_STRUCT;

#endif /* _STRUCTURE_TYPE_HDR_IO2LCU_STRUCT */

/* DT_LOG_STRUCT */
#ifndef _STRUCTURE_TYPE_DT_LOG_STRUCT
#define _STRUCTURE_TYPE_DT_LOG_STRUCT

typedef struct DT_LOG_STRUCT{
	/* Logging level */
	E_LOG_LEVEL logLevel;
	/* Log message index, TBD */
	E_LOG_ID logId;
	BUFFER_STRUCT logStr;
} DT_LOG_STRUCT;

#endif /* _STRUCTURE_TYPE_DT_LOG_STRUCT */

/* TL_IO2LCU_STRUCT */
#ifndef _STRUCTURE_TYPE_TL_IO2LCU_STRUCT
#define _STRUCTURE_TYPE_TL_IO2LCU_STRUCT

typedef struct TL_IO2LCU_STRUCT{
	/* CRC in 32 bits */
	AdiUInt32 crc;
} TL_IO2LCU_STRUCT;

#endif /* _STRUCTURE_TYPE_TL_IO2LCU_STRUCT */

/* DT_NONCE_STRUCT */
#ifndef _STRUCTURE_TYPE_DT_NONCE_STRUCT
#define _STRUCTURE_TYPE_DT_NONCE_STRUCT

typedef struct DT_NONCE_STRUCT{
	/* IPv4 address, little endian (Source IP?) */
	AdiUInt32 ipAddr;
	/* PBit status, 0 = Success, other values = TBD */
	AdiUInt32 pbitStatus;
} DT_NONCE_STRUCT;

#endif /* _STRUCTURE_TYPE_DT_NONCE_STRUCT */

/* DT_REQUEST_STATUS_STRUCT */
#ifndef _STRUCTURE_TYPE_DT_REQUEST_STATUS_STRUCT
#define _STRUCTURE_TYPE_DT_REQUEST_STATUS_STRUCT

typedef struct DT_REQUEST_STATUS_STRUCT{
	/* The id of the request message the response relates to */
	E_MESSAGE_TYPE reqId;
	/* eErrorVals, TBD, 0 = success */
	E_ERROR_ID errId;
	BUFFER_STRUCT errString;
} DT_REQUEST_STATUS_STRUCT;

#endif /* _STRUCTURE_TYPE_DT_REQUEST_STATUS_STRUCT */

/* DT_SERIAL_IO2LCU_STRUCT */
#ifndef _STRUCTURE_TYPE_DT_SERIAL_IO2LCU_STRUCT
#define _STRUCTURE_TYPE_DT_SERIAL_IO2LCU_STRUCT

typedef struct DT_SERIAL_IO2LCU_STRUCT{
	/* Numerical indentification of serial communication channel in IO Unit */
	AdiUInt32 serialId;
	BUFFER_STRUCT data;
} DT_SERIAL_IO2LCU_STRUCT;

#endif /* _STRUCTURE_TYPE_DT_SERIAL_IO2LCU_STRUCT */

/* sRegInfo_STRUCT */
#ifndef _STRUCTURE_TYPE_sRegInfo_STRUCT
#define _STRUCTURE_TYPE_sRegInfo_STRUCT

typedef struct sRegInfo_STRUCT{
	/* Main version, should be an integer (ex. 5.256 -> Main version is '5') */
	AdiUInt8 unitMainVer;
	/* Sub version, should be an integer (ex. 5.256 -> Sub version is '256') */
	AdiUInt8 unitSubVer;
	AdiUInt16 unitRevision;
	/* Unit id - IOUnit, FIB, LUED21-4... */
	E_UNIT_ID unitId;
	/* CBit status, enumeration - result of performing continuous BIT testing */
	E_CBIT_STATUS cbitStatus;
	/* Registration status -Enumeration - unregistered, valid, failed indices */
	E_REGISTRATION_STATUS registerStatus;
	/* enum - IDLE, PROGRAMMING, OPERATIONAL */
	E_UNIT_STATE sysState;
} sRegInfo_STRUCT;

#endif /* _STRUCTURE_TYPE_sRegInfo_STRUCT */

/* REGISTERED_UNITS_ARR */
#ifndef _ARRAY_TYPE_REGISTERED_UNITS_ARR
#define _ARRAY_TYPE_REGISTERED_UNITS_ARR

typedef sRegInfo_STRUCT REGISTERED_UNITS_ARR[10];

#endif /* _ARRAY_TYPE_REGISTERED_UNITS_ARR */

/* REGISTERED_UNITS_STRUCT */
#ifndef _STRUCTURE_TYPE_REGISTERED_UNITS_STRUCT
#define _STRUCTURE_TYPE_REGISTERED_UNITS_STRUCT

typedef struct REGISTERED_UNITS_STRUCT{
	AdiUInt32 length;
	REGISTERED_UNITS_ARR registeredUnits;
} REGISTERED_UNITS_STRUCT;

#endif /* _STRUCTURE_TYPE_REGISTERED_UNITS_STRUCT */

/* sDiscInfo_STRUCT */
#ifndef _STRUCTURE_TYPE_sDiscInfo_STRUCT
#define _STRUCTURE_TYPE_sDiscInfo_STRUCT

typedef struct sDiscInfo_STRUCT{
	/* Discrete's ID */
	E_DISCRETE_ID discreteId;
	/* Discrete's value */
	E_BOOLEAN discreteValue;
} sDiscInfo_STRUCT;

#endif /* _STRUCTURE_TYPE_sDiscInfo_STRUCT */

/* PERIODIC_DISC_INFO_ARR */
#ifndef _ARRAY_TYPE_PERIODIC_DISC_INFO_ARR
#define _ARRAY_TYPE_PERIODIC_DISC_INFO_ARR

typedef sDiscInfo_STRUCT PERIODIC_DISC_INFO_ARR[256];

#endif /* _ARRAY_TYPE_PERIODIC_DISC_INFO_ARR */

/* MONITORED_DATA_DISC_STRUCT */
#ifndef _STRUCTURE_TYPE_MONITORED_DATA_DISC_STRUCT
#define _STRUCTURE_TYPE_MONITORED_DATA_DISC_STRUCT

typedef struct MONITORED_DATA_DISC_STRUCT{
	AdiUInt32 length;
	PERIODIC_DISC_INFO_ARR periodicDiscInfo;
} MONITORED_DATA_DISC_STRUCT;

#endif /* _STRUCTURE_TYPE_MONITORED_DATA_DISC_STRUCT */

/* sAnalogInfo_STRUCT */
#ifndef _STRUCTURE_TYPE_sAnalogInfo_STRUCT
#define _STRUCTURE_TYPE_sAnalogInfo_STRUCT

typedef struct sAnalogInfo_STRUCT{
	/* Numerical Indentification of analog measurement */
	AdiUInt32 analogId;
	AdiFloat32 analogVal;
} sAnalogInfo_STRUCT;

#endif /* _STRUCTURE_TYPE_sAnalogInfo_STRUCT */

/* ANALOG_INPUTS_INFO_ARR */
#ifndef _ARRAY_TYPE_ANALOG_INPUTS_INFO_ARR
#define _ARRAY_TYPE_ANALOG_INPUTS_INFO_ARR

typedef sAnalogInfo_STRUCT ANALOG_INPUTS_INFO_ARR[100];

#endif /* _ARRAY_TYPE_ANALOG_INPUTS_INFO_ARR */

/* ANALOG_INPUTS_STRUCT */
#ifndef _STRUCTURE_TYPE_ANALOG_INPUTS_STRUCT
#define _STRUCTURE_TYPE_ANALOG_INPUTS_STRUCT

typedef struct ANALOG_INPUTS_STRUCT{
	AdiUInt32 length;
	ANALOG_INPUTS_INFO_ARR analogInputsInfo;
} ANALOG_INPUTS_STRUCT;

#endif /* _STRUCTURE_TYPE_ANALOG_INPUTS_STRUCT */

/* sSerialInfo_STRUCT */
#ifndef _STRUCTURE_TYPE_sSerialInfo_STRUCT
#define _STRUCTURE_TYPE_sSerialInfo_STRUCT

typedef struct sSerialInfo_STRUCT{
	/* Numerical indentification of serial communication channel in IO Unit */
	AdiUInt32 serialId;
	BUFFER_STRUCT data;
} sSerialInfo_STRUCT;

#endif /* _STRUCTURE_TYPE_sSerialInfo_STRUCT */

/* SERIAL_INPUTS_INFO_ARR */
#ifndef _ARRAY_TYPE_SERIAL_INPUTS_INFO_ARR
#define _ARRAY_TYPE_SERIAL_INPUTS_INFO_ARR

typedef sSerialInfo_STRUCT SERIAL_INPUTS_INFO_ARR[10];

#endif /* _ARRAY_TYPE_SERIAL_INPUTS_INFO_ARR */

/* SERIAL_INPUTS_STRUCT */
#ifndef _STRUCTURE_TYPE_SERIAL_INPUTS_STRUCT
#define _STRUCTURE_TYPE_SERIAL_INPUTS_STRUCT

typedef struct SERIAL_INPUTS_STRUCT{
	AdiUInt32 length;
	SERIAL_INPUTS_INFO_ARR serialInputsInfo;
} SERIAL_INPUTS_STRUCT;

#endif /* _STRUCTURE_TYPE_SERIAL_INPUTS_STRUCT */

/* DT_MONITORED_DATA_STRUCT */
#ifndef _STRUCTURE_TYPE_DT_MONITORED_DATA_STRUCT
#define _STRUCTURE_TYPE_DT_MONITORED_DATA_STRUCT

typedef struct DT_MONITORED_DATA_STRUCT{
	REGISTERED_UNITS_STRUCT registeredUnits;
	sRegInfo_STRUCT internalUnitInfo;
	MONITORED_DATA_DISC_STRUCT periodicDiscInfo;
	ANALOG_INPUTS_STRUCT analogInputsInfo;
	SERIAL_INPUTS_STRUCT serialInputsInfo;
} DT_MONITORED_DATA_STRUCT;

#endif /* _STRUCTURE_TYPE_DT_MONITORED_DATA_STRUCT */



#ifdef __cplusplus
}
#endif

#endif 
