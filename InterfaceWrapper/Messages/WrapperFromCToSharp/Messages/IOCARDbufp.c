/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDbufp.c                                                                                                                    */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Public declaration of the I/O buffers related to IOCARD CSCI                                                                    */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDbufp_c
#define IOCARDbufp_c

#include "IOCARDbufplen.h"
#include "typelib.h"

#ifdef __cplusplus
extern "C" {
#endif

AdiUInt8                                       HdrOnlyMessageIo2lcu[HdrOnlyMessageIo2lcu_LEN];
AdiUInt8                                       HdrOnlyMessageLcu2io[HdrOnlyMessageLcu2io_LEN];
AdiUInt8                                       KeepAliveMessage[KeepAliveMessage_LEN];
AdiUInt8                                       LogDataMessage[LogDataMessage_LEN];
AdiUInt8                                       NonceMessage[NonceMessage_LEN];
AdiUInt8                                       RequestStatusMessage[RequestStatusMessage_LEN];
AdiUInt8                                       SerialIo2lcuMessage[SerialIo2lcuMessage_LEN];
AdiUInt8                                       SerialLcu2ioMessage[SerialLcu2ioMessage_LEN];
AdiUInt8                                       SetDiscretesMessage[SetDiscretesMessage_LEN];
AdiUInt8                                       SetSystemStateMessage[SetSystemStateMessage_LEN];
AdiUInt8                                       StartInterfaceMessage[StartInterfaceMessage_LEN];
AdiUInt8                                       StatusMessage[StatusMessage_LEN];
AdiUInt8                                       StopInterfaceMessage[StopInterfaceMessage_LEN];
AdiUInt8                                       TlOnlyMessageIo2lcu[TlOnlyMessageIo2lcu_LEN];
AdiUInt8                                       TlOnlyMessageLcu2io[TlOnlyMessageLcu2io_LEN];


#ifdef __cplusplus
}
#endif

#endif 
